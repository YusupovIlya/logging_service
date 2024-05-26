namespace Logging.Server.Models.StreamData.Api
{
    /// <summary>
    /// Алиас.
    /// </summary>
    public class AliasViewModel
    {
        /// <summary>
        /// Идентификатор пространства пользователей.
        /// </summary>
        public long UserspaceId { get; set; }

        /// <summary>
        /// Название поля.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Значение алиаса.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Применяется ли алиас по умолчанию.
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}
