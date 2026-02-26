using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Interfaces
{
    public interface IDataInitializer
    {
        Task InitializeAsync();
    }
}
