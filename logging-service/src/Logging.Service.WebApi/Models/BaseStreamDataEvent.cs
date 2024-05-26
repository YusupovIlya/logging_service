using Logging.Server.Service.StreamData.Infrastructure;
using System;
using System.Runtime.Serialization;
using Logging.Server.Service.StreamData.Configuration;

namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Базовая модель события потока данных.
    /// </summary>
    public class BaseStreamDataEvent
    {
        /// <summary>
        /// Идентификатор события.
        /// </summary>
        [ClickHouseColumn("_id")]
        public Guid Id { get; set; }

        ///// <summary>
        ///// Идентификатор пакета.
        ///// </summary>
        //[ClickHouseColumn("_rawId")]
        //public Guid RawId { get; set; }

        ///// <summary>
        ///// Идентификатор потока данных.
        ///// </summary>
        //[ClickHouseColumn("_stream.id")]
        //public long StreamId { get; set; }

        ///// <summary>
        ///// Имя потока данных.
        ///// </summary>
        //[ClickHouseColumn("_stream.name")]
        //public string StreamName { get; set; } = default!;

        ///// <summary>
        ///// Дата агрегирования события.
        ///// </summary>
        //[ClickHouseColumn("_aggregatedAt")]
        //public DateTimeOffset AggregatedAt { get; set; }

        /// <summary>
        /// Полная модель события потока данных в формате JSON.
        /// </summary>
        [ClickHouseColumn(AppConstants.SourceRawJson, Codec = "LZ4HC(5)")]
        public string RawJson { get; set; } = default!;

        /// <summary>
        /// Полная модель меток события в формате JSON.
        /// </summary>
        [ClickHouseColumn(AppConstants.LabelsRawJson, Codec = "LZ4HC(5)")]
        public string? LabelsRawJson { get; set; }

        /// <summary>
        /// Пользовательская отметка времени.
        /// </summary>
        [ClickHouseColumn("@timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Служебная дата.
        /// </summary>
        [IgnoreDataMember]
        [ClickHouseColumn("_date")]
        public DateTime Date => Timestamp.ToLocalTime().Date;

        ///// <summary>
        ///// Идентификатор пользовательского пространства.
        ///// </summary>
        //[ClickHouseColumn("_userspaceId")]
        //public long UserspaceId { get; set; }
    }
}
