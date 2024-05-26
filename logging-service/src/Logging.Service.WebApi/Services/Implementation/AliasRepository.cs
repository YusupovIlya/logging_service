using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Logging.Server.Models.StreamData.Api;
using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.Models;

namespace Logging.Server.Service.StreamData.Services.Implementation
{
    // REM: Репозиторий будет содержать обращение к кешу.

    /// <summary>
    /// Реализация репозитория алиасов.
    /// </summary>
    //public class AliasRepository : IAliasRepository
    //{
    //    readonly IMapper _mapper;
    //    readonly StreamDataContext _context;

    //    /// <summary>
    //    /// Конструктор реализации репозитория алиасов.
    //    /// </summary>
    //    public AliasRepository(StreamDataContext context, IMapper mapper)
    //    {
    //        _context = context;
    //        _mapper = mapper;
    //    }

    //    /// <summary>
    //    /// Создать алиас.
    //    /// </summary>
    //    /// <param name="aliasViewModel">Создаваемый алиас.</param>
    //    public async Task<AliasViewModel?> Upsert(AliasViewModel aliasViewModel)
    //    {
    //        var storedAlias = await _context.Aliases.AsNoTracking().FirstOrDefaultAsync(val =>
    //            val.UserspaceId == aliasViewModel.UserspaceId
    //            && val.FieldName == aliasViewModel.FieldName
    //            && val.Value == aliasViewModel.Value);

    //        var alias = _mapper.Map<Alias>(aliasViewModel);

    //        if (storedAlias is not null && storedAlias.IsDefault == alias.IsDefault)
    //            return null;

    //        if (alias.IsDefault)
    //        {
    //            var defaultAlias = await _context.Aliases.AsNoTracking()
    //                .FirstOrDefaultAsync(val => val.UserspaceId == alias.UserspaceId
    //                                            && val.FieldName == alias.FieldName
    //                                            && val.IsDefault);
    //            if (defaultAlias is not null)
    //            {
    //                defaultAlias.IsDefault = false;
    //                _context.Aliases.Update(defaultAlias);
    //            }
    //        }

    //        if (storedAlias is null)
    //            _context.Aliases.Add(alias);
    //        else
    //            _context.Aliases.Update(alias);

    //        await _context.SaveChangesAsync();
    //        return _mapper.Map<AliasViewModel>(alias);
    //    }

    //    /// <summary>
    //    /// Проверить существование алиаса.
    //    /// </summary>
    //    /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
    //    /// <param name="fieldName">Название поля.</param>
    //    /// <param name="value">Значение алиаса.</param>
    //    public Task<bool> Exists(long userspaceId, string fieldName, string value) =>
    //        _context.Aliases.AnyAsync(val => val.UserspaceId == userspaceId
    //                                         && val.FieldName == fieldName
    //                                         && val.Value == value);

    //    /// <summary>
    //    /// Получить алиасы для поля.
    //    /// </summary>
    //    /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
    //    /// <param name="fieldName">Название поля.</param>
    //    public async Task<IEnumerable<AliasViewModel>> Get(long userspaceId, string fieldName)
    //    {
    //        var aliases = await _context.Aliases.Where(val => val.UserspaceId == userspaceId && val.FieldName == fieldName).ToListAsync();
    //        return _mapper.Map<IEnumerable<AliasViewModel>>(aliases);
    //    }

    //    /// <summary>
    //    /// Получить все алиасы по умолчанию.
    //    /// </summary>
    //    /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
    //    public async Task<IEnumerable<AliasViewModel>> GetDefault(long userspaceId)
    //    {
    //        var aliases = await _context.Aliases.Where(val => val.UserspaceId == userspaceId && val.IsDefault).ToListAsync();
    //        return _mapper.Map<IEnumerable<AliasViewModel>>(aliases);
    //    }

    //    /// <summary>
    //    /// Получить все алиасы пространства.
    //    /// </summary>
    //    /// <param name="userspaceId">Идентификатор пространства пользователей.</param>
    //    public async  Task<IEnumerable<AliasViewModel>> GetAll(long userspaceId)
    //    {
    //        var aliases = await _context.Aliases.Where(val => val.UserspaceId == userspaceId).ToListAsync();
    //        return _mapper.Map<IEnumerable<AliasViewModel>>(aliases);
    //    }
    //}
}
