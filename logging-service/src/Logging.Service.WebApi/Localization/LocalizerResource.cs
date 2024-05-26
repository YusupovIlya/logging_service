using Microsoft.Extensions.Localization;
using System.Linq;
using System.Reflection;

namespace Logging.Server.Service.StreamData.Localization
{
    /// <summary>
    /// Ресурс для локализации параметров атрибутов методов контроллера,
    /// через значения ErrorMessageResourceType и ErrorMessageResourceName.
    /// </summary>
    public class LocalizerResource
    {
        static LocalizerResource? _self;

        /// <summary>
        /// Конструктор ресурса для локализации параметров атрибутов методов контроллера.
        /// </summary>
        public LocalizerResource() => _self = this;

        /// <summary>
        /// Инициализировать текущую локализацию.
        /// </summary>
        public static void Init(IStringLocalizer localizer)
        {
            var properties = typeof(LocalizerResource).GetProperties(BindingFlags.Static | BindingFlags.Public).ToList();
            properties.ForEach(p =>
            {
                var key = FromPascalCaseToUnderscoreCase(p.Name);
                p.SetValue(_self, localizer[key] == null ? key : localizer[key].Value);
            });
        }

        public static string FromPascalCaseToUnderscoreCase(string str)
            => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString().ToLower()));
    }

}
