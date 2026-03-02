using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using IncrementGame.Server.Hubs;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IncrementGame.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpgradeController : ControllerBase
    {
        private readonly IUpgradeManager _upgradeManager;
        private readonly IPointManager _pointManager;
        private readonly IHubContext<GameHub> _hubContext;

        /// <summary>
        /// Контроллер для управления улучшениями игрока
        /// </summary>
        public UpgradeController(
            IUpgradeManager upgradeManager,
            IPointManager pointManager,
            IHubContext<GameHub> hubContext)
        {
            _upgradeManager = upgradeManager;
            _pointManager = pointManager;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Получает текущие эффекты от всех улучшений игрока
        /// </summary>
        /// <returns>Сила клика, пассивный доход и интервал</returns>
        [HttpGet("effects")]
        public async Task<ActionResult<ApiResult<UpgradeEffectsDto>>> GetEffects()
        {
            try
            {
                int pointsId = 1;
                var result = await _upgradeManager.GetCurrentEffectsAsync(pointsId);
                return Ok(ApiResult<UpgradeEffectsDto>.Ok(result, "Эффекты получены"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении эффектов");
                return StatusCode(500, ApiResult<UpgradeEffectsDto>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Получает список всех доступных улучшений с текущим прогрессом игрока
        /// </summary>
        /// <returns>Список улучшений с уровнями, ценами и значениями</returns>
        [HttpGet("available")]
        public async Task<ActionResult<ApiResult<List<UpgradeDto>>>> GetAvailableUpgrades()
        {
            try
            {
                int pointsId = 1;
                var result = await _upgradeManager.GetAvailableUpgradesAsync(pointsId);
                return Ok(ApiResult<List<UpgradeDto>>.Ok(result, "Доступные улучшения получены"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении списка улучшений");
                return StatusCode(500, ApiResult<List<UpgradeDto>>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Получает список купленных улучшений игрока
        /// </summary>
        /// <returns>Список улучшений с текущими уровнями</returns>
        [HttpGet("player")]
        public async Task<ActionResult<ApiResult<List<PlayerUpgradeDto>>>> GetPlayerUpgrades()
        {
            try
            {
                int pointsId = 1;
                var result = await _upgradeManager.GetPlayerUpgradesAsync(pointsId);
                return Ok(ApiResult<List<PlayerUpgradeDto>>.Ok(result, "Улучшения игрока получены"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении прогресса улучшений");
                return StatusCode(500, ApiResult<List<PlayerUpgradeDto>>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Покупает улучшение для игрока
        /// </summary>
        /// <param name="upgradeId">ID улучшения</param>
        /// <returns>Обновленные данные о купленном улучшении</returns>
        [HttpPost("buy/{upgradeId}")]
        public async Task<ActionResult<ApiResult<PlayerUpgradeDto>>> BuyUpgrade(int upgradeId)
        {
            try
            {
                int pointsId = 1;

                var result = await _upgradeManager.BuyUpgradeAsync(pointsId, upgradeId);

                // Отправляем обновления всем клиентам
                var amount = await _pointManager.GetCurrentAmountAsync();
                var effects = await _upgradeManager.GetCurrentEffectsAsync(pointsId);

                await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", new
                {
                    value = amount,
                    clickPower = effects.ClickPower,
                    passiveIncome = effects.PassiveIncome,
                    passiveInterval = effects.PassiveInterval
                });

                return Ok(ApiResult<PlayerUpgradeDto>.Ok(result, "Улучшение куплено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при покупке улучшения {UpgradeId}", upgradeId);
                return StatusCode(500, ApiResult<PlayerUpgradeDto>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Проверяет, может ли игрок купить улучшение
        /// </summary>
        /// <param name="upgradeId">ID улучшения</param>
        /// <returns>true если достаточно очков, иначе false</returns>
        [HttpGet("can-buy/{upgradeId}")]
        public async Task<ActionResult<ApiResult<bool>>> CanAffordUpgrade(int upgradeId)
        {
            try
            {
                int pointsId = 1;
                var result = await _upgradeManager.CanAffordUpgradeAsync(pointsId, upgradeId);
                return Ok(ApiResult<bool>.Ok(result, "Улучшение может быть куплено"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при проверке возможности покупки");
                return StatusCode(500, ApiResult<bool>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Получает цену следующего уровня улучшения
        /// </summary>
        /// <param name="upgradeId">ID улучшения</param>
        /// <returns>Цена следующего уровня</returns>
        [HttpGet("price/{upgradeId}")]
        public async Task<ActionResult<ApiResult<long>>> GetNextPrice(int upgradeId)
        {
            try
            {
                int pointsId = 1;
                var result = await _upgradeManager.GetNextUpgradePriceAsync(pointsId, upgradeId);
                return Ok(ApiResult<long>.Ok(result, "Получена следующая цена"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении цены улучшения");
                return StatusCode(500, ApiResult<long>.Fail("Внутренняя ошибка сервера"));
            }
        }
    }
}