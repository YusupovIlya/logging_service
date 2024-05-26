using CsvHelper;
using Logging.Server.Service.StreamData.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Сервис для создания csv файлов.
    /// </summary>
    public class CsvFileService : FileService
    {
        /// <summary>
        /// Конструктор сервиса для создания csv файлов.
        /// </summary>
        public CsvFileService(string mimeType) : base(mimeType) { }

        /// <summary>
        /// Сформировать файл и вернуть его в виде массива байтов.
        /// </summary>
        /// <param name="streamFields">Словарь соответствия полей потокам.</param>
        /// <param name="orderedFields">Упорядоченные поля.</param>
        /// <param name="values">Данные.</param>
        /// <returns>Файл в виде массива байтов.</returns>
        public override byte[] Prepare(
            Dictionary<long, IEnumerable<string>> streamFields,
            IEnumerable<string> orderedFields,
            IEnumerable<BaseStreamDataEvent> values)
        {
            using var stream = new MemoryStream();
            using var writeStream = new StreamWriter(stream);
            using var csv = new CsvWriter(writeStream, CultureInfo.InvariantCulture);

            foreach (var fieldName in orderedFields)
                csv.WriteField(fieldName);
            csv.NextRecord();

            foreach (var value in values)
            {
                var preparedValues = GetPreparedValues(streamFields[1], value);
                //var preparedValues = GetPreparedValues(streamFields[value.StreamId], value);
                foreach (var field in orderedFields)
                    csv.WriteField(preparedValues.TryGetValue(field, out var preparedValue) ? preparedValue : string.Empty);

                csv.NextRecord();
            }

            writeStream.Flush();
            return stream.ToArray();
        }
    }
}
