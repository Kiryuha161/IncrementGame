using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.DTOs.Common;
using IncrementGame.Server.Hubs;
using Serilog;

namespace IncrementGame.Server.Controllers
{
    /// <summary>
    /// Управление игровыми очками и состоянием игры.
    /// </summary>
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

        /// <summary>
        /// Получить текущее состояние игры (количество очков, уровень и т.д.).
        /// </summary>
        /// <returns>Объект GameStateDto с текущими показателями.</returns>
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

        /// <summary>
        /// Выполнить клик (заработать очки).
        /// Увеличивает счетчик и оповещает всех подключенных клиентов через SignalR.
        /// </summary>
        /// <returns>Обновленное состояние игры после клика.</returns>
        [HttpPost("click")]
        public async Task<ActionResult<ApiResult>> Click()
        {
            try
            {
                var dto = await _pointManager.ClickAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", dto);

                return Ok(ApiResult.Ok(dto, "Клик выполнен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при клике");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }

        /// <summary>
        /// Сохранить состояние игры (например, при покупке улучшений или выходе).
        /// После сохранения оповещает всех клиентов об обновлении через SignalR.
        /// </summary>
        /// <param name="state">Новое состояние игры для сохранения.</param>
        /// <returns>Статус операции.</returns>
        [HttpPost("state")]
        public async Task<ActionResult<ApiResult>> SaveState([FromBody] GameStateDto state)
        {
            try
            {
                await _pointManager.SaveStateAsync(state);
                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", state);

                return Ok(ApiResult.Ok(null, "Сохранено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при отправке SignalR");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }

        /// <summary>
        /// Начисляет пассивный доход на основе текущих улучшений игры.
        /// Вызывается автоматически таймером на клиенте (раз в текущий интервал игрока).
        /// После начисления оповещает всех подключенных клиентов об обновлении состояния.
        /// </summary>
        /// <returns>Обновленное состояние игры с новым количеством очков.</returns>
        [HttpPost("passive")]
        public async Task<ActionResult<ApiResult>> ProcessPassive()
        {
            try
            {
                var dto = await _pointManager.ProcessPassiveIncomeAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", dto);
                return Ok(ApiResult.Ok(dto, "Пассивный доход начислен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при начислении пассивного дохода");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }
    }
}