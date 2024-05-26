using Logging.Server.Models.StreamData.Api.Schemas;

namespace Logging.Server.StreamData.Validator.Models
{
    /// <summary>
    /// Агрегационный запрос.
    /// </summary>
    public class AggregationQuery
    {
        /// <summary>
        /// Тип агрегационной функции.
        /// </summary>
        public AggregationType Type { get; set; }

        /// <summary>
        /// Поле, по которому будет выполняться агрегация.
        /// </summary>
        public string Column { get; set; } = default!;

        /// <summary>
        /// Необязательное поле, значение для функции <see cref="AggregationType.Count"/>.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Необязательное поле, тип значения для функции <see cref="AggregationType.Count"/>.
        /// </summary>
        public StreamDataSchemaColumnType? ValueType { get; set; }

        /// <summary>
        /// Необязательное поле, количество значений топа <see cref="AggregationType.Top"/>.
        /// </summary>
        public int Count { get; set; } = 5;
    }
}
