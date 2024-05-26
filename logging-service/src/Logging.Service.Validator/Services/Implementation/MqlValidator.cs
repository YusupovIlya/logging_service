using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Logging.Server.StreamData.Validator.Extensions;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Класс предварительной валидации запроса.
    /// </summary>
    public class MqlValidator
    {
        /// <summary>
        /// Проверка на валидность запроса. Проверяются специальные символы и скобки.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <returns>Результат валидации.</returns>
        public static bool IsQueryValid(string query)
        {
            const string specSymbols = "?*\"";
            var quoteBefore = false;
            var rightBraceBefore = false;
            var negationBefore = false;
            var slashCount = 0;
            var leftCount = 0;
            foreach (var symbol in query)
            {
                if (symbol == Backslash)
                {
                    slashCount++;
                    continue;
                }

                if (!slashCount.IsEven() && !specSymbols.Contains(symbol))
                    return false;

                if (slashCount.IsEven() && symbol == DoubleQuote)
                    quoteBefore = !quoteBefore;

                slashCount = 0;
                if (quoteBefore)
                    continue;

                if (negationBefore && " ):".Contains(symbol))
                    return false;

                if (rightBraceBefore && symbol != Space && symbol != RightBrace)
                    return false;

                rightBraceBefore = false;
                if (symbol == RightBrace)
                {
                    leftCount--;
                    rightBraceBefore = true;
                }
                else if (symbol == LeftBrace)
                    leftCount++;

                if (leftCount < 0)
                    return false;

                negationBefore = "!-".Contains(symbol);
            }

            return leftCount == 0 && !quoteBefore && slashCount.IsEven() && !negationBefore;
        }

        /// <summary>
        /// Проверка на валидность поля.
        /// </summary>
        public static bool IsFieldValid(string field) =>
            Regex.Matches(field, @"[\d\w_\-.@]+").Count == 1;

        /// <summary>
        /// Проверка, валидны ли операторы.
        /// </summary>
        /// <param name="value">Разбитый пробелами запрос.</param>
        public static bool IsOccursValid(IEnumerable<string> value)
        {
            var isEven = value.Count(val => !string.IsNullOrEmpty(val)).IsEven();
            if (isEven)
                return false;
            return !value.Where((val, i) => i.IsEven() == Occurs.Contains(val)
                                            || i.IsEven() && Regex.Matches(val, @"\{\d+\}|0").Count != 1).ToList().Any();
        }
    }
}
