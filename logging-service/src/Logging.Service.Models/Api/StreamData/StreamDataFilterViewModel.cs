using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Monq.Models.Abstractions.v2;
using Newtonsoft.Json;

namespace Logging.Server.Models.StreamData.Api
{
    /// <summary>
    /// Принимаемая модель фильтра событий потока данных.
    /// </summary>
    public class StreamDataFilterViewModel
    {
        /// <summary>
        /// Строка запроса.
        /// </summary>
        public string? Query { get; set; } = null;

        /// <summary>
        /// Количество возвращаемых отфильтрованных данных.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Amount of returned filtered data must be in the given range.")]
        public int FilteredCount { get; set; } = 500;

        /// <summary>
        /// Интервал (в сек.), по которому будут разбиты агрегированные данные по кол-ву событий за период времени.
        /// </summary>
        [Required(ErrorMessage = "Не указан интервал, по которому будут разбиты агрегированные данные по кол-ву событий за период времени.")]
        public uint Interval { get; set; }

        /// <summary>
        /// Дата регистрации события.
        /// </summary>
        [JsonProperty(PropertyName = "@timestamp")]
        //[Required(ErrorMessage = "The date range of events registration is not specified")]
        public DateRangePostViewModel Timestamp { get; set; } = default!;

        /// <summary>
        /// Список идентификаторов потоков данных.
        /// </summary>
        [JsonProperty(PropertyName = "_streamIds")]
        public IEnumerable<long>? StreamIds { get; set; } = null;
    }
}