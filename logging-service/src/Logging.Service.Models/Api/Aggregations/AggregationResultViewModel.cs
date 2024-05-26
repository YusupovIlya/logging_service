namespace Logging.Server.Models.StreamData.Api.Aggregations
{
    /// <summary>
    /// Модель результата агрегации.
    /// </summary>
    public class AggregationResultViewModel
    {
        /// <summary>
        /// Значение результат агрегации, для нескалярных — значение поля;
        /// для скалярных — null.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Результат агрегации, для нескалярных — сколько раз значение встретилось;
        /// для скалярных — сам результат.
        /// </summary>
        public double Value { get; set; }
    }
}
