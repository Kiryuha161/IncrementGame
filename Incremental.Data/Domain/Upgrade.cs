using Incremental.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{
    /// <summary>
    /// Таблица хранящая общие данные улучшений
    /// </summary>
    public class Upgrade
    {
        /// <summary>
        /// Идентификатор улучшения
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Название улучшения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание улучшения для подсказок в UI
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Базовое значение (long для очков)
        /// </summary>
        public long BaseValue { get; set; }

        /// <summary>
        /// Множитель значения за уровень (decimal для точности)
        /// Для клика: 1.5 (увеличение)
        /// Для интервала: 0.9 (уменьшение)
        /// </summary>
        public decimal ValueMultiplier { get; set; } = 1.0m;

        /// <summary>
        /// Базовая цена (long для очков)
        /// </summary>
        public long BasePrice { get; set; }

        /// <summary>
        /// Множитель цены за уровень
        /// </summary>
        public decimal PriceMultiplier { get; set; } = 1.5m;

        /// <summary>
        /// Тип улучшения
        /// </summary>
        public UpgradeTypes UpgradeType { get; set; }

        /// <summary>
        /// Навигационное свойство: все записи PlayerUpgrade, использующие это улучшение
        /// </summary>
        public ICollection<PlayerUpgrade> PlayerUpgrades { get; set; } = new List<PlayerUpgrade>();
    }
}
