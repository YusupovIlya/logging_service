using System;
using System.Collections.Generic;
using System.Linq;
using Monq.Models.Abstractions.v2;

namespace Logging.Server.Models.StreamData.Api
{
    /// <summary>
    /// Модель представления результата фильтрации событий, которая включает в себя:
    /// 1) Агрегированные данные по кол-ву событий за период времени;
    /// 2) Список логов.
    /// </summary>
    public class StreamDataFilterResultViewModel
    {
        /// <summary>
        /// Список агрегированных данных по кол-ву событий за период времени.
        /// </summary>
        public IEnumerable<StreamDataAggregateViewModel> Aggregations { get; set; } = Array.Empty<StreamDataAggregateViewModel>();

        /// <summary>
        /// Список событий.
        /// </summary>
        public IEnumerable<StreamDataEventViewModel> Documents { get; set; } = Array.Empty<StreamDataEventViewModel>();

        /// <summary>
        /// Получить пустую модель, которая содержит пустые слоты агрегированных данных исходя из временных рамок и интервала.
        /// </summary>
        /// <param name="timestamp">Временные рамки, в которые входят события.</param>
        /// <param name="interval">Интервал (в сек.), по которому будут разбиты агрегированные данные по кол-ву событий за период времени.</param>
        public static StreamDataFilterResultViewModel Empty(DateRangePostViewModel timestamp, uint interval) =>
            new() { Aggregations = GetEmptyAggregations(timestamp, interval).ToList() };

        static IEnumerable<StreamDataAggregateViewModel> GetEmptyAggregations(DateRangePostViewModel timestamp, uint interval)
        {
            var slotsCount = (timestamp.End - timestamp.Start).TotalSeconds / interval;
            for (var i = 0; i <= slotsCount; i++)
                yield return new StreamDataAggregateViewModel(timestamp.Start.AddSeconds(interval * i));
        }
    }
}
