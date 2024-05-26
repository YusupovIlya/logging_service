using System.Collections.Generic;
using System.Threading.Tasks;
using Logging.Server.Models.StreamData.Api;
using Logging.Server.Service.StreamData.Models;

namespace Logging.Server.Service.StreamData.Services
{
    /// <summary>
    /// Интерфейс репозитория алиасов.
    /// </summary>
    public interface IAliasRepository
    {
        /// <summary>
        /// Создать алиас.
        /// </summary>
        /// <param name="aliasViewModel">Создаваемый алиас.</param>
        Task<AliasViewModel?> Upsert(AliasViewModel aliasViewModel);

        /// <summary>
        /// Проверить существование алиаса.
        /// </summary>
        /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="value">Значение алиаса.</param>
        Task<bool> Exists(long userspaceId, string fieldName, string value);

        /// <summary>
        /// Получить алиасы для поля.
        /// </summary>
        /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
        /// <param name="fieldName">Название поля.</param>
        Task<IEnumerable<AliasViewModel>> Get(long userspaceId, string fieldName);

        /// <summary>
        /// Получить все алиасы по умолчанию.
        /// </summary>
        /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
        Task<IEnumerable<AliasViewModel>> GetDefault(long userspaceId);

        /// <summary>
        /// Получить все алиасы пространства.
        /// </summary>
        /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
        Task<IEnumerable<AliasViewModel>> GetAll(long userspaceId);
    }
}
