using Logging.Server.Models.StreamData.Api.Schemas;
using System.Threading.Tasks;

namespace Logging.Server.Service.StreamData.Services
{
    /// <summary>
    /// Интерфейс репозитория схем потоковых данных.
    /// </summary>
    public interface IStreamDataSchemasRepository
    {
        /// <summary>
        /// Создать новую таблицу БД по указанной схеме.
        /// </summary>
        /// <param name="value">Модель</param>
        /// <returns><see cref="Task"/>, показывающий завершение операции.</returns>
        Task Create(StreamDataSchemaPostViewModel value);

        /// <summary>
        /// Получить схему таблицы БД по идентификатору потока.
        /// </summary>
        /// <param name="streamId">Идентификатор потока.</param>
        /// <returns>Схема потоковых данных (см. <see cref="StreamDataSchemaViewModel"/>).</returns>
        Task<StreamDataSchemaViewModel?> Get(long streamId);

        /// <summary>
        /// Проверить, существует ли таблица потоковых данных.
        /// </summary>
        /// <param name="streamId">Идентификатор потока.</param>
        /// <returns>Флаг существования таблицы.</returns>
        Task<bool> Exists(long streamId);

        /// <summary>
        /// Обновить схему потоковых данных.
        /// </summary>
        /// <param name="value">Модель для обновления схемы потоковых данных (см. <see cref="StreamDataSchemaPutViewModel"/>).</param>
        /// <returns><see cref="Task"/>, показывающий завершение операции.</returns>
        Task Update(StreamDataSchemaPutViewModel value);
    }
}
