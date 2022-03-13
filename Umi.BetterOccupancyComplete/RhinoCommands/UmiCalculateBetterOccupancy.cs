using Rhino;
using Rhino.Commands;
using System.Linq;
using System.Runtime.InteropServices;
using Umi.RhinoServices;
using Umi.RhinoServices.Context;

namespace Umi.BetterOccupancyComplete.RhinoCommands
{
    [Guid("d21cf132-592d-455d-8e73-6f179384e4f2")]
    public class UmiCalculateBetterOccupancy : UmiCommand
    {
        public override string EnglishName => nameof(UmiCalculateBetterOccupancy);

        public override Result Run(RhinoDoc doc, UmiContext context, RunMode mode)
        {
            foreach (var building in context.Buildings.All)
            {
                var buildingOccupancy = building.OccupantDensity * building.GrossFloorArea;
                RhinoApp.WriteLine($"Building: {building.Name}: {buildingOccupancy} occupant(s)");
            }

            return Result.Success;
        }
    }
}
