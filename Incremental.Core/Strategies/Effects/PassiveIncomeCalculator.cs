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
    public class PassiveIncomeCalculator : BaseEffectCalculator
    {
        public override UpgradeTypes UpgradeType => UpgradeTypes.PassiveIncome;

        public override void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result)
        {
            result.PassiveIncome = GetTotalValue(playerUpgrades);
        }
    }
}
