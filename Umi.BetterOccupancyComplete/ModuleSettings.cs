using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umi.BetterOccupancyComplete
{
    internal class ModuleSettings
    {
        public Dictionary<string, double> BetterOccupantDensities { get; set; } = new Dictionary<string, double>();
    }
}
