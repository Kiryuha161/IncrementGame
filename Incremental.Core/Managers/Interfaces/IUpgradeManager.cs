using Incremental.Core.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers.Interfaces
{
    public interface IUpgradeManager
    {
        /// <summary>
        /// Получить все доступные улучшения для игрока
        /// </summary>
        Task<ApiResult<List<UpgradeDto>>> GetAvailableUpgradesAsync(int pointsId);

        /// <summary>
        /// Получить текущий прогресс улучшений игрока
        /// </summary>
        Task<ApiResult<List<PlayerUpgradeDto>>> GetPlayerUpgradesAsync(int pointsId);

        /// <summary>
        /// Купить улучшение (повысить уровень)
        /// </summary>
        Task<ApiResult<PlayerUpgradeDto>> BuyUpgradeAsync(int pointsId, int upgradeId);

        /// <summary>
        /// Проверить, может ли игрок купить улучшение
        /// </summary>
        Task<ApiResult<bool>> CanAffordUpgradeAsync(int pointsId, int upgradeId);

        /// <summary>
        /// Получить стоимость следующего уровня улучшения
        /// </summary>
        Task<ApiResult<long>> GetNextUpgradePriceAsync(int pointsId, int upgradeId);
    }
}
