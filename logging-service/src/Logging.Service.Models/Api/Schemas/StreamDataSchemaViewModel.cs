using System;
using System.Collections.Generic;

namespace Logging.Server.Models.StreamData.Api.Schemas
{
    /// <summary>
    /// Модель представления схемы данных потока.
    /// </summary>
    public class StreamDataSchemaViewModel
    {
        /// <summary>
        /// Идентификатор потока данных.
        /// </summary>
        public long StreamId { get; set; }

        /// <summary>
        /// Колонки таблицы базы данных.
        /// </summary>
        public IEnumerable<StreamDataSchemaColumnViewModel> Columns { get; set; } = Array.Empty<StreamDataSchemaColumnViewModel>();
    }
}
