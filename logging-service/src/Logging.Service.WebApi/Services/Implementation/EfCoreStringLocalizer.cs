using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Logging.Server.Service.StreamData.Configuration;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена

    //public class EfCoreStringLocalizer : IStringLocalizer
    //{
    //    /// <summary>
    //    /// Gets the string resource with the given name.
    //    /// </summary>
    //    /// <param name="name">The name of the string resource.</param>
    //    /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
    //    public LocalizedString this[string name] => Get(name);

    //    /// <summary>
    //    /// Gets the string resource with the given name and formatted with the supplied arguments.
    //    /// </summary>
    //    /// <param name="name">The name of the string resource.</param>
    //    /// <param name="arguments">The values to format the string with.</param>
    //    /// <returns>The formatted string resource as a <see cref="LocalizedString" />.</returns>
    //    public LocalizedString this[string name, params object[] arguments] => Get(name);

    //    readonly StreamDataContext _context;
    //    readonly IMapper _mapper;
    //    readonly IMemoryCache _cache;

    //    static readonly TimeSpan _cacheExpiresAfter = TimeSpan.FromMinutes(5);

    //    /// <summary>
    //    /// Конструктор сервиса локализации запросов на основе <see cref="IStringLocalizer"/>.
    //    /// Создаёт новый экземпляр <see cref="EfCoreStringLocalizer"/>.
    //    /// </summary>
    //    public EfCoreStringLocalizer(
    //        StreamDataContext context,
    //        IMapper mapper,
    //        IMemoryCache cache)
    //    {
    //        _context = context;
    //        _mapper = mapper;
    //        _cache = cache;
    //    }

    //    /// <summary>
    //    /// Gets all string resources.
    //    /// </summary>
    //    /// <param name="includeParentCultures">
    //    /// A <see cref="bool" /> indicating whether to include strings from parent cultures.
    //    /// </param>
    //    /// <returns>The strings.</returns>
    //    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    //    {
    //        var cultureName = CultureInfo.CurrentCulture.Name;
    //        var key = $"{cultureName}:{nameof(GetAllStrings)}";
    //        var result = _cache.GetOrCreate(key, cache =>
    //        {
    //            cache.AbsoluteExpirationRelativeToNow = _cacheExpiresAfter;
    //            var resources = _context.Resources
    //                .AsNoTracking()
    //                .Where(val => val.LanguageId == cultureName)
    //                .ToListAsync();

    //            return _mapper.Map<IEnumerable<LocalizedString>>(resources.Result);
    //        });
    //        return result;
    //    }

    //    /// <summary>
    //    /// Creates a new <see cref="IStringLocalizer" /> for a specific <see cref="CultureInfo" />.
    //    /// </summary>
    //    /// <param name="culture">The <see cref="CultureInfo" /> to use.</param>
    //    /// <returns>A culture-specific <see cref="IStringLocalizer" />.</returns>
    //    public IStringLocalizer WithCulture(CultureInfo culture)
    //    {
    //        CultureInfo.DefaultThreadCurrentCulture = culture;
    //        return new EfCoreStringLocalizer(_context, _mapper, _cache);
    //    }

    //    string? GetString(string name)
    //    {
    //        var cultureName = CultureInfo.CurrentCulture.Name;
    //        var key = $"{cultureName}:{nameof(GetString)}:{name}";
    //        var result = _cache.GetOrCreate(key, cache =>
    //        {
    //            cache.AbsoluteExpirationRelativeToNow = _cacheExpiresAfter;
    //            try
    //            {
    //                return _context.Resources
    //                     .AsNoTracking()
    //                     .FirstOrDefault(val => val.LanguageId == cultureName && val.Key == name)?
    //                     .Value;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        });
    //        return result;
    //    }

    //    LocalizedString Get(string name)
    //    {
    //        var result = GetString(name);
    //        return result is null ? Fallback(name) : new LocalizedString(name, result);
    //    }

    //    static LocalizedString Fallback(string name) =>
    //        new(name, $"[{CultureInfo.CurrentCulture.Name}:{name}]", true);
    //}
}
