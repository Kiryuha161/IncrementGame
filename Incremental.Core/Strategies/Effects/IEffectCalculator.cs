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
    public interface IEffectCalculator
    {
        UpgradeTypes UpgradeType { get; }
        void Calculate(List<PlayerUpgrade> playerUpgrades, UpgradeEffectsDto result);
    }
}
