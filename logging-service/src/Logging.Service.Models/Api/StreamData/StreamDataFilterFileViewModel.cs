using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Monq.Models.Abstractions.v2;
using Newtonsoft.Json;

namespace Logging.Server.Models.StreamData.Api.StreamData
{
    /// <summary>
    /// Принимаемая модель получения файла с отфильтрованными событиями потока данных.
    /// </summary>
    public class StreamDataFilterFileViewModel
    {
        /// <summary>
        /// Строка запроса.
        /// </summary>
        public string? Query { get; set; } = null;

        /// <summary>
        /// Выбранные поля.
        /// </summary>
        public IEnumerable<string>? CheckedFields { get; set; } = null;

        /// <summary>
        /// Дата регистрации события.
        /// </summary>
        [JsonProperty(PropertyName = "@timestamp")]
        [Required(ErrorMessage = "The date range of events registration is not specified")]
        public DateRangePostViewModel Timestamp { get; set; }

        /// <summary>
        /// Список идентификаторов потоков данных.
        /// </summary>
        [JsonProperty(PropertyName = "_streamIds")]
        public IEnumerable<long>? StreamIds { get; set; } = null;
    }
}
