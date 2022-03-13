using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Linq;
using System.Runtime.InteropServices;
using Umi.RhinoServices;
using Umi.RhinoServices.Context;

namespace Umi.BetterOccupancyComplete.RhinoCommands
{
    [Guid("e7bfa093-278e-4d0f-a7b8-509ef1afcacb")]
    public class UmiSetBetterOccupantDensityForTemplate : UmiCommand
    {
        public override string EnglishName => nameof(UmiSetBetterOccupantDensityForTemplate);

        public override Result Run(RhinoDoc doc, UmiContext context, RunMode mode)
        {
            if (context.TemplateLibrary == null)
            {
                RhinoApp.WriteLine("The project has no template library!");
                return Result.Failure;
            }

            var templateNames = context.TemplateLibrary.AvailableTemplateNames().ToList();

            var templateNameGetter = new GetOption();
            templateNameGetter.SetCommandPrompt("Specify a building template");
            templateNameGetter.AddOptionList("Template", templateNames, 0);
            templateNameGetter.AcceptNothing(true);
            var getTemplateResult = templateNameGetter.Get();

            if (getTemplateResult == GetResult.Cancel)
            {
                return Result.Cancel;
            }

            var selectedIndex = 0;

            if (getTemplateResult != GetResult.Nothing)
            {
                selectedIndex = templateNameGetter.OptionIndex();
            }

            var templateName = templateNames[selectedIndex];

            RhinoApp.WriteLine($"Selected template: {templateName}");

            return Result.Success;
        }
    }
}
