using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{
    /// <summary>
    /// Достижения, которые дают бонусы к улучшениям
    /// </summary>
    public class Achievement
    {
        /// <summary>
        /// Идентификатор достижения
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название достижения
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Описание достижения и условия его получения
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// JSON с конфигурацией бонусов
        /// Пример: 
        /// {
        ///   "clickPowerMultiplier": 1.1,
        ///   "passiveIncomeMultiplier": 1.15,
        ///   "discountBonus": 5,
        ///   "powerBoostBonus": 10
        /// }
        /// </summary>
        public string JsonContent { get; set; }

        /// <summary>
        /// Навигационное свойство: все записи PlayerAchievement, использующие это достижение
        /// </summary>
        public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
