using Logging.Server.StreamData.Validator.Services.Implementation;

namespace Logging.Server.StreamData.Validator.Services
{
    /// <summary>
    /// Интерфейс-обёртка для строителя mql парсера.
    /// </summary>
    public interface IMqlQueryParserBuilder
    {
        /// <summary>
        /// Создать парсер.
        /// </summary>
        MqlQueryParser Create();
    }
}
