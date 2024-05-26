namespace Logging.Server.StreamData.Validator.Models
{
    /// <summary>
    /// Виды выражений.
    /// </summary>
    public enum TermType
    {
        /// <summary>
        /// Поиск по всем полям.
        /// </summary>
        FullSearchTerm,

        /// <summary>
        /// Обычный поиск с явным указанием поля.
        /// </summary>
        CommonTerm,

        /// <summary>
        /// Поиск экранированного значения с явным указанием поля.
        /// </summary>
        EscapedTerm
    }
}