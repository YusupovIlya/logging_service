using Logging.Server.Models.StreamData.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Server.Models.StreamData.AE
{
    /// <summary>
    /// Модель сообщения для добавления события потоковых данных.
    /// </summary>
    public class StreamDataEventAeModel
    {
        /// <summary>
        /// Версия схемы таблицы потоковых данных.
        /// </summary>
        public int SchemaVersion { get; set; }

        /// <summary>
        /// Словарь значений колонок таблицы потоковых данных. Ключ - название столбца.
        /// </summary>
        public IDictionary<string, object?> DbValues { get; set; } = Array.Empty<KeyValuePair<string, object?>>().ToDictionary(val => val.Key, val => val.Value);

        /// <summary>
        /// Непосредственная модель события потоковых данных.
        /// </summary>
        public StreamDataEventViewModel Event { get; set; } = default!;

        /// <summary>
        /// Счётчик ошибок записи события.
        /// </summary>
        public int ErrorsCount { get; set; } = 0;

        /// <summary>
        /// Увеличить счётчик ошибок записи события на единицу.
        /// </summary>
        public void IncrementErrorsCount()
        {
            ErrorsCount++;
        }
    }
}
