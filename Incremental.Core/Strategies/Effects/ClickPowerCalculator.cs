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
    /// Калькулятор для силы клика
    /// </summary>
    public class ClickPowerCalculator : BaseEffectCalculator
    {
        public override UpgradeTypes UpgradeType => UpgradeTypes.ClickPower;

        public override void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result)
        {
            var total = GetTotalValue(playerUpgrades);

            result.ClickPower = total > 0 ? total : 1;
        }
    }
}
