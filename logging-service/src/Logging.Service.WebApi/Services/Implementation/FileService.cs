using Logging.Server.Service.StreamData.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Logging.Server.Service.StreamData.Configuration.AppConstants;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Базовый класс для преобразования данных в файл.
    /// </summary>
    public abstract class FileService
    {
        /// <summary>
        /// MIME тип файла.
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// Базовый конструктор.
        /// </summary>
        protected FileService(string mimeType) => MimeType = mimeType;

        /// <summary>
        /// Сформировать файл и вернуть его в виде массива байтов.
        /// </summary>
        /// <param name="streamFields">Словарь соответствия полей потокам.</param>
        /// <param name="orderedFields">Упорядоченные поля.</param>
        /// <param name="values">Данные.</param>
        /// <returns>Файл в виде массива байтов.</returns>
        public abstract byte[] Prepare(Dictionary<long, IEnumerable<string>> streamFields, IEnumerable<string> orderedFields, IEnumerable<BaseStreamDataEvent> values);

        /// <summary>
        /// Получить значение поля по его названию.
        /// </summary>
        /// <param name="fields">Поля.</param>
        /// <param name="value">Модель, из которой получается значение.</param>
        protected Dictionary<string, string> GetPreparedValues(IEnumerable<string> fields, BaseStreamDataEvent value) =>
            GetFieldsValues(fields, value).ToDictionary(key => key.Field, val => val.Value);

        IEnumerable<(string Field, string Value)> GetFieldsValues(IEnumerable<string> fields, BaseStreamDataEvent value)
        {
            var rawJson = JObject.Parse(value.RawJson);
            var isLabelsEmpty = string.IsNullOrEmpty(value.LabelsRawJson);
            var labelJson = isLabelsEmpty ? default : JObject.Parse(value.LabelsRawJson!);

            foreach (var field in fields)
            {
                switch (field)
                {
                    case "_id":
                        yield return (field, value.Id.ToString());
                        break;
                    //case "_rawId":
                    //    yield return (field, value.RawId.ToString());
                    //    break;
                    //case "_aggregatedAt":
                    //    yield return (field, value.AggregatedAt.ToString());
                    //    break;
                    case "@timestamp":
                        yield return (field, value.Timestamp.ToString());
                        break;
                    //case "_date":
                    //    yield return (field, value.Date.ToString());
                    //    break;
                    //case "_userspaceId":
                    //    yield return (field, value.UserspaceId.ToString());
                    //    break;
                    //case "_stream.name":
                    //    yield return (field, value.StreamName);
                    //    break;
                    //case "_stream.id":
                    //    yield return (field, value.StreamId.ToString());
                    //    break;
                }

                string token;
                if (field.StartsWith(SourcePrefix))
                {
                    token = Escape(field[SourcePrefix.Length..]);
                    yield return (field, rawJson.SelectToken(token)?.ToString() ?? string.Empty);
                }
                else if (field.StartsWith(LabelsPrefix) && !isLabelsEmpty)
                {
                    token = Escape(field[LabelsPrefix.Length..]);
                    yield return (field, labelJson.SelectToken(token)?.ToString() ?? string.Empty);
                }
            }
        }

        static string Escape(string token)
        {
            return string.Join(string.Empty,
                Regex.Replace(token, @"([\\\'])", @"\$1")
                    .Split('.')
                    .Select(val => $"['{val}']")
                    .ToList());
        }

        /// <summary>
        /// Добавить обязательные поля.
        /// </summary>
        /// <param name="fields">Исходные выбранные поля.</param>
        protected IEnumerable<string> AddRequiredFields(IEnumerable<string> fields) =>
            fields.Prepend("_id").Prepend("@timestamp").Distinct();
    }
}
