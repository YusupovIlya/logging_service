using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Logging.Server.Service.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения для клиента ClickHouse.
    /// </summary>
    public static class ClickHouseClientExtensions
    {
        /// <summary>
        /// Извлечь записи и преобразовать в конкретный тип из потока чтения данных.
        /// </summary>
        /// <param name="reader">Поток чтения данных.</param>
        public static async Task<IEnumerable<T>> FetchRecords<T>(this DbDataReader reader)
        {
            var result = new List<T>();

            var isValueType = typeof(T).IsValueType;
            var isString = typeof(T) == typeof(string);

            var properties = new List<(PropertyInfo Prop, string ColName)>();

            if (!(isValueType && isString))
                properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<IgnoreDataMemberAttribute>() is null)
                    .Select(x => (x, x.TryGetClickHouseName()))
                    .ToList();

            while (await reader.ReadAsync())
            {
                if (isValueType || isString)
                {
                    var val = reader.GetValue(0);
                    result.Add((T)val);
                }
                else
                {
                    var entry = Activator.CreateInstance<T>();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = $"`{reader.GetName(i)}`";
                        var (prop, _) = properties.FirstOrDefault(x => x.ColName == columnName);
                        if (prop is not null)
                        {
                            if (reader.GetFieldType(i) == typeof(DateTime) && prop.PropertyType == typeof(DateTimeOffset))
                            {
                                DateTimeOffset convertedDateTime = DateTime.SpecifyKind((DateTime) reader.GetValue(i), DateTimeKind.Utc);
                                convertedDateTime = convertedDateTime.ToOffset(TimeZoneInfo.Local.BaseUtcOffset);
                                prop.SetValue(entry, convertedDateTime);
                                continue;
                            }

                            prop.SetValue(entry, reader.GetValue(i));
                        }
                    }

                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
