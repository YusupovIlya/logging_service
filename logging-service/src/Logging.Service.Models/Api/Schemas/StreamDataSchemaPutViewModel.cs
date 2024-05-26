using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Logging.Server.Models.StreamData.Api.Schemas
{
    /// <summary>
    /// Модель для обновления схемы потоковых данных.
    /// </summary>
    public class StreamDataSchemaPutViewModel
    {
        /// <summary>
        /// Идентификатор потока данных.
        /// </summary>
        [Range(1, long.MaxValue, ErrorMessage = "Wrong stream ID.")]
        public long StreamId { get; set; }

        /// <summary>
        /// Колонки таблицы базы данных, которые необходимо добавить в схему.
        /// </summary>
        public IEnumerable<StreamDataSchemaColumnViewModel> ColumnsToAdd { get; set; } = Array.Empty<StreamDataSchemaColumnViewModel>();
    }
}
