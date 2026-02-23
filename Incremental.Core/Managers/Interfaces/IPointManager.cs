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
        Task<GameStateDto> GetStateAsync();
        Task<GameStateDto> ClickAsync();
        Task SaveStateAsync(GameStateDto state);
    }
}
