namespace Logging.Server.StreamData.Validator.Models
{
    /// <summary>
    /// Тип агрегационной функций.
    /// </summary>
    public enum AggregationType
    {
        /// <summary>
        /// Получить список топ-значений.
        /// </summary>
        Top,

        /// <summary>
        /// Получить среднее значение.
        /// </summary>
        Avg,

        /// <summary>
        /// Получить максимальное значение.
        /// </summary>
        Max,

        /// <summary>
        /// Получить минимальное значение.
        /// </summary>
        Min,

        /// <summary>
        /// Получить медиану.
        /// </summary>
        Median,

        /// <summary>
        /// Получить самое часто встречающее значение.
        /// </summary>
        Mode,

        /// <summary>
        /// Получить количество.
        /// </summary>
        Count,

        /// <summary>
        /// Получить количество уникальных значений.
        /// </summary>
        Uniq
    }
}
