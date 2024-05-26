using ClickHouse.Client;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Logging.Server.Models.StreamData.Extensions;
using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.Extensions;
using Logging.Server.Service.StreamData.Models;
using Monq.Core.MvcExtensions.Extensions;
using Logging.Server.StreamData.Validator.Models;
using Monq.Models.Abstractions.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Logging.Server.Service.StreamData.Configuration.AppConstants;
using static Logging.Server.Service.StreamData.Configuration.AppConstants.ClickHouse;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Репозиторий для чтения потоковых данных.
    /// </summary>
    public class StreamDataRepositoryImpl : BaseRepository, IStreamDataRepository
    {
        readonly ILogger<StreamDataRepositoryImpl> _logger;
        static readonly AggregationType[] _nonScalarAggregation =
        {
            AggregationType.Top,
            AggregationType.Mode,
            AggregationType.Count,
            AggregationType.Uniq
        };

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataRepositoryImpl"/>.
        /// </summary>
        /// <param name="clickHouseOptions">Конфигурация ClickHouse.</param>
        /// <param name="logger">Сервис логирования.</param>
        public StreamDataRepositoryImpl(
            IOptions<ClickHouseOptions> clickHouseOptions,
            ILogger<StreamDataRepositoryImpl> logger) : base(clickHouseOptions) =>
            _logger = logger;

        /// <summary>
        /// Получить список всех таблиц с потоками данных.
        /// </summary>
        /// <param name="streamIds">Список идентификаторов потоков данных.</param>
        public async Task<IEnumerable<long>> GetExistingStreamIds(IEnumerable<long> streamIds)
        {
            await using var connection = GetConnection();

            var sql = @$"SELECT toInt64(streamId)
            FROM system.tables
            ARRAY JOIN [{string.Join(',', streamIds)}] AS streamId
            WHERE database = '{connection.Database}' AND name = concat('{StreamDataTablePrefix}', toString(streamId))";

            await using var reader = await connection.ExecuteReaderAsync(sql);
            var result = await reader.FetchRecords<long>();
            return result;
        }

        /// <summary>
        /// Получить результат агрегации.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="query">Запрос.</param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события.</param>
        public async Task<IEnumerable<AggregationResultModel>> GetAggregation(
            IEnumerable<Schema> schemas,
            AggregationQuery query,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp)
        {
            if (schemas.IsEmpty())
                return Array.Empty<AggregationResultModel>();

            var nestedQueries = schemas.Select(x =>
            {
                var preWhereExpressions = GetPrewhereExpressions(timestamp, x.Conditions);
                return GetNestedQueriesForAggregationFunction(x.TableName, query.Column, preWhereExpressions);
            });

            await using var connection = GetConnection();
            var command = connection.CreateCommand();
            foreach (var (value, name) in parameters)
                command.AddParameter(name, value);
            if (_nonScalarAggregation.Contains(query.Type))
            {
                command.CommandText = BuildNonScalarAggregationSql(query, nestedQueries);
                await using var nonScalarReader = await command.ExecuteReaderAsync();
                return await nonScalarReader.FetchRecords<AggregationResultModel>();
            }

            command.CommandText = BuildScalarAggregationSql(query, nestedQueries);
            await using var scalarReader = await command.ExecuteReaderAsync();
            var result = await command.ExecuteScalarAsync();
            return new[] { new AggregationResultModel { Value = Convert.ToDouble(result) } };
        }

        static string BuildNonScalarAggregationSql(AggregationQuery query, IEnumerable<string> nestedQueries) =>
            $"SELECT CAST(Count(`{query.Column}`) AS DOUBLE) AS Value, toString(`{query.Column}`) AS Key FROM ({UnionAll(nestedQueries)})"
            + $" GROUP BY `{query.Column}` {query.Value}";

        static string BuildScalarAggregationSql(AggregationQuery query, IEnumerable<string> nestedQueries) =>
            $"SELECT CAST({query.Type.ToString().ToLower()}(`{query.Column}`) AS DOUBLE) FROM ({UnionAll(nestedQueries)})";

        static string GetNestedQueriesForAggregationFunction(string tableName, string field, string[] preWhereExpressions)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ").Append(field.AddQuotes())
                .AppendLine()
                .Append("FROM ").AppendLine(tableName)
                .Append(" PREWHERE `_id` IN(")
                .AppendLine("SELECT `_id`")
                .Append("FROM ").AppendLine(tableName)
                .AppendPreWhereExpression(preWhereExpressions).AppendLine()
                .Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Получить события по фильтру.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="limit">Макс. кол-во событий, которые будут получены.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        public async Task<IEnumerable<BaseStreamDataEvent>> GetEventsByFilter(
            IEnumerable<Schema> schemas,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp,
            int limit,
            string timestampSortDir = "desc")
        {
            if (schemas.IsEmpty())
                return Enumerable.Empty<BaseStreamDataEvent>();

            await using var connection = GetConnection();
            var command = connection.CreateCommand();
            foreach (var (value, name) in parameters)
                command.AddParameter(name, value);
            command.CommandText = BuildQueryForEventsList(schemas, timestamp, limit, timestampSortDir);

            await using var reader = await command.ExecuteReaderAsync();
            var result = await reader.FetchRecords<BaseStreamDataEvent>();
            return result;
        }

        /// <summary>
        /// Получить события по фильтру в виде csv файла.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="checkedFields">Выбранные поля.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        public async Task<byte[]> GetCsvFile(
            IEnumerable<Schema> schemas,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp,
            IEnumerable<string> checkedFields,
            string timestampSortDir = "desc")
        {
            if (schemas.IsEmpty())
                return Array.Empty<byte>();

            var serviceFields = checkedFields.Where(val => val.IsServiceField()).ToList();

            var sql = BuildQueryForCsvFile(
                schemas,
                timestamp,
                timestampSortDir,
                checkedFields.Where(val => !serviceFields.Contains(val)).ToList(),
                serviceFields.Select(val => val.AddQuotes()).ToList());

            await using var connection = GetConnection();

            var command = connection.CreateCommand();
            foreach (var (value, name) in parameters)
                command.AddParameter(name, value);
            command.CommandText = sql;
            var result = await command.ExecuteRawResultAsync(new CancellationToken(false));
            return await result.ReadAsByteArrayAsync();
        }

        static string[] GetPrewhereExpressions(DateRangePostViewModel timestamp, string conditions)
        {
            var preWhereExpressions = GetPrewhereExpressions(timestamp).ToHashSet();
            if (!string.IsNullOrWhiteSpace(conditions))
                preWhereExpressions.Add(conditions);
            return preWhereExpressions.ToArray();
        }

        static string BuildQueryForCsvFile(
            IEnumerable<Schema> schemas,
            DateRangePostViewModel timestamp,
            string timestampSortDir,
            IEnumerable<string> checkedFields,
            IEnumerable<string> serviceFields)
        {
            var nestedQueries = schemas.Select(x =>
            {
                var preWhereExpressions = GetPrewhereExpressions(timestamp, x.Conditions);
                var fields = serviceFields.Concat(checkedFields.Select(val => x.StreamFields.Contains(val) ? $"toString(`{val}`) AS `{val}`" : "''"));
                return GetSelectQueryForEventsList(x.TableName, fields, preWhereExpressions, int.MaxValue, timestampSortDir);
            });

            var sql = $"SELECT * FROM ({UnionAll(nestedQueries)}) ORDER BY `@timestamp` {timestampSortDir} FORMAT CSVWithNames";
            return sql;
        }

        /// <summary>
        /// Получить агрегированные данные по событиям, которые удовлетворяют фильтру.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="interval">Интервал (в сек.), по которому будут разбиты агрегированные данные по кол-ву событий за период времени.</param>
        public async Task<IEnumerable<StreamDataAggregateModel>> GetAggregationsByFilter(
            IEnumerable<Schema> schemas,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp,
            uint interval)
        {
            if (schemas.IsEmpty())
                return GetEmptyAggregations(timestamp, interval).ToList();

            await using var connection = GetConnection();
            var command = connection.CreateCommand();
            foreach (var (value, name) in parameters)
                command.AddParameter(name, value);
            command.CommandText = BuildQueryForAggregations(schemas, timestamp, interval);

            await using var reader = await command.ExecuteReaderAsync();

            var result = await reader.FetchRecords<StreamDataAggregateModel>();
            return result;
        }

        /// <summary>
        /// Получить количество событий потока данных.
        /// </summary>
        /// <param name="streamId">Идентификатор потока данных.</param>
        /// <returns>Количество событий.</returns>
        public async Task<ulong> GetCount(long streamId)
        {
            var sql = $"SELECT COUNT() FROM {StreamDataTablePrefix}{streamId}";
            await using var connection = GetConnection();
            try
            {
                var count = await connection.ExecuteScalarAsync(sql);
                return Convert.ToUInt64(count);
            }
            catch (ClickHouseServerException)
            {
                _logger.LogInformation($"Stream data table {streamId} is not found.");
                return default;
            }
        }

        static IEnumerable<StreamDataAggregateModel> GetEmptyAggregations(DateRangePostViewModel timestamp, uint interval)
        {
            var slotsCount = (timestamp.End - timestamp.Start).TotalSeconds / interval;
            for (var i = 0; i <= slotsCount; i++)
                yield return new StreamDataAggregateModel(timestamp.Start.AddSeconds(interval * i));
        }

        static string BuildQueryForAggregations(IEnumerable<Schema> schemas, DateRangePostViewModel timestamp, uint interval)
        {
            var nestedQueries = schemas.Select(x =>
            {
                var preWhereExpressions = GetPrewhereExpressions(timestamp, x.Conditions);
                return GetNestedQueryForAggregations(x.TableName, preWhereExpressions, interval, timestamp.Start.ToUnixTimeSeconds());
            });

            return $"SELECT slot, events_count FROM (SELECT slot, SUM(events_count) AS events_count FROM ({UnionAll(nestedQueries)}) GROUP BY slot) ORDER BY slot";
        }

        static string GetNestedQueryForAggregations(string tableName, string[] preWhereExpressions, uint interval, long start)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SELECT toDateTime({0} + intDiv((toUnixTimestamp(`@timestamp`) - {0}), {1}) * {1}) AS slot, COUNT() AS events_count ", start, interval)
                .AppendLine("FROM ").AppendLine(tableName)
                .AppendPreWhereExpression(preWhereExpressions)
                .Append("GROUP BY slot");

            return sb.ToString();
        }

        static string BuildQueryForEventsList(
            IEnumerable<Schema> schemas,
            DateRangePostViewModel timestamp,
            int limit,
            string timestampSortDir,
            IEnumerable<string>? checkedFields = null)
        {
            var fields = Validate(checkedFields);

            var nestedQueries = schemas.Select(x =>
            {
                var preWhereExpressions = GetPrewhereExpressions(timestamp, x.Conditions);
                return GetSelectQueryForEventsList(x.TableName, fields, preWhereExpressions, limit, timestampSortDir);
            });

            var query = new StringBuilder();
            query.AppendLine("SELECT *")
                .AppendLine("FROM")
                .AppendLine("(")
                    .AppendLine(UnionAll(nestedQueries))
                .AppendLine(")")
                .Append("ORDER BY `@timestamp` ").AppendLine(timestampSortDir)
                .Append("LIMIT ").Append(limit);

            var sql = query.ToString();
            return sql;
        }

        static string GetSelectQueryForEventsList(
            string tableName,
            IEnumerable<string> fields,
            string[] preWhereExpressions,
            int limit,
            string timestampSortDir)
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ").AppendJoin(", ", fields).AppendLine()
                .Append("FROM ").AppendLine(tableName);

            if (timestampSortDir.Equals("asc", StringComparison.OrdinalIgnoreCase))
                sb.AppendLine()
                    .AppendLine("ORDER BY `_date` ASC, `@timestamp` ASC")
                    .Append("LIMIT ").Append(limit);
            else
                sb.Append(" PREWHERE `_id` IN(")
                    .AppendLine("SELECT `_id`")
                    .Append("FROM ").AppendLine(tableName)
                    .AppendPreWhereExpression(preWhereExpressions).AppendLine()
                    .AppendLine("ORDER BY `_date` DESC, `@timestamp` DESC")
                    .Append("LIMIT ").Append(limit)
                    .Append(')');

            return sb.ToString();
        }

        static IEnumerable<string> Validate(IEnumerable<string>? fields)
        {
            var allFields = GetAllFields().ToList();

            if (fields.IsEmpty())
                return allFields;

            var checkedFields = fields
                .Where(field => allFields.Contains(field))
                .Union(new[] { "_id", "@timestamp" })
                .ToList();

            if (fields.Any(val => val.StartsWith(SourcePrefix)))
                checkedFields.Add(SourceRawJson);
            if (fields.Any(val => val.StartsWith(LabelsPrefix)))
                checkedFields.Add(LabelsRawJson);

            return checkedFields;
        }

        static string[] GetPrewhereExpressions(DateRangePostViewModel timestamp) => new[]
        {
            timestamp.GetSqlExpression(typeof(BaseStreamDataEvent).GetProperty(nameof(BaseStreamDataEvent.Date))?.TryGetClickHouseName(), DateTimeExtensions.ToDate),
            timestamp.GetSqlExpression(typeof(BaseStreamDataEvent).GetProperty(nameof(BaseStreamDataEvent.Timestamp))?.TryGetClickHouseName(), DateTimeExtensions.ToDateTime64)
        };

        static IEnumerable<string> GetAllFields() =>
            typeof(BaseStreamDataEvent)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<IgnoreDataMemberAttribute>() is null)
                .Select(x => x.TryGetClickHouseName())
                .ToList();

        static string UnionAll(IEnumerable<string> selections) =>
            string.Join(Environment.NewLine + "UNION ALL" + Environment.NewLine, selections);
    }
}