using Incremental.Core.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers.Interfaces
{
    public interface IPointManager
    {
        /// <summary>
        /// Получить игровое состояние на текущий момент
        /// </summary>
        /// <returns></returns>
        Task<GameStateDto> GetStateAsync();


        /// <summary>
        /// Обрабатывает стандартный клик
        /// </summary>
        /// <returns>GameStateDto</returns>
        Task<GameStateDto> ClickAsync();

        /// <summary>
        /// Сохранение состояния игры
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task SaveStateAsync(GameStateDto state);

        /// <summary>
        /// Определение текущего пассивного дохода
        /// </summary>
        /// <returns></returns>
        Task<GameStateDto> ProcessPassiveIncomeAsync();
    }
}
