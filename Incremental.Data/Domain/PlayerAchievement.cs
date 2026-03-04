using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{

    /// <summary>
    /// Связующая таблица, хранящая полученные достижения для конкретного игрока
    /// </summary>
    public class PlayerAchievement
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
        /// Идентификатор достижения (внешний ключ к таблице Achievement)
        /// </summary>
        public int AchievementId { get; set; }

        /// <summary>
        /// Дата и время получения достижения
        /// </summary>
        public DateTime UnlockedAt { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с таблицей Points
        /// </summary>
        [ForeignKey(nameof(PointsId))]
        public Point Point { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с таблицей Achievement
        /// </summary>
        [ForeignKey(nameof(AchievementId))]
        public Achievement Achievement { get; set; }
    }
}
