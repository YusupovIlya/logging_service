using ClosedXML.Excel;
using Logging.Server.Service.StreamData.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Сервис для создания xlsx файлов.
    /// </summary>
    public class XlsxFileService : FileService
    {
        /// <summary>
        /// Конструктор сервиса для создания xlsx файлов.
        /// </summary>
        public XlsxFileService(string mimeType) : base(mimeType) { }

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
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(DateTimeOffset.Now.ToString("dd.MM.yyyy"));

            var i = 1;
            foreach (var field in orderedFields)
            {
                worksheet.Cell(1, i).SetValue(field);
                i++;
            }

            var row = 2;
            foreach (var value in values)
            {
                //var preparedValues = GetPreparedValues(streamFields[value.StreamId], value);
                var preparedValues = GetPreparedValues(streamFields.First().Value, value);
                var column = 1;
                foreach (var field in orderedFields)
                {
                    worksheet.Cell(row, column).SetValue(preparedValues.TryGetValue(field, out var preparedValue) ? preparedValue : string.Empty);
                    column++;
                }

                if (!worksheet.Row(row).IsEmpty())
                    row++;
            }

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
