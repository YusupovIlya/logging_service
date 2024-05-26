using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Logging.Server.Models.StreamData.Api;
using Logging.Server.Models.StreamData.Api.Aggregations;
using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.Models.StreamData.Api.StreamData;
using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.Extensions;
using Logging.Server.Service.StreamData.HttpServices;
using Logging.Server.Service.StreamData.Models;
using Logging.Server.Service.StreamData.Services;
using Logging.Server.Service.StreamData.Services.Implementation;
using Logging.Server.StreamData.Validator.Services.Implementation;
using Monq.Core.MvcExtensions.Extensions;
using Monq.Core.MvcExtensions.Validation;
using Monq.Core.MvcExtensions.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Logging.Server.StreamData.Validator.Services;
using static Monq.Core.BasicDotNetMicroservice.AuthConstants.AuthorizationScopes;

namespace Logging.Server.Service.StreamData.Controllers
{
    /// <summary>
    /// Контроллер для чтения потоковых данных.
    /// </summary>
    //[Authorize]
    [ApiController]
    [Route("api/cl/stream-data")]
    public class StreamDataController : ControllerBase
    {
        private readonly List<long> _streams = new(1) { 1, 2 };
        const string DefaultSortDir = "desc";
        readonly IMapper _mapper;
        readonly IMqlQueryParserBuilder _parserBuilder;
       // readonly IStringLocalizer _localizer;
        readonly IStreamDataRepository _streamDataRepository;
        readonly IStreamDataSchemasRepository _streamDataSchemasRepository;
        readonly IStreamsHttpService _streamsHttpService;


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StreamDataController"/>.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="parserBuilder">Строитель MQL парсера.</param>
        /// <param name="localizer">Сервис локализации для текста ошибок.</param>
        /// <param name="streamDataRepository">Репозиторий для чтения потоковых данных.</param>
        /// <param name="streamDataSchemasRepository">Репозиторий схем потоковых данных.</param>
        /// <param name="streamsHttpService">
        /// Интерфейс взаимодействия с потоками данных через cl-streams-service.
        /// </param>
        public StreamDataController(
            IMapper mapper,
            IMqlQueryParserBuilder parserBuilder,
            //IStringLocalizer localizer,
            IStreamDataRepository streamDataRepository,
            IStreamDataSchemasRepository streamDataSchemasRepository,
            IStreamsHttpService streamsHttpService)
        {
            _mapper = mapper;
            _parserBuilder = parserBuilder;
            _streamDataRepository = streamDataRepository;
            _streamDataSchemasRepository = streamDataSchemasRepository;
            _streamsHttpService = streamsHttpService;
            //_localizer = localizer;
        }

        /// <summary>
        /// Получить события потока данных по фильтру.
        /// </summary>
        /// <param name="value">Принимаемая модель фильтра событий потока данных.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        /// <response code="200">Запрос успешно исполнен.</response>
        /// <response code="403">Недостаточно прав.</response>
        //[Authorize(Read)]
        [HttpPost("filter")]
        [ValidateActionParameters]
        [Produces(typeof(StreamDataFilterResultViewModel))]
        public async Task<ActionResult<StreamDataFilterResultViewModel>> Filter(
            [FromBody] StreamDataFilterViewModel value,
            [FromQuery] string timestampSortDir = DefaultSortDir)
        {
            if (value.Timestamp.Start > value.Timestamp.End)
                (value.Timestamp.Start, value.Timestamp.End) = (value.Timestamp.End, value.Timestamp.Start);

            var schemas = await GetSchemas(_streams);
            var parser = _parserBuilder.Create();
            var schemasQueries = GetSchemasQueries(value.Query, parser, schemas);

            var events = await _streamDataRepository.GetEventsByFilter(schemasQueries, parser.Parameters, value.Timestamp, value.FilteredCount, timestampSortDir);
            var aggregations = await _streamDataRepository.GetAggregationsByFilter(schemasQueries, parser.Parameters, value.Timestamp, value.Interval);

            var result = new StreamDataFilterResultViewModel
            {
                Aggregations = _mapper.Map<IEnumerable<StreamDataAggregateViewModel>>(aggregations),
                Documents = _mapper.Map<IEnumerable<StreamDataEventViewModel>>(events)
            };

            return result;
        }

