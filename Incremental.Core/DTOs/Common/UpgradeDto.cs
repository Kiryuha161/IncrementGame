using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    /// <summary>
    /// DTO для информации об улучшении
    /// </summary>
    public class UpgradeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public long CurrentValue { get; set; }
        public long NextValue { get; set; }
        public long CurrentPrice { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public string UpgradeType { get; set; }
        public string Icon { get; set; } // Для UI
        public long? OriginalPrice { get; set; }
    }
}
