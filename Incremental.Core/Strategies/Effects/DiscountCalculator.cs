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
    /// <summary>
    /// Калькулятор скидки на все улучшения
    /// </summary>
    public class DiscountCalculator : BaseEffectCalculator
    {
        public override UpgradeTypes UpgradeType => UpgradeTypes.DiscountAll;

        public override void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result)
        {
            var total = GetTotalValue(playerUpgrades);
            result.DiscountPercent = Math.Min(0.5m, total / 100m);
        }
    }
}
