using System;
using System.Collections.Generic;

namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Язык ресурсов локализации.
    /// </summary>
    public class Language : IEquatable<Language>
    {
        /// <summary>
        /// Идентификатор языка (RFC 4646).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Справочник предметной области сервиса локализации.
        /// </summary>
        public List<Resource> Resources { get; set; } = new();

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Language? other) => !(other is null) && Id == other.Id;
    }
}
