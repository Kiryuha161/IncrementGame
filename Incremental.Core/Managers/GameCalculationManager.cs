using Incremental.Core.Managers.Interfaces;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers
{
    public class GameCalculationManager : IGameCalculationManager
    {
        /// <summary>
        /// Базовая сила клика, если нет улучшений
        /// </summary>
        private const long BASE_CLICK_POWER = 1;

        /// <summary>
        /// Базовый интервал пассивного дохода в миллисекундах
        /// </summary>
        private const int BASE_PASSIVE_INTERVAL_MS = 5000;

        /// <summary>
        /// Минимальный интервал пассивного дохода
        /// </summary>
        private const int MIN_PASSIVE_INTERVAL_MS = 1000;

        /// <summary>
        /// Вычисление итоговой силы клика с учетом всех улучшений
        /// </summary>
        public long CalculateTotalClickPower(Point point)
        {
            if (point?.PlayerUpgrades == null || !point.PlayerUpgrades.Any())
                return BASE_CLICK_POWER;

            var upgradeBonus = point.PlayerUpgrades
                .Where(pu => pu.Upgrade != null && pu.Upgrade.UpgradeType == UpgradeTypes.ClickPower)
                .Sum(pu => pu.CurrentValue);

            // Если есть бонус от улучшений, возвращаем его, иначе базовое значение
            return upgradeBonus > 0 ? upgradeBonus : BASE_CLICK_POWER;
        }

        /// <summary>
        /// Вычисление пассивного дохода с учетом всех улучшений
        /// </summary>
        public long CalculateTotalPassiveIncome(Point point)
        {
            if (point?.PlayerUpgrades == null || !point.PlayerUpgrades.Any())
                return 0;

            return point.PlayerUpgrades
                .Where(pu => pu.Upgrade != null && pu.Upgrade.UpgradeType == UpgradeTypes.PassiveIncome)
                .Sum(pu => pu.CurrentValue);
        }

        /// <summary>
        /// Вычисление интервала пассивного дохода с учетом всех улучшений
        /// </summary>
        public int CalculateTotalPassiveInterval(Point point)
        {
            if (point?.PlayerUpgrades == null || !point.PlayerUpgrades.Any())
                return BASE_PASSIVE_INTERVAL_MS;

            var intervalBonus = point.PlayerUpgrades
                .Where(pu => pu.Upgrade != null && pu.Upgrade.UpgradeType == UpgradeTypes.PassiveInterval)
                .Sum(pu => pu.CurrentValue);

            // Интервал уменьшается с улучшениями, но не может быть меньше минимального
            var result = BASE_PASSIVE_INTERVAL_MS - (int)intervalBonus;
            return Math.Max(MIN_PASSIVE_INTERVAL_MS, result);
        }

        /// <summary>
        /// Вычисление всех значений сразу (для оптимизации)
        /// </summary>
        public (long ClickPower, long PassiveIncome, int PassiveInterval) CalculateAll(Point point)
        {
            return (
                CalculateTotalClickPower(point),
                CalculateTotalPassiveIncome(point),
                CalculateTotalPassiveInterval(point)
            );
        }

        /// <summary>
        /// Вычисление количества прошедших тиков пассивного дохода
        /// </summary>
        public int CalculatePassiveTicks(Point point, DateTime now)
        {
            if (point?.LastPassiveTick == null)
                return 0;

            var interval = CalculateTotalPassiveInterval(point);
            if (interval <= 0) return 0;

            var timeSinceLastTick = (now - point.LastPassiveTick.Value).TotalMilliseconds;
            return (int)(timeSinceLastTick / interval);
        }

        /// <summary>
        /// Вычисление, активен ли пассивный доход
        /// </summary>
        public bool IsPassiveIncomeActive(Point point)
        {
            return CalculateTotalPassiveIncome(point) > 0 &&
                   CalculateTotalPassiveInterval(point) > 0;
        }

        /// <summary>
        /// Вычисление потенциального дохода за указанное время
        /// </summary>
        public long CalculatePotentialPassiveIncome(Point point, TimeSpan timeSpan)
        {
            var income = CalculateTotalPassiveIncome(point);
            var interval = CalculateTotalPassiveInterval(point);

            if (income <= 0 || interval <= 0) return 0;

            var ticks = (int)(timeSpan.TotalMilliseconds / interval);
            return income * ticks;
        }
    }
}