using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Server.Service.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения для работы со словарями.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Удалить все элементы, которые соответствуют условию <paramref name="predicate"/>.
        /// </summary>
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue, bool> predicate)
        {
            var keys = dict.Keys.Where(k => predicate(dict[k])).ToList();
            foreach (var key in keys)
                dict.Remove(key);
        }
    }
}