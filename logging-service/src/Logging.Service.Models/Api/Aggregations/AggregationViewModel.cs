using System;
using System.Collections.Generic;

namespace Logging.Server.Models.StreamData.Api.Aggregations
{
    /// <summary>
    /// Модель результата агрегации.
    /// </summary>
    public class AggregationViewModel
    {
        /// <summary>
        /// Создает новый экземпляр <see cref="AggregationViewModel"/>.
        /// </summary>
        public AggregationViewModel()
        { }

        /// <summary>
        /// Агрегационная функция.
        /// </summary>
        public string Function { get; set; } = default!;

        /// <summary>
        /// Поле, по которому проводилась агрегация.
        /// </summary>
        public string Column { get; set; } = default!;

        /// <summary>
        /// Результат агрегации.
        /// </summary>
        public IEnumerable<AggregationResultViewModel> Result { get; set; } = Array.Empty<AggregationResultViewModel>();
    }
}
