using System;

namespace Logging.Server.Service.StreamData.Models
{
    public class Resource : IEquatable<Resource>
    {
        /// <summary>
        /// Идентификатор языка (RFC 4646) ресурса.
        /// </summary>
        public string? LanguageId { get; set; }

        /// <summary>
        /// Ключ ресурса (переводимая строка).
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Значение ключа ресурса на запрашиваемом языке.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Язык ресурса.
        /// </summary>
        public Language? Language { get; set; }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Resource? other)
            => !(other is null)
               && LanguageId == other.LanguageId
               && Key == other.Key
               && Value == other.Value;
    }

}
