using System;
using System.Collections.Generic;
using System.Linq;
using Logging.Server.Models.StreamData.Api.Schemas;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Extensions
{
    /// <summary>
    /// Набор методов расширения для помощи с валидацией.
    /// </summary>
    public static class MqlExtensions
    {
        /// <summary>
        /// Получить количество подряд идущих символов с конца.
        /// </summary>
        /// <param name="value">Значение, в котором считаются символы.</param>
        /// <param name="symbol">Символ, который нужно посчитать.</param>
        /// <returns>Количество символов с конца.</returns>
        public static int CountFromTheEnd(this string value, char symbol)
        {
            var i = value.Length - 1;
            while (i >= 0 && value[i] == symbol)
                i--;

            return value.Length - i - 1;
        }

        /// <summary>
        /// Проверка чётное ли число.
        /// </summary>
        /// <param name="value">Число.</param>
        /// <returns>Флаг, отображающий чётное ли число.</returns>
        public static bool IsEven(this int value) => value % 2 == 0;

        /// <summary>
        /// Получить поле по названию.
        /// </summary>
        /// <param name="columns">Все поля.</param>
        /// <param name="name">Название нужного поля.</param>
        /// <returns>Поле.</returns>
        public static StreamDataSchemaColumnViewModel? GetByName(
            this IEnumerable<StreamDataSchemaColumnViewModel> columns,
            string name) =>
            columns.FirstOrDefault(val => val.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                          || val.Name.Equals(string.Concat(SourcePrefix, name), StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// Добавить кавычки для значения.
        /// </summary>
        public static string AddFieldQuotes(this string value) =>
            string.Concat('`', value, '`');

        /// <summary>
        /// Применить форматирование для подстановки параметра в ClickHouse запрос.
        /// </summary>
        public static string ToReplacement(this string value) =>
            string.Concat('{', value, Colon, StreamDataSchemaColumnType.String.GetClickHouseType(), '}');
    }
}
