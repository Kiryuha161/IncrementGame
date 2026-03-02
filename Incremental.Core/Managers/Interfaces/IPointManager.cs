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
        /// Получить текущее количество очков
        /// </summary>
        Task<long> GetCurrentAmountAsync();

        /// <summary>
        /// Выполнить клик с указанной силой
        /// </summary>
        Task<long> ClickAsync(long clickPower);

        /// <summary>
        /// Сохранить количество очков
        /// </summary>
        Task SaveAmountAsync(long amount);

        /// <summary>
        /// Обработать пассивный доход с указанными параметрами
        /// </summary>
        Task<long> ProcessPassiveIncomeAsync(long passiveIncome, int passiveInterval);
    }
}