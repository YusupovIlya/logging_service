using System;
using TimeZoneConverter;

namespace Logging.Server.Models.StreamData.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        const ushort DateTimePrecision = 3;

        /// <summary>
        /// Получить наименование временной зоны в tzdb.
        /// </summary>
        public static bool TryGetTzdbIdByOffset(this DateTimeOffset dateTimeOffset, out string? id)
        {
            foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                if (dateTimeOffset.Offset != timeZone.BaseUtcOffset)
                    continue;
                id = TZConvert.TryWindowsToIana(timeZone.Id, out var ianaTimeZoneName) ? ianaTimeZoneName : timeZone.Id;
                return true;
            }

            id = null;
            return false;
        }

        /// <summary>
        /// Преобразовать в DateTime64, который поддерживает ClickHouse.
        /// </summary>
        public static string ToDateTime64(DateTimeOffset date) =>
            $"toDateTime64('{date:yyyy-MM-dd HH:mm:ss.fff}', {DateTimePrecision}{(date.TryGetTzdbIdByOffset(out var tzdbId) ? $", '{tzdbId}'" : string.Empty)})";

        /// <summary>
        /// Преобразовать в дату без времени.
        /// </summary>
        public static string ToDate(DateTimeOffset date) => $"'{date:yyyy-MM-dd}'";
    }
}