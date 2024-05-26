using Logging.Server.Service.StreamData.Models;
using Monq.Models.Abstractions.v2;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging.Server.StreamData.Validator.Models;

namespace Logging.Server.Service.StreamData.Services
{
    /// <summary>
    /// Репозиторий для чтения потоковых данных.
    /// </summary>
    public interface IStreamDataRepository
    {
        /// <summary>
        /// Получить список всех таблиц с потоками данных.
        /// </summary>
        /// <param name="streamIds">Список идентификаторов потоков данных.</param>
        Task<IEnumerable<long>> GetExistingStreamIds(IEnumerable<long> streamIds);

        /// <summary>
        /// Получить результат агрегации.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="query">Запрос.</param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события.</param>
        Task<IEnumerable<AggregationResultModel>> GetAggregation(
            IEnumerable<Schema> schemas,
            AggregationQuery query,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp);

        /// <summary>
        /// Получить события по фильтру в виде csv файла.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="checkedFields">Выбранные поля.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        Task<byte[]> GetCsvFile(
            IEnumerable<Schema> schemas,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp,
            IEnumerable<string> checkedFields,
            string timestampSortDir = "desc");

        /// <summary>
        /// Получить события по фильтру.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="limit">Макс. кол-во событий, которые будут получены.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        Task<IEnumerable<BaseStreamDataEvent>> GetEventsByFilter(
            IEnumerable<Schema> schemas,
            IDictionary<string, string> parameters,
            DateRangePostViewModel timestamp,
            int limit = 500,
            string timestampSortDir = "desc");

        /// <summary>
        /// Получить агрегированные данные по событиям, которые удовлетворяют фильтру.
        /// </summary>
        /// <param name="schemas">
        /// Список, каждый элемент которого состоит из наименования таблицы, из которой будет производиться выборка,
        /// и дополнительных SQL выражений, которые являются специфичными для каждой схемы.
        /// </param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <param name="timestamp">Дата регистрации события из фильтра.</param>
        /// <param name="interval">Интервал (в сек.), по которому будут разбиты агрегированные данные по кол-ву событий за период времени.</param>
        Task<IEnumerable<StreamDataAggregateModel>> GetAggregationsByFilter(IEnumerable<Schema> schemas, IDictionary<string, string> parameters, DateRangePostViewModel timestamp, uint interval);

        /// <summary>
        /// Получить количество событий потока данных.
        /// </summary>
        /// <param name="streamId">Идентификатор потока данных.</param>
        /// <returns>Количество событий.</returns>
        Task<ulong> GetCount(long streamId);
    }
}
