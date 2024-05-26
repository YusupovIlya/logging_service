using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Logging.Server.Models.StreamData.Api.Schemas
{
    /// <summary>
    /// Модель для создания схемы потоковых данных.
    /// </summary>
    public class StreamDataSchemaPostViewModel
    {
        /// <summary>
        /// Идентификатор потока данных.
        /// </summary>
        [Range(1, long.MaxValue, ErrorMessage = "Wrong stream ID.")]
        public long StreamId { get; set; }

        /// <summary>
        /// Колонки таблицы базы данных.
        /// </summary>
        public IEnumerable<StreamDataSchemaColumnViewModel> Columns { get; set; } = Array.Empty<StreamDataSchemaColumnViewModel>();
    }
}
