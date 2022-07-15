using Microsoft.AspNetCore.Components;

namespace CentralStation.Client.Shared.Dialog
{
    public interface IBuilderReady
    {
        IBuilderReady AddButton(ButtonInfo button);
        IBuilderReady AddBody(RenderFragment body);

        Action Show { get; }
        //RenderFragment? Message { get; set; }
    }
}
