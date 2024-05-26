using System;
using Logging.Server.Service.StreamData.Infrastructure;

namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Модель агрегированных данных по событиям за определенный период времени.
    /// </summary>
    public class StreamDataAggregateModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateModel"/>.
        /// </summary>
        public StreamDataAggregateModel()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateModel"/>.
        /// </summary>
        /// <param name="timestamp">Конечная отметка периода, в которой агрегированы события.</param>
        public StreamDataAggregateModel(DateTimeOffset timestamp) => Timestamp = timestamp;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataAggregateModel"/>.
        /// </summary>
        /// <param name="timestamp">Конечная отметка периода, в которой агрегированы события.</param>
        /// <param name="count">Кол-во событий за данный период.</param>
        public StreamDataAggregateModel(DateTimeOffset timestamp, ulong count)
        {
            Timestamp = timestamp;
            Count = count;
        }

        /// <summary>
        /// Конечная отметка периода, в которой агрегированы события.
        /// </summary>
        [ClickHouseColumn("slot")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Кол-во событий за данный период.
        /// </summary>
        [ClickHouseColumn("events_count")]
        public ulong Count { get; set; }
    }
}
