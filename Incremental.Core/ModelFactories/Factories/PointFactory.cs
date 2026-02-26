using Incremental.Core.DTOs.Common;
using Incremental.Core.Managers.Interfaces;
using Incremental.Core.ModelFactories.Interfaces;
using Incremental.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.ModelFactories.Factories
{
    public class PointFactory : IPointFactory
    {
        private readonly IGameCalculationManager _calculations;

        public PointFactory(IGameCalculationManager calculations)
        {
            _calculations = calculations;
        }
        
        public GameStateDto PrepareGameStateDto(Point point)
        {
            if (point == null)
                return PrepareDefaultGameStateDto();

            return new GameStateDto
            {
                Value = point.Amount,
                ClickPower = _calculations.CalculateTotalClickPower(point),
                PassiveIncome = _calculations.CalculateTotalPassiveIncome(point),
                PassiveInterval = _calculations.CalculateTotalPassiveInterval(point)
            };
        }

        public GameStateDto PrepareDefaultGameStateDto()
        {
            return new GameStateDto
            {
                Value = 0,
                ClickPower = 1,
                PassiveIncome = 0,
                PassiveInterval = 0
            };
        }
    }
}
