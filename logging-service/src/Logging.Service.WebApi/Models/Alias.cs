namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Алиас.
    /// </summary>
    public class Alias
    {
        /// <summary>
        /// Идентификатор пространства пользователей.
        /// </summary>
        public long UserspaceId { get; set; }

        /// <summary>
        /// Название поля.
        /// </summary>
        public string FieldName { get; set; } = default!;

        /// <summary>
        /// Значение алиаса.
        /// </summary>
        public string Value { get; set; } = default!;

        /// <summary>
        /// Применяется ли алиас по умолчанию.
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}
