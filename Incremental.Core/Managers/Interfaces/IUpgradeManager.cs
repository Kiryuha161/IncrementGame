using Incremental.Core.DTOs.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Incremental.Core.Managers.Interfaces
{
    public interface IUpgradeManager
    {
        /// <summary>
        /// Получает текущие эффекты от всех улучшений игрока
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <returns>Dto с силой клика, пассивным доходом и интервалом</returns>
        Task<UpgradeEffectsDto> GetCurrentEffectsAsync(int pointsId);

        /// <summary>
        /// Получает список всех доступных улучшений с текущим прогрессом игрока
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <returns>Список улучшений с текущим уровнем, ценой и значениями</returns>
        Task<List<UpgradeDto>> GetAvailableUpgradesAsync(int pointsId);

        /// <summary>
        /// Получает список купленных улучшений игрока
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <returns>Список улучшений с текущим уровнем и характеристиками</returns>
        Task<List<PlayerUpgradeDto>> GetPlayerUpgradesAsync(int pointsId);

        /// <summary>
        /// Покупает улучшение для игрока
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <param name="upgradeId">Идентификатор улучшения</param>
        /// <returns>Обновленные данные о купленном улучшении</returns>
        /// <exception cref="Exception">Если игрок не найден, улучшение не найдено или недостаточно очков</exception>
        Task<PlayerUpgradeDto> BuyUpgradeAsync(int pointsId, int upgradeId);

        /// <summary>
        /// Проверяет, может ли игрок купить улучшение
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <param name="upgradeId">Идентификатор улучшения</param>
        /// <returns>True если достаточно очков, иначе false</returns>
        Task<bool> CanAffordUpgradeAsync(int pointsId, int upgradeId);

        /// <summary>
        /// Получает цену следующего уровня улучшения
        /// </summary>
        /// <param name="pointsId">Идентификатор игрока</param>
        /// <param name="upgradeId">Идентификатор улучшения</param>
        /// <returns>Цена следующего уровня</returns>
        /// <exception cref="Exception">Если улучшение не найдено</exception>
        Task<long> GetNextUpgradePriceAsync(int pointsId, int upgradeId);
    }
}