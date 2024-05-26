using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Logging.Server.StreamData.Validator.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Проверить не пустая ли коллекция.
        /// </summary>
        public static bool IsAny<T>([NotNullWhen(true)] this IEnumerable<T>? data) => data is not null && data.Any();

        /// <summary>
        /// Определяет все ли свойства объекта являются null или пустым IEnumerable.
        /// </summary>
        /// <returns>
        ///   <c>true</c> Если объект пустой; иначе, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this object obj)
        {
            switch (obj)
            {
                case null:
                    return true;
                case IEnumerable enumerable:
                    return !enumerable.Any();
            }

            foreach (var prop in obj.GetType().GetProperties())
                switch (prop.GetValue(obj))
                {
                    case IEnumerable enumerable when enumerable.Any():
                    case not null and not IEnumerable:
                        return false;
                }

            return true;
        }

        static bool Any(this IEnumerable? source)
        {
            if (source is null)
                return false;

            var enumerator = source.GetEnumerator();
            return enumerator.MoveNext();
        }
    }
}
