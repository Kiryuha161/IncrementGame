using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace IncrementGame.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IPointManager _pointManager;

        public PointsController(IPointManager pointManager)
        {
            _pointManager = pointManager;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> Get()
        {
            try
            {
                var dto = await _pointManager.GetStateAsync();
                return Ok(ApiResult.Ok(dto, "Состояние игры получено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении состояния игры");
                return StatusCode(500, ApiResult.Fail("Ошибка при получении состояния игры"));
            }
        }

        [HttpPost("click")]
        public async Task<ActionResult<ApiResult>> Click()
        {
            try
            {
                var dto = await _pointManager.ClickAsync();
                return Ok(ApiResult.Ok(dto, "Клик выполнен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при выполнении клика");
                return StatusCode(500, ApiResult.Fail("Ошибка при выполнении клика"));
            }
        }

        [HttpPost("state")]
        public async Task<ActionResult<ApiResult>> SaveState([FromBody] GameStateDto state)
        {
            try
            {
                await _pointManager.SaveStateAsync(state);
                return Ok(ApiResult.Ok(null, "Состояние сохранено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сохранении состояния");
                return StatusCode(500, ApiResult.Fail("Ошибка при сохранении состояния"));
            }
        }
    }
}