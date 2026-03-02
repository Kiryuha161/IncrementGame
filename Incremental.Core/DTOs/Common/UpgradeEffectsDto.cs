using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    public class UpgradeEffectsDto
    {
        public long ClickPower { get; set; }
        public long PassiveIncome { get; set; }
        public int PassiveInterval { get; set; }
    }
}
