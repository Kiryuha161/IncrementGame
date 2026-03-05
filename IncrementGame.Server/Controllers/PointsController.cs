using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.DTOs.Common;
using Incremental.Core.DTOs.Request;
using IncrementGame.Server.Hubs;
using Serilog;
using System;
using System.Threading.Tasks;
using Incremental.Core.Managers;

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
        private readonly IUpgradeManager _upgradeManager;

        public PointsController(
            IPointManager pointManager,
            IHubContext<GameHub> hubContext,
            IUpgradeManager upgradeManager)
        {
            _pointManager = pointManager;
            _hubContext = hubContext;
            _upgradeManager = upgradeManager;
        }

        /// <summary>
        /// Получает текущее количество очков
        /// </summary>
        /// <returns>Количество очков</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult<long>>> Get()
        {
            try
            {
                var amount = await _pointManager.GetCurrentAmountAsync();
                return Ok(ApiResult<long>.Ok(amount, "Количество очков получено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении количества очков");
                return StatusCode(500, ApiResult<long>.Fail("Ошибка сервера"));
            }
        }

        /// <summary>
        /// Выполняет клик с указанной силой
        /// </summary>
        /// <param name="request">Объект с силой клика</param>
        /// <returns>Новое количество очков</returns>
        [HttpPost("click")]
        public async Task<ActionResult<ApiResult<long>>> Click([FromBody] ClickRequest request)
        {
            try
            {
                var newAmount = await _pointManager.ClickAsync(request.ClickPower);

                await _hubContext.Clients.All.SendAsync("ReceiveAmountUpdate", newAmount);

                return Ok(ApiResult<long>.Ok(newAmount, "Клик выполнен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при клике");
                return StatusCode(500, ApiResult<long>.Fail("Ошибка сервера"));
            }
        }

        /// <summary>
        /// Сохраняет указанное количество очков
        /// </summary>
        /// <param name="amount">Новое количество очков</param>
        [HttpPost("state")]
        public async Task<ActionResult<ApiResult>> SaveState([FromBody] long amount)
        {
            try
            {
                await _pointManager.SaveAmountAsync(amount);
                await _hubContext.Clients.All.SendAsync("ReceiveAmountUpdate", amount);

                return Ok(ApiResult.Ok(null, "Сохранено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сохранении");
                return StatusCode(500, ApiResult.Fail("Ошибка сервера"));
            }
        }

        /// <summary>
        /// Начисляет пассивный доход на основе текущих улучшений
        /// </summary>
        /// <returns>Новое количество очков</returns>
        [HttpPost("passive")]
        public async Task<ActionResult<ApiResult<long>>> ProcessPassive()
        {
            try
            {
                int pointsId = 1; // Временное решение

                // Получаем параметры пассивного дохода из UpgradeManager
                var effects = await _upgradeManager.GetCurrentEffectsAsync(pointsId);

                // Передаем их в PointManager для начисления
                var newAmount = await _pointManager.ProcessPassiveIncomeAsync(
                    effects.PassiveIncome,
                    effects.PassiveInterval
                );

                await _hubContext.Clients.All.SendAsync("ReceiveAmountUpdate", newAmount);

                return Ok(ApiResult<long>.Ok(newAmount, "Пассивный доход начислен"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при начислении пассивного дохода");
                return StatusCode(500, ApiResult<long>.Fail("Ошибка сервера"));
            }
        }
    }
}