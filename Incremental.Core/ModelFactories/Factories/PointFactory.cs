using Incremental.Core.DTOs.Common;
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
        public GameStateDto PrepareGameStateDto(Point point)
        {
            if (point == null)
            {
                return null;
            }

            GameStateDto dto = new GameStateDto
            {
                Value = point.Amount,
                ClickPower = point.ClickPower
            };

            return dto;
        }
    }
}
