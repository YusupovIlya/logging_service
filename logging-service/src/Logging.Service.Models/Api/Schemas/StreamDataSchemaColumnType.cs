namespace Logging.Server.Models.StreamData.Api.Schemas
{
    /// <summary>
    /// Тип колонки схемы потоковых данных.
    /// </summary>
    public enum StreamDataSchemaColumnType
    {
        /// <summary>
        /// Неизвестный тип.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Целое число.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// Число с плавающей точкой.
        /// </summary>
        Double = 2,

        /// <summary>
        /// Строка.
        /// </summary>
        String = 3,

        /// <summary>
        /// Дата.
        /// </summary>
        Date = 4,

        /// <summary>
        /// Логический тип.
        /// </summary>
        Bool = 5,

        /// <summary>
        /// Массив.
        /// </summary>
        Array = 6,

        /// <summary>
        /// Универсальный уникальный идентификатор.
        /// </summary>
        Guid = 7,
    }
}
