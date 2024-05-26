using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Logging.Server.Service.StreamData.Extensions;
using Logging.Server.Service.StreamData.Helpers;
using Monq.Core.BasicDotNetMicroservice;
using Monq.Core.HttpClientExtensions;
//using Monq.Core.HttpClientExtensions.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static Logging.Server.Service.StreamData.Configuration.AppConstants;

namespace Logging.Server.Service.StreamData.HttpServices
{
    /// <summary>
    /// Реализация <see cref="IStreamsHttpService"/>, которая представляет собой
    /// интерфейс взаимодействия с потоками данных через cl-streams-service.
    /// </summary>
    /// <remarks>
    /// https://gitlab.monq.ru/smon.monq.ru/cl/cl-streams-service
    /// </remarks>
    public class StreamsHttpServiceImpl : IStreamsHttpService//: BasicSingleHttpService<AppConfiguration>, IStreamsHttpService
    {
        //readonly IMemoryCache _memoryCache;

        //readonly long _userspaceId;
        //readonly long _currentUserId;

        //readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(10);
        //readonly TimeSpan _defaultCacheTime = TimeSpan.FromMinutes(1);

        ///// <summary>
        ///// Инициализирует новый экземпляр класса <see cref="StreamsHttpServiceImpl" />.
        ///// </summary>
        ///// <param name="optionsAccessor">The options accessor.</param>
        ///// <param name="log">The logger factory.</param>
        ///// <param name="configuration">The configuration.</param>
        ///// <param name="httpContextAccessor">The HTTP context accessor.</param>
        ///// <param name="memoryCache"></param>
        ///// <param name="httpMessageInvoker">The HTTP message invoker.</param>
        //public StreamsHttpServiceImpl(
        //    IOptions<AppConfiguration> optionsAccessor,
        //    ILoggerFactory log,
        //    BasicHttpServiceOptions configuration,
        //    IHttpContextAccessor httpContextAccessor,
        //    IMemoryCache? memoryCache = null,
        //    HttpMessageHandler? httpMessageInvoker = null)
        //    : base(optionsAccessor, log, configuration, httpContextAccessor,
        //        new Uri(new Uri(optionsAccessor.Value.BaseUri), MicroserviceBaseUris.Cl).ToString(),
        //        httpMessageInvoker)
        //{
        //    Guard.Against.Null(httpContextAccessor, nameof(httpContextAccessor));
        //    Guard.Against.Null(memoryCache, nameof(memoryCache));

        //    _memoryCache = memoryCache;
        //    _userspaceId = httpContextAccessor.GetUserspaceId();
        //    if (httpContextAccessor.HttpContext is null)
        //        throw new ArgumentNullException(nameof(httpContextAccessor));
        //    _currentUserId = httpContextAccessor.HttpContext.User.Subject();
        //}

        ///// <inheritdoc />
        //public async Task<IEnumerable<StreamListViewModel>?> Filter(StreamFilterViewModel value)
        //{
        //    var key = $"{nameof(Filter)}.{_userspaceId}.{_currentUserId}.{HashHelper.GetShaHash(JsonConvert.SerializeObject(value))}";

        //    return await _memoryCache.GetOrCreateAsync(key, async cache =>
        //    {
        //        cache.SetAbsoluteExpiration(_defaultCacheTime);
        //        const string uri = "streams/filter";
        //        var client = CreateRestHttpClient();
        //        var result = await client.Post<StreamFilterViewModel, IEnumerable<StreamListViewModel>>(uri, value, _defaultTimeout);
        //        return result.ResultObject;
        //    });
        //}
    }
}