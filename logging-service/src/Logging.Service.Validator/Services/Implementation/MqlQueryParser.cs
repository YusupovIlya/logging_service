using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.StreamData.Validator.Extensions;
using Logging.Server.StreamData.Validator.Models;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Класс для преобразования запроса в Clickhouse запрос.
    /// </summary>
    public class MqlQueryParser
    {
        int _number;
        readonly Dictionary<string, string> _parameters = new ();
        readonly MqlFunctionBuilder _functionBuilder;

        static readonly AggregationType[] _nonScalarAggregation =
        {
            AggregationType.Top,
            AggregationType.Mode,
            AggregationType.Count,
            AggregationType.Uniq
        };

        static readonly StreamDataSchemaColumnType[] _numericTypes =
        {
            StreamDataSchemaColumnType.Integer,
            StreamDataSchemaColumnType.Double
        };

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MqlQueryParser"/>.
        /// </summary>
        /// <param name="functionBuilder">Строитель функций.</param>
        public MqlQueryParser(MqlFunctionBuilder functionBuilder) => _functionBuilder = functionBuilder;

        /// <summary>
        /// Параметры запроса.
        /// </summary>
        public Dictionary<string, string> Parameters => _parameters;

        /// <summary>
        /// Преобразовать запрос в ClickHouse запрос.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="columns">Колонки.</param>
        /// <returns>Строка запроса.</returns>
        public string ToSqlQuery(string query, IEnumerable<StreamDataSchemaColumnViewModel> columns)
        {
            if (!MqlValidator.IsQueryValid(query))
                return FalseCondition;

            var splitter = new MqlQuerySplitter();
            var (newQuery, terms) = splitter.SplitQuery(query);
            var parts = terms
                .Select(term => term.Type switch
                {
                    TermType.FullSearchTerm => SearchAllFields(term.Value, columns),
                    TermType.CommonTerm => BuildTerm(term.Field, term.Value, columns),
                    TermType.EscapedTerm => BuildTermQuery(columns, term.Field, term.Value) ?? FalseCondition,
                    _ => FalseCondition
                })
                .Cast<object>()
                .ToArray();

            return string.Format(newQuery, parts);
        }

        string SearchAllFields(string value, IEnumerable<StreamDataSchemaColumnViewModel> columns)
        {
            var queries = ServiceFields
                .Select(val => CompareFieldWithValue(value, val.Name.AddFieldQuotes(), val.Type, val.ElementType))
                .Concat(columns.Select(val =>
                {
                    var json = GetParentJsonField(val.Name);
                    var query = BuildQueryToAccessNestedJson(json, val.Name);
                    if (!query.HasValue)
                        return string.Empty;
                    var path = val.Type.GetVisitParamExtractFunc(query.Value.Path, query.Value.Field);
                    return CompareFieldWithValue(value, path, val.Type, val.ElementType);
                }))
                .Where(val => !string.IsNullOrWhiteSpace(val))
                .ToList();
            return string.Join(Environment.NewLine + "\t OR ", queries);
        }

        string BuildTerm(string field, string value, IEnumerable<StreamDataSchemaColumnViewModel> columns)
        {
            if (_functionBuilder.TryParseFunction(value, out var functionType))
                return BuildFunctionQuery(columns, field, functionType) ?? FalseCondition;

            if (value.Any(val => val is LeftBrace or RightBrace))
                return FalseCondition;

            return BuildTermQuery(columns, field, value) ?? FalseCondition;
        }

        string? ReplaceWildcardSymbols(string query, StreamDataSchemaColumnType type)
        {
            var wildcardReplacer = new MqlWildcardReplacer();
            var (result, isWildcardBefore) = wildcardReplacer.ReplaceWildcardSymbols(query);
            var count = result.Count();

            if (count == 1)
                return PrepareValue(result.First(), isWildcardBefore);

            // Обычные строки и содержащие wildcard символы чередуются.
            Func<int, bool> isWildcard = isWildcardBefore == count.IsEven()
                ? x => !x.IsEven()
                : x => x.IsEven();

            if (!type.IsValid(result.Where((_, j) => !isWildcard(j)).ToList()))
                return null;

            var stringBuilder = new StringBuilder("CONCAT(");
            stringBuilder
                .AppendJoin(", ", result.Select((val, j) => PrepareValue(val, isWildcard(j))).ToList())
                .Append(RightBrace);

            return stringBuilder.ToString();
        }

        string PrepareValue(string value, bool condition)
        {
            if (condition)
                return string.Concat('\'', value, '\'')
                    .Replace('*', '%')
                    .Replace('?', '_');

            var upperValue = value.ToUpperInvariant();
            if (_parameters.TryGetValue(upperValue, out var name))
                return name.ToReplacement();

            ++_number;
            name = $"value{_number}";
            _parameters.Add(upperValue, name);

            return name.ToReplacement();
        }

        string? BuildTermQuery(IEnumerable<StreamDataSchemaColumnViewModel> columns, string fieldName, string value)
        {
            var serviceColumn = ServiceFields.GetByName(fieldName);
            if (serviceColumn is not null)
                return CompareFieldWithValue(
                    value,
                    serviceColumn.Name.AddFieldQuotes(),
                    serviceColumn.Type,
                    serviceColumn.ElementType);

            var column = columns.GetByName(fieldName);
            if (column is null)
                return null;

            var json = GetParentJsonField(column.Name);
            var query = BuildQueryToAccessNestedJson(json, column.Name);
            if (query is null)
                return null;

            var path = column.Type.GetVisitParamExtractFunc(query.Value.Path, query.Value.Field);
            return CompareFieldWithValue(value, path, column.Type, column.ElementType);
        }

        string? BuildFunctionQuery(IEnumerable<StreamDataSchemaColumnViewModel> columns, string field, FunctionType type)
        {
            var serviceColumn = ServiceFields.GetByName(field);
            if (serviceColumn is not null)
                return _functionBuilder.BuildFunction(serviceColumn.Name, serviceColumn.Name, type);

            var column = columns.GetByName(field);
            if (column is null)
                return null;

            var json = GetParentJsonField(column.Name);
            var query = BuildQueryToAccessNestedJson(json, column.Name);
            return query.HasValue
                ? _functionBuilder.BuildFunction(query.Value.Path, query.Value.Field, type)
                : null;
        }

        static string RemovePrefix(string field)
        {
            if (field.StartsWith(LabelsPrefix, StringComparison.OrdinalIgnoreCase))
                return field[LabelsPrefix.Length..];

            if (field.StartsWith(SourcePrefix, StringComparison.OrdinalIgnoreCase))
                return field[SourcePrefix.Length..];

            return field;
        }

        /// <summary>
        /// Построить агрегатную функцию.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="columns">Все поля.</param>
        public AggregationQuery? BuildAggregationFunctionQuery(string query, IEnumerable<StreamDataSchemaColumnViewModel> columns)
        {
            if (string.IsNullOrEmpty(query))
                return null;

            var parts = query.Split(Colon, 2);
            if (parts.Length != 2 || parts.Any(string.IsNullOrEmpty))
                return null;

            var field = parts[0];
            var function = parts[1];
            if (!MqlValidator.IsFieldValid(field))
                return null;

            if (!_functionBuilder.TryParseAggregationFunction(function, out var type))
                return null;

            var column = ServiceFields.Concat(columns).GetByName(field);
            if (column is null)
                return null;

            var result = _functionBuilder.BuildAggregationFunction(function, column.Name, type);
            if (!_numericTypes.Contains(column.Type) && !_nonScalarAggregation.Contains(result.Type))
                return null;

            if (result.Type == AggregationType.Count && result.ValueType == StreamDataSchemaColumnType.String)
            {
                var replacement = PrepareValue(_functionBuilder.GetParameter(function), false);
                result.Value = string.Format(result.Value!, replacement);
            }

            return result;
        }

        static LinkedList<string> GetProperties(string name) =>
            new(RemovePrefix(name).Split('.').Reverse());

        static (string Path, string Field)? BuildQueryToAccessNestedJson(
            in string jsonColumn,
            in string columnName)
        {
            var propertyInJson = GetProperties(columnName).First;
            if (propertyInJson is null)
                return null;

            var path = BuildQueryToAccessNestedJson(jsonColumn, propertyInJson.Next);
            return (path, propertyInJson.Value);
        }

        static string BuildQueryToAccessNestedJson(in string jsonColumn, in LinkedListNode<string>? segment) =>
            segment is null
                ? jsonColumn
                : $"JSONExtractRaw({BuildQueryToAccessNestedJson(jsonColumn, segment.Next)}, '{segment.Value}')";

        static string GetParentJsonField(string field) =>
            field.StartsWith(LabelsPrefix, StringComparison.OrdinalIgnoreCase) ? LabelsRawJson : SourceRawJson;

        string? CompareFieldWithValue(
            string value,
            string fieldPath,
            StreamDataSchemaColumnType columnType,
            StreamDataSchemaColumnType elementType = StreamDataSchemaColumnType.Unknown)
        {
            var valueType = value.GetSearchType();
            if (columnType == StreamDataSchemaColumnType.Array)
            {
                var castElement = elementType.CastArrayElement("x");
                if (string.IsNullOrEmpty(castElement))
                    return null;

                var innerExpr = CompareFieldWithValue(value, castElement, elementType);
                return innerExpr is not null ? $"arrayExists(x -> {innerExpr} > 0, {fieldPath})" : null;
            }

            // REM: заглушка до обновления Clickhouse
            if (columnType == StreamDataSchemaColumnType.Date
                && !ServiceFields.Any(val => fieldPath.Equals(val.Name.AddFieldQuotes())
                                             && val.Type == StreamDataSchemaColumnType.Date)
                && DateTimeOffset.TryParse(value, out var date))
            {
                value = date.ToString("yyyy-MM-ddTHH:mm:ss.fff*K");
            }

            // Если тип поля - строка, то поиск выполняем в любом случае.
            if (columnType == StreamDataSchemaColumnType.String || HasUnescapedWildcardSymbols(value))
            {
                var replacement = ReplaceWildcardSymbols(value, columnType);
                if (columnType != StreamDataSchemaColumnType.String)
                    fieldPath = $"toString({fieldPath})";
                return replacement is not null
                    ? StreamDataSchemaColumnType.String.GetEqualsStatement(fieldPath, replacement)
                    : null;
            }

            // Если поля в БД - Double, то разрешается также производить поиск по integer.
            if (columnType == StreamDataSchemaColumnType.Double && valueType == StreamDataSchemaColumnType.Integer)
                return valueType.GetEqualsStatement(fieldPath, value);

            // Если типы не совпадают, то пропускаем.
            return columnType != valueType ? null : valueType.GetEqualsStatement(fieldPath, value);
        }

        static bool HasUnescapedWildcardSymbols(string value)
        {
            char[] wildcardSymbols = { '*', '?' };
            var wildcardIndex = value.IndexOfAny(wildcardSymbols);
            while (wildcardIndex != -1)
            {
                var count = value[..wildcardIndex].CountFromTheEnd(Backslash);
                if (count.IsEven())
                    return true;
                wildcardIndex = value.IndexOfAny(wildcardSymbols, wildcardIndex + 1);
            }

            return false;
        }
    }
}
