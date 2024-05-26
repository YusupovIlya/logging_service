using System;

namespace Logging.Server.Service.StreamData.Infrastructure
{
    /// <summary>
    /// Атрибут для конфигурации таблицы в БД.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClickHouseTableAttribute : Attribute
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ClickHouseTableAttribute"/>.
        /// </summary>
        /// <param name="name">Наименование поля в БД.</param>
        public ClickHouseTableAttribute(string name) => Name = name;

        /// <summary>
        /// Наименование поля в БД.
        /// </summary>
        public string Name { get; }
    }
}
