using Incremental.Core.DTOs.Common;
using Incremental.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Core.ModelFactories.Interfaces
{
    public interface IPointFactory
    {
        GameStateDto PrepareGameStateDto(Point point);
    }
}
