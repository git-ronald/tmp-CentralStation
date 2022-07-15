using Microsoft.AspNetCore.Components;

namespace CentralStation.Client.Shared.Dialog;

public class DialogBuilder : IBuilderReady
{
    public DialogBuilder(string title, Action showAction) //, Func<Task> closeAction)
    {
        Title = title;
        Show = showAction;
        //_closeAction = closeAction;
    }

    public string Title { get; } = String.Empty;
    public RenderFragment? Body { get; set; }
    //public string Body { get; private set; } = String.Empty;
    public ICollection<ButtonInfo> Buttons { get; } = new List<ButtonInfo>();
    public Action Show { get; }

    public IBuilderReady AddBody(RenderFragment body)
    {
        Body = body;
        return this;
    }

    public IBuilderReady AddButton(ButtonInfo button)
    {
        this.Buttons.Add(button);
        return this;
    }
}
