using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Logging.Server.Service.StreamData.Localization
{
    /// <summary>
    /// Middleware для работы с данными локализации.
    /// </summary>
    public class LocalizerMiddleware
    {
        readonly RequestDelegate _next;
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
        public LocalizerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IStringLocalizer localizer)
        {
            LocalizerResource.Init(localizer);
            await _next.Invoke(context);
        }
    }

    public static class LocalizerExtensions
    {
        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder builder) =>
            builder.UseMiddleware<LocalizerMiddleware>();
    }

}
