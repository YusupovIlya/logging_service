using System;
using System.Text;

namespace Logging.Server.Service.StreamData.Helpers
{
    /// <summary>
    /// Хелпер для работы с хеш-функциями.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Получить SHA1 hash Для строки
        /// </summary>
        /// <param name="value">Исходное значение в строковом представлении.</param>
        /// <returns></returns>
        public static string GetShaHash(string value)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var buf = Encoding.UTF8.GetBytes(value);
            var hash = sha1.ComputeHash(buf, 0, buf.Length);
            var hashStr = BitConverter.ToString(hash).Replace("-", "");
            return hashStr;
        }
    }
}