using Rhino;
using Rhino.DocObjects;
using Rhino.PlugIns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using Umi.BetterOccupancyComplete.Panel;
using Umi.BetterOccupancyComplete.Properties;
using Umi.RhinoServices;
using Umi.RhinoServices.Buildings;
using Umi.RhinoServices.Context;
using Umi.RhinoServices.ModuleProjectSettings;
using Umi.RhinoServices.UmiEvents;

namespace Umi.BetterOccupancyComplete
{
    public class Module : UmiModule
    {
        public static Module Instance { get; private set; }

        private readonly PanelViewModel panelViewModel;
        private readonly Dictionary<Guid, int> selectedBuildingOccupancy;

        private Dictionary<Guid, int> betterOccupancies = new Dictionary<Guid, int>();

        public Module()
        {
            panelViewModel = new();
            selectedBuildingOccupancy = new();

            ModuleControl = new PanelControl { DataContext = panelViewModel };

            Instance = this;
        }

        protected override UserControl ModuleControl { get; }

        protected override Tuple<Bitmap, ImageFormat> TabHeaderIcon => Tuple.Create(Resources.PanelIcon, ImageFormat.Png);

        protected override string TabHeaderToolTip => "Better Occupancy";

        public override IEnumerable<IModuleProjectSettingsHandler> ProjectSettingsHandlers
        {
            get
            {
                yield return new DefaultProjectSettingsHandler<ModuleSettings>("better-occupancy.json");
            }
        }

        protected override Color? Falsecolor(IUmiBuilding building)
        {
            return Color.Black;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            UmiEventSource.Instance.ProjectOpened += OnProjectOpened;
            UmiEventSource.Instance.ProjectClosed += OnProjectClosed;

            return base.OnLoad(ref errorMessage);
        }

        private void AddBuildingsToSelection(IEnumerable<RhinoObject> selectedRhinoObjects)
        {
            if (UmiContext.Current == null)
            {
                return;
            }

            foreach (var selectedRhinoObject in selectedRhinoObjects)
            {
                betterOccupancies.TryGetValue(selectedRhinoObject.Id, out var betterOccupancy);

                selectedBuildingOccupancy[selectedRhinoObject.Id] = betterOccupancy;
            }
        }

        private void OnDeselectAllObjects(object sender, RhinoDeselectAllObjectsEventArgs e)
        {
            selectedBuildingOccupancy.Clear();

            panelViewModel.TotalSelectedBuildingOccupants = 0;
        }

        private void OnSelectionChanged(object sender, RhinoObjectSelectionEventArgs e)
        {
            if (e.Selected)
            {
                AddBuildingsToSelection(e.RhinoObjects);
            }
            else
            {
                RemoveBuildingsFromSelection(e.RhinoObjects);
            }

            panelViewModel.TotalSelectedBuildingOccupants = selectedBuildingOccupancy.Values.Sum();
        }

        private void RemoveBuildingsFromSelection(IEnumerable<RhinoObject> deselectedRhinoObjects)
        {
            foreach (var rhinoObject in deselectedRhinoObjects)
            {
                selectedBuildingOccupancy.Remove(rhinoObject.Id);
            }
        }

        private void OnProjectOpened(object sender, UmiContext newProjectContext)
        {
            foreach (var umiObject in newProjectContext.GetObjects())
            {
                if (umiObject.Data.TryGetValue("cody module demo/better occupancy", out var series))
                {
                    betterOccupancies[Guid.Parse(umiObject.Id)] = (int)series.Data[0];
                }
            }

            RhinoDoc.DeselectAllObjects += OnDeselectAllObjects;
            RhinoDoc.DeselectObjects += OnSelectionChanged;
            RhinoDoc.SelectObjects += OnSelectionChanged;

            selectedBuildingOccupancy.Clear();

            AddBuildingsToSelection(RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(includeLights: false, includeGrips: false));
        }

        private void OnProjectClosed(object sender, object e)
        {
            RhinoDoc.SelectObjects -= OnSelectionChanged;
            RhinoDoc.DeselectObjects -= OnSelectionChanged;
            RhinoDoc.DeselectAllObjects -= OnDeselectAllObjects;
        }

        public void SetBetterOccupancy(Guid buildingId, int occupancy)
        {
            betterOccupancies[buildingId] = occupancy;
        }
    }
}
