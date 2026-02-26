using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    /// <summary>
    /// DTO для прогресса улучшения игрока
    /// </summary>
    public class PlayerUpgradeDto
    {
        public int UpgradeId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public long CurrentValue { get; set; }
        public long NextValue { get; set; }
        public long NextPrice { get; set; }
    }
}
