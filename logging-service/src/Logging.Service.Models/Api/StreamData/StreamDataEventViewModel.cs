using Newtonsoft.Json;
using System;

namespace Logging.Server.Models.StreamData.Api
{
    /// <summary>
    /// Модель представления события потока данных.
    /// </summary>
    public class StreamDataEventViewModel
    {
        /// <summary>
        /// Идентификатор события.
        /// </summary>
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Пользовательская отметка времени.
        /// </summary>
        [JsonProperty("@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Исходная модель события.
        /// </summary>
        [JsonProperty("source")]
        public object Source { get; set; } = default!;

        /// <summary>
        /// Метки.
        /// </summary>
        //[JsonProperty("_labels")]
        //public object? Labels { get; set; }
    }
}