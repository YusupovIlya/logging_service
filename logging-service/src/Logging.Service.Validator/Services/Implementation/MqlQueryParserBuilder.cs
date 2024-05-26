namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Реализация интерфейса строителя mql парсера.
    /// </summary>
    public class MqlQueryParserBuilder : IMqlQueryParserBuilder
    {
        /// <summary>
        /// Создать парсер.
        /// </summary>
        public MqlQueryParser Create() =>
            new(new MqlFunctionBuilder());
    }
}
