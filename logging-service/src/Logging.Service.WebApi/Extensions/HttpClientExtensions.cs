using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
//using Monq.Core.Authorization.Exceptions;

namespace Logging.Server.Service.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения для работы с http-запросами.
    /// </summary>
    //public static class HttpClientExtensions
    //{
    //    /// <summary>
    //    /// Получить идентификатор пользовательского пространства из заголовков HTTP запроса.
    //    /// </summary>
    //    /// <param name="contextAccessor">Аксессор для получения данных по текущему запросу.</param>
    //    public static long GetUserspaceId(this IHttpContextAccessor contextAccessor)
    //    {
    //        var userspaceId = contextAccessor?.HttpContext?.Request?.Userspace();
    //        if (userspaceId is null || userspaceId.Value == default)
    //            throw new UserspaceNotFoundException("Невозможно получить пространство пользователя.");

    //        return userspaceId.Value;
    //    }
    //}
}