        /// <summary>
        /// Получить данные в виде файла указанного формата.
        /// </summary>
        /// <param name="value">Принимаемая модель получения файла с отфильтрованными событиями потока данных.</param>
        /// <param name="type">Тип файла.</param>
        /// <param name="timestampSortDir">Направление сортировки по дате регистрации события (по умолчанию desc).</param>
        /// <response code="200">Запрос успешно исполнен.</response>
        /// <response code="400">Некорректный тип файла.</response>
        /// <response code="403">Недостаточно прав.</response>
        //[Authorize(Read)]
        [HttpPost("filter/file")]
        [ValidateActionParameters]
        [Produces(typeof(FileResult))]
        public async Task<ActionResult> GetFile(
            [FromBody] StreamDataFilterFileViewModel value,
            [FromQuery][Required] string type,
            [FromQuery] string timestampSortDir = DefaultSortDir)
        {
            if (value.Timestamp.Start > value.Timestamp.End)
                (value.Timestamp.Start, value.Timestamp.End) = (value.Timestamp.End, value.Timestamp.Start);

            var fileService = FileServiceFactory.Create(type);
            if (fileService is null)
                return BadRequest(new ErrorResponseViewModel("Файл не корректен!"));

            //var permittedStreamIds = await GetPermittedStreamIds(value.StreamIds);
            //if (permittedStreamIds.IsEmpty())
            //    return File(Array.Empty<byte>(), fileService.MimeType);

            //var existingStreamIds = await _streamDataRepository.GetExistingStreamIds(permittedStreamIds!);
            var schemas = await GetSchemas(_streams);

            var checkedFields = GetOrderedFields(schemas, value.CheckedFields);
            var streamFields = GetStreamFields(schemas, checkedFields);
            var parser = _parserBuilder.Create();

            byte[]? file;
            if (fileService is CsvFileService)
            {
                var schemasQueries = GetSchemasQueries(value.Query, parser, schemas, streamFields);
                file = await _streamDataRepository.GetCsvFile(schemasQueries, parser.Parameters, value.Timestamp, checkedFields, timestampSortDir);
            }
            else
            {
                var schemasQueries = GetSchemasQueries(value.Query, parser, schemas);
                var events = await _streamDataRepository.GetEventsByFilter(schemasQueries, parser.Parameters, value.Timestamp, int.MaxValue, timestampSortDir);

                // Если нет выбранных полей, то берем все существующие поля для данных потоков.
                file = fileService.Prepare(streamFields, checkedFields, events);
            }

            return File(file, fileService.MimeType);
        }

        static IEnumerable<string> GetOrderedFields(IEnumerable<StreamDataSchemaViewModel> schemas, IEnumerable<string>? checkedFields) =>
            (checkedFields.IsEmpty() ? schemas.SelectMany(val => val.Columns).Select(val => val.Name) : checkedFields)!
            .Union(new[] {"_id", "@timestamp"}).ToList();

        static Dictionary<long, IEnumerable<string>> GetStreamFields(IEnumerable<StreamDataSchemaViewModel> schemas, IEnumerable<string> checkedFields) =>
            schemas.ToDictionary(key => key.StreamId, val => checkedFields.Where(x => val.Columns.Any(y => y.Name == x) || x.IsServiceField()));

