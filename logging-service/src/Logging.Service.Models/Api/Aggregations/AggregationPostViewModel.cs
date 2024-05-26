using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Monq.Models.Abstractions.v2;
using Newtonsoft.Json;

namespace Logging.Server.Models.StreamData.Api.Aggregations
{
    /// <summary>
    /// Модель агрегации.
    /// </summary>
    public class AggregationPostViewModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AggregationPostViewModel"/>.
        /// </summary>
        public AggregationPostViewModel()
        { }

        /// <summary>
        /// Строка запроса, определяющая выборку в которой будет производится агрегация.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Строка запроса.
        /// </summary>
        public string? AggregationQuery { get; set; }

        /// <summary>
        /// Список идентификаторов потоков данных.
        /// </summary>
        public IEnumerable<long>? StreamIds { get; set; }

        /// <summary>
        /// Дата регистрации события.
        /// </summary>
        [JsonProperty(PropertyName = "@timestamp")]
        [Required(ErrorMessage = "The date range of events registration is not specified")]
        public DateRangePostViewModel Timestamp { get; set; } = default!;
    }
}
