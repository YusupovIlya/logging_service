using Logging.Server.Models.StreamData.Api;

namespace Logging.Server.StreamData.Validator
{
    /// <summary>
    /// Валидатор первичного события.
    /// </summary>
    public interface IEventValidator
    {
        /// <summary>
        /// Является ли событие валидным по данному пользовательскому фильтру.
        /// </summary>
        /// <param name="event">Первичное событие.</param>
        /// <param name="filter">Фильтр событий потока данных.</param>
        bool IsValid(StreamDataEventViewModel @event, StreamDataFilterViewModel filter);
    }
}