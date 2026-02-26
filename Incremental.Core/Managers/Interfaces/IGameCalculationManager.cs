using Incremental.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.Managers.Interfaces
{
    public interface IGameCalculationManager
    {
        long CalculateTotalClickPower(Point point);
        long CalculateTotalPassiveIncome(Point point);
        int CalculateTotalPassiveInterval(Point point);
    }
}
