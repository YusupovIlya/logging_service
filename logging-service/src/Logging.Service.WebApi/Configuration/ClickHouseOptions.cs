namespace Logging.Server.Service.StreamData.Configuration
{
    /// <summary>
    /// Конфигурация ClickHouse.
    /// </summary>
    public class ClickHouseOptions
    {
        /// <summary>
        /// Строка соединения.
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }
}
