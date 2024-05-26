using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Logging.Server.StreamData.Validator.Extensions
{
    /// <summary>
    /// Расширения для работы со свойствами.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Получить имя JSON-свойства модели.
        /// </summary>
        /// <param name="prop">Свойство модели.</param>
        public static string TryGetJsonName(this PropertyInfo prop)
        {
            var jsonName = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute), true) as JsonPropertyAttribute;
            return jsonName?.PropertyName ?? prop.Name;
        }

        /// <summary>
        /// Получает имя JSON-свойства модели, указанного имени.
        /// </summary>
        public static string GetJsonProperty<T>(string propertyName)
        {
            var propertyInfos = typeof(T).GetProperties().Where(x => x.Name.Equals(propertyName));
            var prop = propertyInfos.FirstOrDefault();
            if (prop is null)
                throw new ArgumentNullException($"Property {propertyName} is not found.");
            return TryGetJsonName(prop);
        }
    }
}