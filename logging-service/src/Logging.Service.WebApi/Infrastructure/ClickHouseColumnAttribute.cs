using System;

namespace Logging.Server.Service.StreamData.Infrastructure
{
    /// <summary>
    /// Атрибут для конфигурации поля в БД.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ClickHouseColumnAttribute : Attribute
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ClickHouseColumnAttribute"/>.
        /// </summary>
        /// <param name="name">Наименование поля в БД.</param>
        public ClickHouseColumnAttribute(string name) => Name = name;

        /// <summary>
        /// Наименование поля в БД.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Тип поля.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Кодек.
        /// </summary>
        public string? Codec { get; set; }
    }
}
