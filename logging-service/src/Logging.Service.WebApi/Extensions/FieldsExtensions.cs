using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Logging.Server.Service.StreamData.Models;

namespace Logging.Server.Service.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения для работы с полями.
    /// </summary>
    public static class FieldsExtensions
    {
        /// <summary>
        /// Проверка, является ли поле системным.
        /// </summary>
        public static bool IsServiceField(this string field)
        {
            return typeof(BaseStreamDataEvent)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<IgnoreDataMemberAttribute>() is null)
                .Any(val => val.TryGetClickHouseName() == field.AddQuotes());
        }

        /// <summary>
        /// Добавить кавычки для значения.
        /// </summary>
        public static string AddQuotes(this string value) =>
            string.Concat("`", value, "`");
    }
}
