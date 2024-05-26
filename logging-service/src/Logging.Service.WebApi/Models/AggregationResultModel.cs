namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Модель агрегированных данных.
    /// </summary>
    public class AggregationResultModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AggregationResultModel"/>.
        /// </summary>
        public AggregationResultModel()
        { }

        /// <summary>
        /// Поле, по которому проводилась агрегация.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Значение результат агрегации.
        /// </summary>
        public double Value { get; set; }
    }
}
