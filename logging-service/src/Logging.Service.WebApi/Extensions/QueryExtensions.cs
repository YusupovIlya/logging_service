using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.Infrastructure;
using Monq.Models.Abstractions.v2;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Logging.Server.Service.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Встроить PREWHERE в запрос.
        /// </summary>
        /// <param name="sb">Строитель запроса.</param>
        /// <param name="preWhereExpressions">Выражения для фильтрации.</param>
        public static StringBuilder AppendPreWhereExpression(this StringBuilder sb, string[] preWhereExpressions)
        {
            if (preWhereExpressions is null || preWhereExpressions.Length == 0)
                return sb;

            sb.Append("PREWHERE ");
            for (var i = 0; i < preWhereExpressions.Length; i++)
            {
                sb.Append('(').Append(preWhereExpressions[i]).Append(')');
                if (i + 1 < preWhereExpressions.Length)
                    sb.Append(Environment.NewLine).Append(" AND ");
            }

            return sb;
        }

        /// <summary>
        /// Получить SQL-выражения для фильтрации по определенному полю с датой.
        /// </summary>
        /// <param name="date">Принимаемая модель конкретной даты, либо диапазона, для выполнения фильтрации по определенному полю.</param>
        /// <param name="fieldName">Поле с датой.</param>
        /// <param name="format">Метод преобразования по формату.</param>
        public static string GetSqlExpression(this DateRangePostViewModel? date, string? fieldName, Func<DateTimeOffset, string> format)
        {
            if (date is null)
                throw new ArgumentNullException(nameof(date));
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fieldName));
            if (format is null)
                throw new ArgumentNullException(nameof(format));

            if (date.Start == date.End)
                return $"{fieldName} = {format(date.Start)}";

            return date.Start < date.End
                ? $"{fieldName} BETWEEN {format(date.Start)} AND {format(date.End)}"
                : $"{fieldName} BETWEEN {format(date.End)} AND {format(date.Start)}";
        }

        /// <summary>
        /// Получить имя свойства модели в ClickHouse.
        /// </summary>
        /// <param name="prop">Свойство модели.</param>
        public static string TryGetClickHouseName(this PropertyInfo prop)
        {
            var clickHouseColumn = Attribute.GetCustomAttribute(prop, typeof(ClickHouseColumnAttribute), true) as ClickHouseColumnAttribute;
            return $"`{clickHouseColumn?.Name ?? prop.Name}`";
        }

        /// <summary>
        /// Преобразовать идентификатор потока в название таблицы.
        /// </summary>
        /// <param name="streamId">Идентификатор потока.</param>
        /// <returns>Название таблицы.</returns>
        public static string ToTableName(this long streamId) =>
            $"{AppConstants.ClickHouse.StreamDataTablePrefix}{streamId}";

        /// <summary>
        /// Получить команду создания колонки в ClickHouse, соответствующей свойству модели.
        /// </summary>
        /// <param name="prop">Свойство модели.</param>
        public static string GetClickHouseColumnCommand(this PropertyInfo prop)
        {
            var clickHouseColumn = Attribute.GetCustomAttribute(prop, typeof(ClickHouseColumnAttribute), true) as ClickHouseColumnAttribute;
            var command = $"`{clickHouseColumn?.Name ?? prop.Name}` {prop.PropertyType.GetClickHouseType()}";
            if (clickHouseColumn is null)
                return command;
            if (clickHouseColumn.DefaultValue is not null)
                command += $" DEFAULT {clickHouseColumn.DefaultValue}";
            if (!string.IsNullOrWhiteSpace(clickHouseColumn.Codec))
                command += $" CODEC({clickHouseColumn.Codec})";
            return command;
        }

        ///Получить команду создания колонки в ClickHouse, соответствующей модели колонки схемы потоковых данных.
        public static string GetClickHouseColumnCommand(this StreamDataSchemaColumnViewModel column)
        {
            var command = $"`{column.Name}` {column.Type.GetClickHouseType(column.ElementType)}";
            if (column.DefaultValue is null)
                return command;
            command += $" DEFAULT {column.DefaultValue}";
            return command;
        }

        // TODO: отдельно регистрировать конвертеры типов при запуске сервиса.
        /// <summary>
        /// Получить тип данных в ClickHouse.
        /// </summary>
        /// <param name="type">Тип колонки схемы.</param>
        /// <param name="elementType">Тип элемента колонки схемы.</param>
        public static string GetClickHouseType(this StreamDataSchemaColumnType type, StreamDataSchemaColumnType elementType = StreamDataSchemaColumnType.Unknown)
        {
            var clickHouseType = type switch
            {
                StreamDataSchemaColumnType.Integer => "Int64",
                StreamDataSchemaColumnType.Double => "Float64",
                StreamDataSchemaColumnType.String => "String",
                StreamDataSchemaColumnType.Date => $"DateTime64({AppConstants.ClickHouse.DateTimePrecision})",
                StreamDataSchemaColumnType.Bool => "UInt8",
                StreamDataSchemaColumnType.Array => $"Array({elementType.GetClickHouseType()})",
                StreamDataSchemaColumnType.Guid => "UUID",
                _ => "String"
            };
            return clickHouseType;
        }

        /// <summary>
        /// Получить тип колонки схемы потоковых данных.
        /// </summary>
        /// <param name="clickhouseType">Тип данных в ClickHouse.</param>
        public static (StreamDataSchemaColumnType Type, StreamDataSchemaColumnType ElementType) GetColumnType(this string clickhouseType)
        {
            var elementType = StreamDataSchemaColumnType.Unknown;
            if (clickhouseType.StartsWith("Array"))
            {
                var elementClickHouseType = clickhouseType.Replace("Array(", string.Empty).Replace(")", string.Empty);
                elementType = elementClickHouseType.GetColumnType().Type;
                return (StreamDataSchemaColumnType.Array, elementType);
            }

            if (clickhouseType.StartsWith("DateTime64"))
                return (StreamDataSchemaColumnType.Date, StreamDataSchemaColumnType.Unknown);

            var type = clickhouseType switch
            {
                "Int64" => StreamDataSchemaColumnType.Integer,
                "Float64" => StreamDataSchemaColumnType.Double,
                "String" => StreamDataSchemaColumnType.String,
                "DateTime" => StreamDataSchemaColumnType.Date,
                "UInt8" => StreamDataSchemaColumnType.Bool,
                "UUID" => StreamDataSchemaColumnType.Guid,
                _ => StreamDataSchemaColumnType.Unknown
            };
            return (type, elementType);
        }

        /// <summary>
        /// Получить тип данных в ClickHouse.
        /// </summary>
        /// <param name="type">Тип данных в .NET.</param>
        public static string GetClickHouseType(this Type type)
        {
            if (type == typeof(string))
                return "String";

            if (type.IsEnumerable())
                return $"Array({type.GenericTypeArguments[0].GetClickHouseType()})";

            if (type.IsEnum)
            {
                var values = type.GetFields().Where(x => !x.Name.Equals("value__")).Select(x => $"'{x.Name}' = {x.GetRawConstantValue()}");
                return $"Enum8({string.Join(", ", values)})";
            }

            if (type == typeof(bool))
                return "UInt8";

            if (type == typeof(byte))
                return "UInt8";

            if (type == typeof(ushort))
                return "UInt16";

            if (type == typeof(uint))
                return "UInt32";

            if (type == typeof(ulong))
                return "UInt64";

            if (type == typeof(sbyte))
                return "Int8";

            if (type == typeof(short))
                return "Int16";

            if (type == typeof(int))
                return "Int32";

            if (type == typeof(long))
                return "Int64";

            if (type == typeof(float))
                return "Float32";

            if (type == typeof(double))
                return "Float64";

            if (type == typeof(Guid))
                return "UUID";

            if (type == typeof(DateTime))
                return "Date";

            if (type == typeof(DateTimeOffset))
                return $"DateTime64({AppConstants.ClickHouse.DateTimePrecision})";

            throw new Exception("Error converting to clickHouse type.");
        }

        /// <summary>
        /// Проверить, реализует ли тип интерфейс IEnumerable.
        /// </summary>
        /// <param name="type">Тип данных в .NET.</param>
        public static bool IsEnumerable(this Type type) =>
            type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable));
    }
}
