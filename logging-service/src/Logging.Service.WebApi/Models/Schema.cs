using System;
using System.Collections.Generic;

namespace Logging.Server.Service.StreamData.Models
{
    /// <summary>
    /// Упрощенная схема потока.
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// Поля принадлежащие потоку.
        /// </summary>
        public IEnumerable<string> StreamFields { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Запрос, построенный на основе схемы потока.
        /// </summary>
        public string Conditions { get; set; } = default!;

        /// <summary>
        /// Название таблицы, соответствующей потоку.
        /// </summary>
        public string TableName { get; set; } = default!;
    }
}
