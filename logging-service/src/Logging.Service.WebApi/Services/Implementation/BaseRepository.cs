using ClickHouse.Client.ADO;
using Microsoft.Extensions.Options;
using Logging.Server.Service.StreamData.Configuration;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Базовый репозиторий для взаимодействия с ClickHouse.
    /// </summary>
    public abstract class BaseRepository
    {
        readonly ClickHouseOptions _clickHouseOptions;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BaseRepository"/>.
        /// </summary>
        /// <param name="clickHouseOptions">Конфигурация ClickHouse.</param>
        protected BaseRepository(IOptions<ClickHouseOptions> clickHouseOptions) => 
            _clickHouseOptions = clickHouseOptions.Value;
        
        /// <summary>
        /// Получить объект соединения с ClickHouse.
        /// </summary>
        /// <returns></returns>
        protected ClickHouseConnection GetConnection()
        {
            var con = new ClickHouseConnection(_clickHouseOptions.ConnectionString);
            return con;
        }
    }
}