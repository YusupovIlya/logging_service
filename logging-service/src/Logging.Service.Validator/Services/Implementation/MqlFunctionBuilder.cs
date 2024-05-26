using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.Models.StreamData.Extensions;
using Logging.Server.StreamData.Validator.Extensions;
using Logging.Server.StreamData.Validator.Models;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Класс преобразования функций в запрос кликхауса.
    /// </summary>
    public class MqlFunctionBuilder
    {
        static readonly AggregationType[] _nonScalarAggregation =
        {
            AggregationType.Top,
            AggregationType.Mode,
            AggregationType.Count,
            AggregationType.Uniq
        };

        /// <summary>
        /// Построить запрос для получения результата исполнения функции.
        /// </summary>
        /// <param name="fieldPath">Путь до поля, для которого вызывается функция.</param>
        /// <param name="fieldName">Название поля.</param>
        /// <param name="type">Тип функции.</param>
        public string? BuildFunction(string fieldPath, string fieldName, FunctionType type)
        {
            if (type == FunctionType.Exists)
                return ServiceFields.Any(val => val.Name.Equals(fieldPath, StringComparison.InvariantCultureIgnoreCase))
                    ? TrueCondition
                    : $"visitParamHas({fieldPath}, '{fieldName}')";

            return null;
        }

        /// <summary>
        /// Проверка является ли значение функцией.
        /// </summary>
        public bool TryParseFunction(string value, out FunctionType functionType) =>
            TryParse(value, _patterns, out functionType);

        /// <summary>
        /// Проверка является ли значение агрегатной функцией.
        /// </summary>
        public bool TryParseAggregationFunction(string value, out AggregationType functionType) =>
            TryParse(value, _aggregationPatterns, out functionType);

        static bool TryParse<T>(string value, IEnumerable<(T, string)> patterns, out T? functionType)
        {
            foreach (var (type, pattern) in patterns)
                if (Regex.IsMatch(value, pattern))
                {
                    functionType = type;
                    return true;
                }

            functionType = default;
            return false;
        }

        readonly IEnumerable<(AggregationType Type, string Pattern)> _aggregationPatterns = new[]
        {
            (AggregationType.Top, @"^top\(\d*\)$"),
            (AggregationType.Avg, @"^avg\(\)$"),
            (AggregationType.Max, @"^max\(\)$"),
            (AggregationType.Min, @"^min\(\)$"),
            (AggregationType.Median, @"^median\(\)$"),
            (AggregationType.Mode, @"^mode\(\)$"),
            (AggregationType.Uniq, @"^uniq\(\)$"),
            (AggregationType.Count, @"^count\(.+\)$")
        };

        readonly IEnumerable<(FunctionType Type, string Pattern)> _patterns = new[]
        {
            (FunctionType.Exists, @"^exists\(\)$")
        };

        /// <summary>
        /// Получить запрос для исполнения агрегатной функции.
        /// </summary>
        public AggregationQuery BuildAggregationFunction(string function, string column, AggregationType type)
        {
            AggregationQuery result = new() { Type = type, Column = column};
            if (type == AggregationType.Top)
            {
                var match = GetParameter(function);
                if (int.TryParse(match, out var count) && count > 0)
                    result.Count = count;
            }
            else if (type == AggregationType.Count)
            {
                (result.Value, result.ValueType) = GetCountQuery(column, GetParameter(function));
                result.Count = 1;
            }

            if (_nonScalarAggregation.Contains(result.Type))
                result.Value = GetPostCondition(result);
            return result;
        }

        static string GetPostCondition(AggregationQuery query)
        {
            const string orderBy = "ORDER BY Value DESC";
            string[] postCondition = query.Type switch
            {
                AggregationType.Count => new[]
                {
                    query.Value!,
                    orderBy
                },
                AggregationType.Top => new[] { orderBy, $"LIMIT {query.Count}" },
                _ => new[] { orderBy }
            };
            return string.Join(Space, postCondition);
        }

        /// <summary>
        /// Получить значения параметра функции.
        /// </summary>
        /// <param name="value">Функция.</param>
        public string GetParameter(string value) =>
            Regex.Match(value, @"(?<=\()\w+(?=\))").ToString();

        static (string, StreamDataSchemaColumnType) GetCountQuery(string field, string value)
        {
            if (Guid.TryParse(value, out var guid))
                return ($"HAVING {field.AddFieldQuotes()} = '{guid}'", StreamDataSchemaColumnType.Guid);

            if (double.TryParse(value, out var number))
                return ($"HAVING {field.AddFieldQuotes()} = {number}", StreamDataSchemaColumnType.Double);

            if (DateTimeOffset.TryParse(value, out var date))
                return ($"HAVING {field.AddFieldQuotes()} = {DateTimeExtensions.ToDateTime64(date)}", StreamDataSchemaColumnType.Date);

            return ("HAVING upperUTF8(Key) LIKE CONCAT('%', {0}, '%')", StreamDataSchemaColumnType.String);
        }
    }
}
