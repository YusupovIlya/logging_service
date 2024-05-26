using System.Collections.Generic;
using System.Threading.Tasks;
//using Logging.Server.Models.Streams.Api;

namespace Logging.Server.Service.StreamData.HttpServices
{
    /// <summary>
    /// Интерфейс взаимодействия с потоками данных через cl-streams-service.
    /// </summary>
    /// <remarks>
    /// https://gitlab.monq.ru/smon.monq.ru/cl/cl-streams-service
    /// </remarks>
    public interface IStreamsHttpService
    {
        /// <summary>
        /// Получить список всех доступных потоков данных по фильтру.
        /// </summary>
        /// <param name="value">Фильтр потоков данных (см. <see cref="StreamFilterViewModel"/>).</param>
        /// <returns>Список моделей потоков данных без конфигурации (см. <see cref="IEnumerable{StreamListViewModel}"/>).</returns>
        //Task<IEnumerable<StreamListViewModel>?> Filter(StreamFilterViewModel value);
    }
}