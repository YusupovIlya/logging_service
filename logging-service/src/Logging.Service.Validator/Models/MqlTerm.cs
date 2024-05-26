namespace Logging.Server.StreamData.Validator.Models
{
    /// <summary>
    /// Выражение.
    /// </summary>
    public class MqlTerm
    {
        /// <summary>
        /// Тип выражения.
        /// </summary>
        public TermType Type { get; set; }

        /// <summary>
        /// Поле, участвующее в выражении.
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Значение, использующееся в выражении.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}