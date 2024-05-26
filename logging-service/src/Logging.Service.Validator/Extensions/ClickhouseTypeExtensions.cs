using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.Models.StreamData.Extensions;
using Logging.Server.StreamData.Validator.Configuration;
using Newtonsoft.Json.Linq;

namespace Logging.Server.StreamData.Validator.Extensions
{
    /// <summary>
    /// Набор методов расширений для работы с кликхаусовскими типами.
    /// </summary>
    public static class ClickhouseTypeExtensions
    {
        /// <summary>
        /// Получить название тип поля.
        /// </summary>
        /// <param name="type">Тип поля.</param>
        /// <param name="elementType">Тип элемента для массивов.</param>
        /// <returns>Название типа поля.</returns>
        public static string GetClickHouseType(this StreamDataSchemaColumnType type, StreamDataSchemaColumnType elementType = StreamDataSchemaColumnType.Unknown) =>
            type switch
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

        /// <summary>
        /// Получить строку преобразования в передаваемый тип.
        /// </summary>
        /// <param name="type">Тип поля, в который необходимо преобразовать.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Строка преобразования.</returns>
        public static string CastArrayElement(this StreamDataSchemaColumnType type, in string value) =>
            type switch
            {
                StreamDataSchemaColumnType.Integer => $"toInt64({value})",
                StreamDataSchemaColumnType.Double => $"toFloat64({value})",
                StreamDataSchemaColumnType.String => value,
                StreamDataSchemaColumnType.Date => $"toDateTime64({value}, 3)",
                StreamDataSchemaColumnType.Bool => $"toUInt8({value})",
                StreamDataSchemaColumnType.Guid => $"toUUID({value})",
                _ => string.Empty
            };

        /// <summary>
        /// Получить строку сравнения значений.
        /// </summary>
        /// <param name="valueType">Тип поля.</param>
        /// <param name="columnName">Название поля.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Строка сравнения.</returns>
        public static string GetEqualsStatement(this StreamDataSchemaColumnType valueType, in string columnName, in string value) =>
           valueType switch
           {
               StreamDataSchemaColumnType.Unknown => $"{columnName} = '{value}'",
               StreamDataSchemaColumnType.Integer => $"{columnName} = toInt64({value})",
               StreamDataSchemaColumnType.Double => $"{columnName} = toFloat64({value})",
               StreamDataSchemaColumnType.String => $"like(upperUTF8({columnName}), {value})",
               StreamDataSchemaColumnType.Date when DateTimeOffset.TryParse(value, out var date) => $"{columnName} = {DateTimeExtensions.ToDateTime64(date)}",
               StreamDataSchemaColumnType.Bool => $"{columnName} = {(bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase) ? "1" : "0")}",
               StreamDataSchemaColumnType.Guid => $"{columnName} = '{value}'",
               _ => string.Empty
           };

        /// <summary>
        /// Проверить является ли часть значения валидной.
        /// </summary>
        public static bool IsValid(this StreamDataSchemaColumnType valueType, IEnumerable<string> value) =>
            valueType switch
            {
                StreamDataSchemaColumnType.Integer => value.All(val => Regex.IsMatch(val, @"^[\-\d]+$")),
                StreamDataSchemaColumnType.Double => value.All(val => Regex.IsMatch(val, @"^[\-\d,]+$")),
                StreamDataSchemaColumnType.Bool => value.All(val => Regex.IsMatch(val, @"^[truefals]+$")),
                StreamDataSchemaColumnType.Guid => value.All(val => Regex.IsMatch(val, @"^[A-Za-z\-\d]+$")),
                _ => true
            };

        /// <summary>
        /// Получить строку преобразования из json'а для определённого типа.
        /// </summary>
        /// <param name="type">Тип поля.</param>
        /// <param name="path">Путь к названию поля.</param>
        /// <param name="value">Значение.</param>
        /// <returns>Строка преобразования из json'а.</returns>
        public static string GetVisitParamExtractFunc(this StreamDataSchemaColumnType type, in string path, in string value) =>
            type switch
            {
                StreamDataSchemaColumnType.Unknown => $"visitParamExtractRaw({path}, '{value}')",
                StreamDataSchemaColumnType.Integer => $"visitParamExtractInt({path}, '{value}')",
                StreamDataSchemaColumnType.Double => $"visitParamExtractFloat({path}, '{value}')",
                StreamDataSchemaColumnType.String => $"visitParamExtractString({path}, '{value}')",
                StreamDataSchemaColumnType.Date => $"visitParamExtractString({path}, '{value}')",
                StreamDataSchemaColumnType.Bool => $"JSONExtractBool({path}, '{value}')",
                StreamDataSchemaColumnType.Guid => $"toUUID(visitParamExtractString({path}, '{value}'))",
                StreamDataSchemaColumnType.Array => $"JSONExtractArrayRaw({path}, '{value}')",
                _ => string.Empty
            };

        /// <summary>
        /// Получить кликхаусовский тип значения.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <returns>Тип значения.</returns>
        public static StreamDataSchemaColumnType GetSearchType(this string value)
        {
            if (bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase)
                || bool.FalseString.Equals(value, StringComparison.OrdinalIgnoreCase))
                return StreamDataSchemaColumnType.Bool;

            if (long.TryParse(value, out _))
                return StreamDataSchemaColumnType.Integer;

            if (double.TryParse(value, NumberStyles.Float, new NumberFormatInfo(), out _))
                return StreamDataSchemaColumnType.Double;

            if (Guid.TryParse(value, out _))
                return StreamDataSchemaColumnType.Guid;

            if (DateTimeOffset.TryParse(value, out _))
                return StreamDataSchemaColumnType.Date;

            return StreamDataSchemaColumnType.Unknown;
        }

        /// <summary>
        /// Получить тип знания поисковой фразы.
        /// </summary>
        /// <param name="value">Значение.</param>
        public static JTokenType GetJTokenSearchType(this string value) =>
            value.GetSearchType() switch
            {
                StreamDataSchemaColumnType.Bool => JTokenType.Boolean,
                StreamDataSchemaColumnType.Integer => JTokenType.Integer,
                StreamDataSchemaColumnType.Double => JTokenType.Float,
                StreamDataSchemaColumnType.Guid => JTokenType.Guid,
                StreamDataSchemaColumnType.Date => JTokenType.Date,
                _ => JTokenType.Raw
            };
    }
}
