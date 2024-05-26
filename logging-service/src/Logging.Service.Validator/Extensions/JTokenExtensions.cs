using Newtonsoft.Json.Linq;

namespace Logging.Server.StreamData.Validator
{
    /// <summary>
    /// Методы расширения для <see cref="JToken"/>.
    /// </summary>
    public static class JTokenExtensions
    {
        /// <summary>
        /// Преобразовать свойство в объект.
        /// </summary>
        public static JObject CastToJObjectUnsafe(this JToken token) => (JObject) token;
    }
}