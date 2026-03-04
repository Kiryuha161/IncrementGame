using Incremental.Core.DTOs.Common;
using Incremental.Data.Domain;
using Incremental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Strategies.Effects
{
    public abstract class BaseEffectCalculator : IEffectCalculator
    {
        /// <summary>
        /// Тип улучшения, который калькулятор обрабатывает
        /// </summary>
        public abstract UpgradeTypes UpgradeType { get; }

        /// <summary>
        /// Получает сумму значений для данного типа улучшения
        /// </summary>
        protected long GetTotalValue(List<PlayerUpgrade> playerUpgrades)
        {
            return playerUpgrades
                .Where(pu => pu.Upgrade.UpgradeType == UpgradeType)
                .Sum(pu => pu.CurrentValue);
        }

        /// <summary>
        /// Основной метод расчета, который должны реализовать наследники
        /// </summary>
        public abstract void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result);
    }
}
