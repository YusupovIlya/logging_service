namespace Logging.Server.Models.StreamData.Api.StreamData
{
    /// <summary>
    /// Модель представления количества событий потока данных.
    /// </summary>
    public class StreamDataEventsCountViewModel
    {
        /// <summary>
        /// Количество событий потока данных.
        /// </summary>
        public ulong Count { get; set; }
    }
}
