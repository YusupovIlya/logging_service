using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Logging.Server.StreamData.Validator.Configuration.AppConstants.Symbols;

namespace Logging.Server.StreamData.Validator.Services.Implementation
{
    /// <summary>
    /// Класс для разбиения значение на строки на содержащие и не содержащие wildcard символы.
    /// </summary>
    public class MqlWildcardReplacer
    {
        readonly List<string> _result = new();
        readonly StringBuilder _lastString = new();
        bool _isWildcardBefore;

        /// <summary>
        /// Разбить значение на строки на содержащие и не содержащие wildcard символы.
        /// </summary>
        public (IEnumerable<string> Result, bool IsWildCardBefore) ReplaceWildcardSymbols(string query)
        {
            // Все значения между слешами являются неэкранированными, кроме символа идущего сразу за слешем.
            var i = 0;
            var indexOfBackslash = query.IndexOf(Backslash);

            while (i < query.Length)
            {
                if (i == indexOfBackslash)
                {
                    AddSymbol(query[++i]);
                    indexOfBackslash = query.IndexOf(Backslash, ++i);
                    continue;
                }

                if (indexOfBackslash == -1 && i == query.Length - 1)
                {
                    AddSymbol(query[i], query[i] == '*' || query[i] == '?');
                    break;
                }

                var replacement = SplitWildcardAndCommonSymbols(indexOfBackslash == -1
                    ? query[i..]
                    : query[i..indexOfBackslash]);
                AddStrings(replacement);

                if (indexOfBackslash == -1)
                    break;

                i = indexOfBackslash + 1;
                indexOfBackslash = query.IndexOf(Backslash, i);

                if (indexOfBackslash == -1 && i == query.Length - 1)
                {
                    AddSymbol(query[i]);
                    break;
                }
            }

            MoveNext();
            return (_result, _isWildcardBefore);
        }

        static bool IsWildcardSymbol(string value) => value.All("*?".Contains);

        void AddSymbol(char symbol, bool isWildcard = false)
        {
            if (_isWildcardBefore != isWildcard)
                MoveNext();

            _lastString.Append(symbol);
            _isWildcardBefore = isWildcard;
        }

        void AddString(string str)
        {
            var isWildcard = IsWildcardSymbol(str);
            if (_isWildcardBefore != isWildcard)
                MoveNext();

            _lastString.Append(str);
            _isWildcardBefore = isWildcard;
        }

        void MoveNext()
        {
            if (_lastString.Length == 0)
                return;
            _result.Add(_lastString.ToString());
            _lastString.Clear();
        }

        void AddStrings(IEnumerable<string> strings)
        {
            AddString(strings.First());
            var count = strings.Count();
            if (count == 1)
                return;
            MoveNext();
            _result.AddRange(strings.Skip(1).TakeWhile((_, i) => i < count - 2).ToList());
            AddString(strings.Last());
        }

        static IEnumerable<string> SplitWildcardAndCommonSymbols(string query) =>
            Regex.Matches(query, "(([?*])+|([^?*])+)")
                .Select(val => val.ToString())
                .ToList();
    }
}
