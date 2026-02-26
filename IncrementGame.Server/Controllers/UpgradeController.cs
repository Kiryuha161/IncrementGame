using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using IncrementGame.Server.Hubs;
using Serilog;
using System;
using System.Threading.Tasks;

namespace IncrementGame.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpgradeController : ControllerBase
    {
        private readonly IUpgradeManager _upgradeManager;
        private readonly IPointManager _pointManager;
        private readonly IHubContext<GameHub> _hubContext;

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
        /// Получить все доступные улучшения для игрока
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<ApiResult<List<UpgradeDto>>>> GetAvailableUpgrades()
        {
            try
            {
                // TODO: получить реальный PointsId из авторизации
                int pointsId = 1; // Временное решение

                var result = await _upgradeManager.GetAvailableUpgradesAsync(pointsId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении списка улучшений");
                return StatusCode(500, ApiResult<List<UpgradeDto>>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Получить текущий прогресс улучшений игрока
        /// </summary>
        [HttpGet("player")]
        public async Task<ActionResult<ApiResult<List<PlayerUpgradeDto>>>> GetPlayerUpgrades()
        {
            try
            {
                int pointsId = 1; // Временное решение

                var result = await _upgradeManager.GetPlayerUpgradesAsync(pointsId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении прогресса улучшений");
                return StatusCode(500, ApiResult<List<PlayerUpgradeDto>>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Купить улучшение
        /// </summary>
        [HttpPost("buy/{upgradeId}")]
        public async Task<ActionResult<ApiResult<PlayerUpgradeDto>>> BuyUpgrade(int upgradeId)
        {
            try
            {
                int pointsId = 1; // Временное решение

                var result = await _upgradeManager.BuyUpgradeAsync(pointsId, upgradeId);

                if (result.Success)
                {
                    // После успешной покупки отправляем обновленное состояние всем клиентам
                    var gameState = await _pointManager.GetStateAsync();
                    await _hubContext.Clients.All.SendAsync("ReceiveGameStateUpdate", gameState);

                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при покупке улучшения {UpgradeId}", upgradeId);
                return StatusCode(500, ApiResult<PlayerUpgradeDto>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Проверить, может ли игрок купить улучшение
        /// </summary>
        [HttpGet("can-buy/{upgradeId}")]
        public async Task<ActionResult<ApiResult<bool>>> CanAffordUpgrade(int upgradeId)
        {
            try
            {
                int pointsId = 1; // Временное решение

                var result = await _upgradeManager.CanAffordUpgradeAsync(pointsId, upgradeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при проверке возможности покупки");
                return StatusCode(500, ApiResult<bool>.Fail("Внутренняя ошибка сервера"));
            }
        }

        /// <summary>
        /// Получить цену следующего уровня улучшения
        /// </summary>
        [HttpGet("price/{upgradeId}")]
        public async Task<ActionResult<ApiResult<long>>> GetNextPrice(int upgradeId)
        {
            try
            {
                int pointsId = 1; // Временное решение

                var result = await _upgradeManager.GetNextUpgradePriceAsync(pointsId, upgradeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении цены улучшения");
                return StatusCode(500, ApiResult<long>.Fail("Внутренняя ошибка сервера"));
            }
        }
    }
}