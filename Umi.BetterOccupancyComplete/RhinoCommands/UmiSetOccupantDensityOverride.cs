using Rhino;
using Rhino.Commands;
using Rhino.Input;
using System.Runtime.InteropServices;
using Umi.RhinoServices;
using Umi.RhinoServices.Context;

namespace Umi.BetterOccupancyComplete.RhinoCommands
{
    [Guid("d4ebe3e0-2110-4f31-8c1a-13129ebeee75")]
    public class UmiSetOccupantDensityOverride : UmiCommand
    {
        public override string EnglishName => nameof(UmiSetOccupantDensityOverride);

        public override Result Run(RhinoDoc doc, UmiContext context, RunMode mode)
        {
            var occupantDensityOverride = 0.0;

            var getResult = RhinoGet.GetNumber("Enter building occupant density override", false, ref occupantDensityOverride);

            if (getResult != Result.Success)
            {
                return getResult;
            }

            var selectedRhinoObjects = doc.Objects.GetSelectedObjects(includeLights: false, includeGrips: false);

            var selectedUmiBuildings = context.Buildings.ForObjects(selectedRhinoObjects);

            var buildingSettings = context.GetSettings<BuildingSettings>();

            foreach (var selectedUmiBuilding in selectedUmiBuildings)
            {
                buildingSettings.GetOrCreate(selectedUmiBuilding.Id).OccupantDensityOverride = occupantDensityOverride;
            }

            return Result.Success;
        }
    }
}
