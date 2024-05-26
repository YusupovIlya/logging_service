using ClickHouse.Client;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.Extensions;
using Logging.Server.Service.StreamData.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Реализация репозитория схем потоковых данных.
    /// </summary>
    public class StreamDataSchemasRepository : IStreamDataSchemasRepository
    {
        readonly ClickHouseOptions _clickHouseOptions;
        readonly ILogger<StreamDataSchemasRepository> _logger;

        /// <summary>
        /// Конструктор реализации репозитория схем потоковых данных.
        /// Создаёт новый экземпляр класса <see cref="StreamDataSchemasRepository"/>.
        /// </summary>
        public StreamDataSchemasRepository(
            IOptions<ClickHouseOptions> clickHouseOptions,
            ILogger<StreamDataSchemasRepository> logger)
        {
            _clickHouseOptions = clickHouseOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Создать новую таблицу БД по указанной схеме.
        /// </summary>
        /// <param name="value">Модель</param>
        /// <returns><see cref="Task"/>, показывающий завершение операции.</returns>
        public Task Create(StreamDataSchemaPostViewModel value)
        {
            var sql = GetCreateTableCommand(value);
            using var connection = GetConnection();
            return connection.ExecuteStatementAsync(sql);
        }

        /// <summary>
        /// Получить схему таблицы БД по идентификатору потока.
        /// </summary>
        /// <param name="streamId">Идентификатор потока.</param>
        /// <returns>Схема потоковых данных (см. <see cref="StreamDataSchemaViewModel"/>).</returns>
        public async Task<StreamDataSchemaViewModel?> Get(long streamId)
        {
            var schema = new StreamDataSchemaViewModel
            {
                StreamId = streamId
            };
            var sql = $"DESCRIBE {GetTableName(streamId)}";
            await using var connection = GetConnection();
            try
            {
                var reader = await connection.ExecuteReaderAsync(sql);
                if (!reader.HasRows)
                    return null;
                while (await reader.ReadAsync())
                {
                    var columnName = reader.GetString(0);

                    // Получаем схему только для специфических полей данных потока.
                    if (!columnName.StartsWith(AppConstants.SourcePrefix) &&
                        !columnName.StartsWith(AppConstants.LabelsPrefix))
                        continue;

                    var columnType = reader.GetString(1).GetColumnType();
                    var columnDefaultValue = reader.GetString(3);
                    var column = new StreamDataSchemaColumnViewModel(
                        columnName,
                        columnType.Type,
                        columnType.ElementType,
                        columnDefaultValue);
                    schema.Columns = schema.Columns.Append(column);
                }
                return schema;
            }
            catch (ClickHouseServerException)
            {
                _logger.LogInformation($"Таблица данных потока {streamId} не найдена.");
                return null;
            }
        }

        /// <summary>
        /// Проверить, существует ли таблица потоковых данных.
        /// </summary>
        /// <param name="streamId">Идентификатор потока.</param>
        /// <returns>Флаг существования таблицы.</returns>
        public async Task<bool> Exists(long streamId)
        {
            var sql = $"EXISTS {GetTableName(streamId)}";
            await using var connection = GetConnection();
            try
            {
                var exists = await connection.ExecuteScalarAsync(sql);
                return Convert.ToBoolean(exists);
            }
            catch (ClickHouseServerException)
            {
                _logger.LogInformation($"Таблица данных потока {streamId} не найдена.");
                return false;
            }
        }

        /// <summary>
        /// Обновить схему потоковых данных.
        /// </summary>
        /// <param name="value">Модель для обновления схемы потоковых данных (см. <see cref="StreamDataSchemaPutViewModel"/>).</param>
        /// <returns><see cref="Task"/>, показывающий завершение операции.</returns>
        public async Task Update(StreamDataSchemaPutViewModel value)
        {
            if (!value.ColumnsToAdd.Any())
                return;

            var sql = GetUpdateTableCommand(value);
            await using var connection = GetConnection();
            await connection.ExecuteStatementAsync(sql);
        }

        static string GetTableName(long streamId) => $"{AppConstants.ClickHouse.StreamDataTablePrefix}{streamId}";

        ClickHouseConnection GetConnection() => new(_clickHouseOptions.ConnectionString);

        static string GetCreateTableCommand(StreamDataSchemaPostViewModel value)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}", GetTableName(value.StreamId)).Append(Environment.NewLine).Append('(');

            var baseProperties = typeof(BaseStreamDataEvent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.GetClickHouseColumnCommand())
                .ToList();
            var otherProperties = value.Columns
                .Select(p => p.GetClickHouseColumnCommand())
                .ToList();
            var allProperties = baseProperties
                .Union(otherProperties)
                .Aggregate(string.Empty, (total, prop) => $"{total}{prop}, ")
                .TrimEnd(", ".ToCharArray());

            sb.Append(allProperties).AppendLine(")");
            sb.AppendLine("ENGINE = MergeTree");
            sb.AppendLine("PARTITION BY `_date`");
            sb.AppendLine("ORDER BY (`_date`, `@timestamp`)");
            sb.AppendLine("SETTINGS enable_mixed_granularity_parts = 1, index_granularity = 8192;");

            return sb.ToString();
        }

        static string GetUpdateTableCommand(StreamDataSchemaPutViewModel value)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("ALTER TABLE {0}", GetTableName(value.StreamId));

            var addColumnCommands = value.ColumnsToAdd
                .Select(c => $"ADD COLUMN IF NOT EXISTS {c.GetClickHouseColumnCommand()}")
                .Aggregate(string.Empty, (total, command) => $"{total}{Environment.NewLine}{command},")
                .TrimEnd(',');
            sb.AppendLine(addColumnCommands);

            return sb.ToString();
        }
    }
}
