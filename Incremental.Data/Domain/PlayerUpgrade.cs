using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{
    /// <summary>
    /// Связующая таблица, хранящая прогресс улучшений для конкретного игрока
    /// </summary>
    public class PlayerUpgrade
    {
        /// <summary>
        /// Уникальный идентификатор записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор игрока (внешний ключ к таблице Points)
        /// </summary>
        public int PointsId { get; set; }

        /// <summary>
        /// Идентификатор улучшения (внешний ключ к таблице Upgrade)
        /// </summary>
        public int UpgradeId { get; set; }

        /// <summary>
        /// Текущий уровень прокачки данного улучшения
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с таблицей Points
        /// </summary>
        [ForeignKey(nameof(PointsId))]
        public Point Points { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с таблицей Upgrade
        /// </summary>
        [ForeignKey(nameof(UpgradeId))]
        public Upgrade Upgrade { get; set; }

        /// <summary>
        /// Текущее значение улучшения (вычисляется на основе базового значения и уровня)
        /// </summary>
        public long CurrentValue => Upgrade.BaseValue * Level;

        /// <summary>
        /// Цена следующего уровня улучшения (вычисляется на основе базовой цены, множителя и текущего уровня)
        /// </summary>
        public long NextPrice => (long)(Upgrade.BasePrice * Math.Pow((double)Upgrade.PriceMultiplier, Level));
    }
}