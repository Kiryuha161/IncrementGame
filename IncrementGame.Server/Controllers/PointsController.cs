using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.DTOs.Common;
using IncrementGame.Server.Hubs;
using Serilog;

namespace IncrementGame.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointsController : ControllerBase
    {
        private readonly IPointManager _pointManager;
        private readonly IHubContext<GameHub> _hubContext;

        public PointsController(
            IPointManager pointManager,
            IHubContext<GameHub> hubContext)
        {
            _pointManager = pointManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> Get()
        {
            try
            {
                var dto = await _pointManager.GetStateAsync();
                return Ok(ApiResult.Ok(dto, "Состояние получено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении состояния");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }

        [HttpPost("click")]
        public async Task<ActionResult<ApiResult>> Click()
        {
            try
            {
                var dto = await _pointManager.ClickAsync();

                // 👇 ОТПРАВКА SIGNALR ПОСЛЕ КЛИКА
                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", dto);
                Log.Information("📢 Отправка SignalR после клика: Value={Value}", dto.Value);

                return Ok(ApiResult.Ok(dto, "Клик выполнен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при клике");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }

        [HttpPost("state")]
        public async Task<ActionResult<ApiResult>> SaveState([FromBody] GameStateDto state)
        {
            try
            {
                await _pointManager.SaveStateAsync(state);

                Log.Information("📢 Отправка SignalR после сохранения: Value={Value}", state.Value);

                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", state);
                Log.Information("✅ SignalR успешно отправлен всем клиентам");

                return Ok(ApiResult.Ok(null, "Сохранено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Ошибка при отправке SignalR");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }
    }
}