using Umi.Core;

namespace Umi.BetterOccupancyComplete
{
    public class BuildingSettings : SettingsObject
    {
        public override string? Id { get; set; }

        public double? OccupantDensityOverride { get; set; }
    }
}
