using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.DTOs.Common
{
    public class GameStateDto
    {
        /// <summary>
        /// Количество очков
        /// </summary>
        public long Value { get; set; }
        
        /// <summary>
        /// Количество очков, которые будут прибавляться к общему числу очков при клике
        /// </summary>
        public long ClickPower { get; set; }
        
        /// <summary>
        /// Пассивный доход, добавляющийся к общим очкам раз в интервал
        /// </summary>
        public long PassiveIncome { get; set; }
        
        /// <summary>
        /// Интервал, по истечению которого, добавляется пассивный доход к общим очкам
        /// </summary>
        public int PassiveInterval { get; set; }
    }
}
