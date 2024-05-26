using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Logging.Server.StreamData.Validator.Extensions;
using Logging.Server.StreamData.Validator.Models;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Класс для разделения запроса по подзапросы.
    /// </summary>
    public class MqlQuerySplitter
    {
        const string SpecialSymbols = "!()-";
        static readonly char[] _trailingCharacters = { RightBrace, Space };

        int _termNumber;
        readonly StringBuilder _query = new();
        readonly List<MqlTerm> _terms = new();

        /// <summary>
        /// Разделить запрос на подзапросы.
        /// </summary>
        /// <param name="query">Запрос.</param>
        public (string Query, IEnumerable<MqlTerm> Terms) SplitQuery(string query)
        {
            var i = 0;
            while (i != -1 && i < query.Length)
                i = query[i] switch
                {
                    _ when SpecialSymbols.Contains(query[i]) => ReplaceSpecialSymbols(query, i),
                    DoubleQuote => AddEscapedFullTerm(query, i),
                    Space => AddSpace(query, i),
                    _ when query.IndexOf(Space, i) < query.IndexOf(Colon, i)
                           && query.IndexOf(Space, i) != -1
                           || query.IndexOf(Colon, i) == -1 => AddFullSearchTerm(query, i),
                    _ => AddCommonTerm(query, i)
                };

            if (!MqlValidator.IsOccursValid(Regex.Split(_query.ToString(), "(AND|OR)")))
                SetFalseCondition();
            return (_query.ToString(), _terms);
        }

        int ReplaceSpecialSymbols(string query, int i)
        {
            var replacements = query[i..]
                .Select(ReplaceSpecialSymbol)
                .TakeWhile(val => !string.IsNullOrWhiteSpace(val))
                .ToList();
            _query.Append(string.Concat(replacements));
            return replacements.Count + i;
        }

        static string ReplaceSpecialSymbol(char symbol) =>
            symbol switch
            {
                '!' => "NOT ",
                '-' => "NOT ",
                '(' => "(",
                ')' => ")",
                _ => string.Empty
            };

        int AddEscapedFullTerm(string query, int i)
        {
            i++;
            var quoteIndex = GetQuoteIndex(query, i);
            if (quoteIndex == -1)
                return SetFalseCondition();

            AddTerm(TermType.FullSearchTerm, query[i..quoteIndex]);
            return quoteIndex + 1;
        }

        static int GetQuoteIndex(string query, int i)
        {
            var quoteIndex = query.IndexOf(DoubleQuote, i + 1);
            if (quoteIndex == -1)
                return -1;

            var backslashCount = query[i..quoteIndex].CountFromTheEnd(Backslash);
            while (!backslashCount.IsEven())
            {
                quoteIndex = query.IndexOf(DoubleQuote, quoteIndex + 1);
                if (quoteIndex == -1)
                    return -1;
                backslashCount = query[i..quoteIndex].CountFromTheEnd(Backslash);
            }

            return quoteIndex;
        }

        int AddSpace(string query, int i)
        {
            while (i < query.Length && query[i] == Space)
                i++;
            _query.Append(Space);
            return i;
        }

        int AddFullSearchTerm(string query, int i)
        {
            var spaceIndex = query.IndexOfAny(_trailingCharacters, i);
            var value = spaceIndex == -1 ? query[i..] : query[i..spaceIndex];
            if (Occurs.Contains(value))
                _query.Append(value);
            else
                AddTerm(TermType.FullSearchTerm, value);

            return spaceIndex;
        }

        int AddCommonTerm(string query, int i)
        {
            var colonIndex = query.IndexOf(Colon, i);
            if (colonIndex == i || colonIndex + 1 == query.Length)
                return SetFalseCondition();

            var field = query[i..colonIndex];
            if (!MqlValidator.IsFieldValid(field))
            {
                _query.Append(FalseCondition);
                if (query[colonIndex + 1] != DoubleQuote)
                    return query.IndexOfAny(_trailingCharacters, colonIndex);
                var quoteIndex = GetQuoteIndex(query, colonIndex + 1);
                if (quoteIndex == -1)
                    SetFalseCondition();
                return quoteIndex;
            }

            if (query[colonIndex + 1] == DoubleQuote)
            {
                var quoteIndex = GetQuoteIndex(query, colonIndex + 1);
                if (quoteIndex == -1)
                    return SetFalseCondition();

                AddTerm(TermType.EscapedTerm, query[(colonIndex + 2)..quoteIndex], field);
                return quoteIndex + 1;
            }

            var index = query.IndexOfAny(_trailingCharacters, colonIndex);
            string value;
            if (index != -1)
            {
                if (query[index - 1] == LeftBrace)
                    index++;
                value = query[(colonIndex + 1)..index];
            }
            else
                value = query[(colonIndex + 1)..];

            AddTerm(TermType.CommonTerm, value, field);
            return index;
        }

        void AddTerm(TermType type, string value, string field = default!)
        {
            _terms.Add(new MqlTerm { Value = value, Field = field, Type = type });
            _query.Append($"{{{_termNumber++}}}");
        }

        int SetFalseCondition()
        {
            _query.Clear();
            _query.Append(FalseCondition);
            _terms.Clear();
            return -1;
        }
    }
}