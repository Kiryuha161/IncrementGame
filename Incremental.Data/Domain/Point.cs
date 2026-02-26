using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{
    /// <summary>
    /// Класс состояния для текущего пользователя
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Идентификатор набора очков для текущего пользователя
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Количество очков для текущего пользователя
        /// </summary>
        public long Amount { get; set; }    

        /// <summary>
        /// Дата последнего срабатывания интервала для пассивного дохода
        /// </summary>
        public DateTime? LastPassiveTick { get; set; }

        /// <summary>
        /// Навигационное свойство: все улучшения этого игрока
        /// </summary>
        public ICollection<PlayerUpgrade> PlayerUpgrades { get; set; } = new List<PlayerUpgrade>();
    }
}
