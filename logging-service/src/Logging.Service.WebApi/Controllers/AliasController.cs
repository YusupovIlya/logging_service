using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Logging.Server.Models.StreamData.Api;
using Logging.Server.Service.StreamData.Services;
using Monq.Core.MvcExtensions.Validation;
using Monq.Core.MvcExtensions.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Monq.Core.BasicDotNetMicroservice.AuthConstants.AuthorizationScopes;

namespace Logging.Server.Service.StreamData.Controllers
{
    /// <summary>
    /// Контроллер алиасов.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/cl/stream-data/aliases")]
    public class AliasController : ControllerBase//, IAsyncActionFilter
    {
        //readonly IAliasRepository _aliasRepository;
        //readonly IStringLocalizer _localizer;

        //long _userspaceId = -1;

        ///// <summary>
        ///// Инициализирует новый экземпляр класса <see cref="AliasController"/>.
        ///// </summary>
        ///// <param name="aliasRepository">Репозиторий алиасов./</param>
        ///// <param name="localizer">Сервис локализации для текста ошибок.</param>
        //public AliasController(IAliasRepository aliasRepository, IStringLocalizer localizer)
        //{
        //    _aliasRepository = aliasRepository;
        //    _localizer = localizer;
        //}

        ///// <inheritdoc />
        //[NonAction]
        //public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{
        //    _userspaceId = context.HttpContext.Request.Userspace();
        //    await next();
        //}

        ///// <summary>
        ///// Получить все алиасы для заданного пространства пользователей.
        ///// </summary>
        ///// <returns>Коллекция алиасов пространства пользователей.</returns>
        ///// <response code="200">Запрос успешно исполнен.</response>
        //[Authorize(Read)]
        //[HttpGet]
        //[ValidateActionParameters]
        //[Produces(typeof(IEnumerable<AliasViewModel>))]
        //public Task<IEnumerable<AliasViewModel>> GetAll() => _aliasRepository.GetAll(_userspaceId);

        ///// <summary>
        ///// Создать новый алиас.
        ///// </summary>
        ///// <param name="aliasViewModel">Модель создаваемого клиента (см. <see cref="AliasViewModel"/>).</param>
        ///// <returns>Созданный клиент (см. <see cref="AliasViewModel"/>).</returns>
        ///// <response code="200">Запрос успешно исполнен.</response>
        ///// <response code="400">Алиас уже существует.</response>
        ///// <response code="403">Недостаточно прав.</response>
        //[Authorize(Write)]
        //[HttpPost]
        //[ValidateActionParameters]
        //public async Task<ActionResult<AliasViewModel>> Create(AliasViewModel aliasViewModel)
        //{
        //    if (!User.IsUserspaceAdmin(_userspaceId))
        //        return Forbid();

        //    var aliasExists = await _aliasRepository.Exists(aliasViewModel.UserspaceId, aliasViewModel.FieldName, aliasViewModel.Value);
        //    if (aliasExists)
        //        return BadRequest(new ErrorResponseViewModel(_localizer["alias_already_exists"]));

        //    var storedModel = await _aliasRepository.Upsert(aliasViewModel);
        //    return CreatedAtAction(nameof(GetAll), storedModel);
        //}

        ///// <summary>
        ///// Обновить существующий алиас.
        ///// </summary>
        ///// <param name="aliasViewModel">Модель обновления (см. <see cref="AliasViewModel"/>).</param>
        ///// <returns>Алиас с обновлёнными свойствами (см. <see cref="ActionResult"/>).</returns>
        ///// <response code="200">Запрос успешно исполнен.</response>
        ///// <response code="400">Алиас не найден.</response>
        ///// <response code="403">Недостаточно прав.</response>
        //[Authorize(Write)]
        //[HttpPut]
        //[ValidateActionParameters]
        //public async Task<ActionResult> Update(AliasViewModel aliasViewModel)
        //{
        //    if (!User.IsUserspaceAdmin(_userspaceId))
        //        return Forbid();

        //    var aliasExists = await _aliasRepository.Exists(_userspaceId, aliasViewModel.FieldName, aliasViewModel.Value);
        //    if (!aliasExists)
        //        return BadRequest(new ErrorResponseViewModel(_localizer["alias_is_not_found"]));

        //    var storedModel = await _aliasRepository.Upsert(aliasViewModel);
        //    return Ok(storedModel);
        //}
    }
}
