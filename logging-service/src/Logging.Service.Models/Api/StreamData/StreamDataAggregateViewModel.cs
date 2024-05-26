using System;
using Newtonsoft.Json;

namespace Logging.Server.Models.StreamData.Api
{
    /// <summary>
    /// Модель представления количества событий за определенный период времени.
    /// </summary>
    public class StreamDataAggregateViewModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateViewModel"/>.
        /// </summary>
        public StreamDataAggregateViewModel()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateViewModel"/>.
        /// </summary>
        /// <param name="timestamp">Конечная отметка периода, в которой агрегированы события.</param>
        public StreamDataAggregateViewModel(DateTimeOffset timestamp) => Timestamp = timestamp;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateViewModel"/>.
        /// </summary>
        /// <param name="timestamp">Конечная отметка периода, в которой агрегированы события.</param>
        /// <param name="count">Кол-во событий за данный период.</param>
        public StreamDataAggregateViewModel(DateTimeOffset timestamp, long count)
        {
            Timestamp = timestamp;
            Count = count;
        }

        /// <summary>
        /// Конечная отметка периода, в которой агрегированы события.
        /// </summary>
        [JsonProperty(PropertyName = "@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Кол-во событий за данный период.
        /// </summary>
        public long Count { get; set; }
    }
}