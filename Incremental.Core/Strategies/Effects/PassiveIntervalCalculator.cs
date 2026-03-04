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
    /// Калькулятор интервала для пассивного дохода
    /// </summary>
    public class PassiveIntervalCalculator : BaseEffectCalculator
    {
        public override UpgradeTypes UpgradeType => UpgradeTypes.PassiveInterval;

        public override void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result)
        {
            var intervalBonus = GetTotalValue(playerUpgrades);

            result.PassiveInterval = Math.Max(1000, 5000 - (int)intervalBonus);
        }
    }
}
