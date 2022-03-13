﻿using Rhino;
using Rhino.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Umi.Core;
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
            var simulationResults = new List<IUmiObject>();

            foreach (var building in context.Buildings.All)
            {
                var buildingOccupancy = building.OccupantDensity * building.GrossFloorArea;

                if (buildingOccupancy.HasValue)
                {
                    var resultDataSeries = new UmiDataSeries();
                    resultDataSeries.Name = "cody module demo/better occupancy";
                    resultDataSeries.Units = "persons";
                    resultDataSeries.Data = new List<double> { buildingOccupancy.Value };

                    var resultObject = UmiObject.Create(building.Name, building.Id.ToString(), resultDataSeries);

                    simulationResults.Add(resultObject);

                    RhinoApp.WriteLine($"Building: {building.Name}: {buildingOccupancy} occupant(s)");
                }
            }

            context.StoreObjects(simulationResults);

            return Result.Success;
        }
    }
}
