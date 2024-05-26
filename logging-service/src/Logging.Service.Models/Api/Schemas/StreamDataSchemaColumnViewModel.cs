using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Logging.Server.Models.StreamData.Api.Schemas
{
    /// <summary>
    /// Модель представления колонки схемы потоковых данных.
    /// </summary>
    public class StreamDataSchemaColumnViewModel
    {
        /// <summary>
        /// Название колонки схемы.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип колонки схемы.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StreamDataSchemaColumnType Type { get; }

        /// <summary>
        /// Тип элемента массива.
        /// </summary>
        /// <value>Unknown, если поле Type не является массивом.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public StreamDataSchemaColumnType ElementType { get; }

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Конструктор колонки схемы.
        /// </summary>
        /// <param name="name">Название колонки схемы.</param>
        /// <param name="type">Тип колонки схемы.</param>
        /// <param name="elementType">Тип элемента массива.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        public StreamDataSchemaColumnViewModel(string name, StreamDataSchemaColumnType type, StreamDataSchemaColumnType elementType = StreamDataSchemaColumnType.Unknown, string? defaultValue = null)
        {
            Name = name;
            Type = type;
            if (type != StreamDataSchemaColumnType.Array)
                ElementType = StreamDataSchemaColumnType.Unknown;
            else
                ElementType = elementType;
            DefaultValue = defaultValue;
        }
    }
}
