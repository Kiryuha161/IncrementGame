using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incremental.Data.Domain
{
    public class Point
    {
        public int Id { get; set; }
        public long Amount { get; set; }
        public long ClickPower { get; set; } = 1;
    }
}
