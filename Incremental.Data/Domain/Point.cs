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
        /// Количество очков, которые будут прибавляться к общему числу очков при клике
        /// </summary>
        public long ClickPower { get; set; } = 1;
        
        /// <summary>
        /// Пассивный доход, добавляющийся к общим очкам раз в интервал
        /// </summary>
        public long PassiveIncome { get; set; } = 0;        

        /// <summary>
        /// Интервал, по истечению которого, добавляется пассивный доход к общим очкам
        /// </summary>
        public int PassiveInterval { get; set; } = 5000;       

        /// <summary>
        /// Дата последнего срабатывания интервала для пассивного дохода
        /// </summary>
        public DateTime LastPassiveTick { get; set; }
    }
}