        /// <summary>
        /// Получить результат агрегации поля.
        /// </summary>
        /// <param name="value">Принимаемая модель агрегации.</param>
        /// <response code="200">Запрос успешно исполнен.</response>
        /// <response code="400">Невалидная строка запроса.</response>
        /// <response code="403">Недостаточно прав.</response>
        //[Authorize(Read)]
        [HttpPost("aggregate")]
        [ValidateActionParameters]
        [Produces(typeof(AggregationViewModel))]
        public async Task<ActionResult<AggregationViewModel>?> Aggregate([FromBody] AggregationPostViewModel value)
        {
            if (value.Timestamp.Start > value.Timestamp.End)
                (value.Timestamp.Start, value.Timestamp.End) = (value.Timestamp.End, value.Timestamp.Start);

            if (string.IsNullOrEmpty(value.AggregationQuery))
                return BadRequest(new ErrorResponseViewModel("Требуется запрос!"));

            //var permittedStreamIds = await GetPermittedStreamIds(value.StreamIds);
            //if (permittedStreamIds.IsEmpty())
            //    return null;

            //var existingStreamIds = await _streamDataRepository.GetExistingStreamIds(permittedStreamIds!);
            var schemas = await GetSchemas(_streams);
            var parser = _parserBuilder.Create();

            var aggregationQuery = parser.BuildAggregationFunctionQuery(value.AggregationQuery, schemas.SelectMany(val => val.Columns).Distinct());
            if (aggregationQuery is null)
                return BadRequest(new ErrorResponseViewModel("Запрос не корректен!"));

            var query = string.IsNullOrWhiteSpace(value.Query)
                ? $"{aggregationQuery.Column}:exists()"
                : $"({value.Query}) AND {aggregationQuery.Column}:exists()";
           
            var acceptableSchemas = aggregationQuery.Column.IsServiceField()
                ? schemas
                : schemas.Where(val => val.Columns.Any(col => col.Name == aggregationQuery.Column)).ToList();
            var schemasQueries = GetSchemasQueries(query, parser, acceptableSchemas);
            var result = await _streamDataRepository.GetAggregation(schemasQueries, aggregationQuery, parser.Parameters, value.Timestamp);
            return new AggregationViewModel
            {
                Column = aggregationQuery.Column,
                Function = aggregationQuery.Type.ToString(),
                Result = _mapper.Map<IEnumerable<AggregationResultViewModel>>(result)
            };
        }


        async Task<IEnumerable<long>?> GetPermittedStreamIds(IEnumerable<long>? streamIds)
        {
            //var streams = await _streamsHttpService.Filter(new StreamFilterViewModel { Ids = streamIds, IsDeleted = false});
            //return streams?.Select(x => x.Id).ToArray() ?? Array.Empty<long>();
            return Array.Empty<long>();
        }

        async Task<IEnumerable<StreamDataSchemaViewModel>> GetSchemas(IEnumerable<long> streamIds)
        {
            var getSchemaTasks = streamIds.Select(streamId => _streamDataSchemasRepository.Get(streamId));
            // Требуется проверка на null, т.к. таблица может быть уже удалена при попытке получения схемы.
            var schemas = await Task.WhenAll(getSchemaTasks);
            return schemas.Where(x => x is not null).ToList()!;
        }

        static IEnumerable<Schema> GetSchemasQueries(
            string? query,
            MqlQueryParser parser,
            IEnumerable<StreamDataSchemaViewModel> schemas,
            IDictionary<long, IEnumerable<string>>? schemaFields = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return schemaFields is null
                    ? schemas.Select(val => new Schema { TableName = val.StreamId.ToTableName() }).ToList()
                    : schemas.Select(val => new Schema { TableName = val.StreamId.ToTableName(), StreamFields = schemaFields[val.StreamId] }).ToList();

            var schemasQueries = schemaFields is null
                ? schemas.Select(val => new Schema
                {
                    TableName = val.StreamId.ToTableName(),
                    Conditions = parser.ToSqlQuery(query, val.Columns)
                }).ToList()
                : schemas.Select(val => new Schema
                {
                    TableName = val.StreamId.ToTableName(),
                    StreamFields = schemaFields[val.StreamId],
                    Conditions = parser.ToSqlQuery(query, val.Columns)
                }).ToList();

            schemasQueries.RemoveAll(x => x.Conditions == AppConstants.FalseCondition);
            return schemasQueries;
        }


        /// <summary>
        /// Получить количество событий потока данных.
        /// </summary>
        /// <param name="streamId">Идентификатор потока данных.</param>
        //[Authorize(Read)]
        [HttpGet("{streamId}/count")]
        //[ValidateActionParameters]
        public async Task<ActionResult<StreamDataEventsCountViewModel>> GetCount(
            [Range(1, long.MaxValue, ErrorMessage = "Stream identifier is incorrect.")] long streamId)
        {
            var count = await _streamDataRepository.GetCount(streamId);
            var result = new StreamDataEventsCountViewModel
            {
                Count = count
            };
            return result;
        }
    }
}
