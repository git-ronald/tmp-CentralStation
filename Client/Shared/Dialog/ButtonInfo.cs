namespace CentralStation.Client.Shared.Dialog
{
    public class ButtonInfo
    {
        public ButtonInfo(string label, string cssClass)
        {
            Label = label;
            CssClass = cssClass;
        }
        public ButtonInfo(string label, string cssClass, Func<Task> action)
        {
            Label = label;
            CssClass = cssClass;
            Action = action;
        }

        public string Label { get; set; } = String.Empty;
        public string CssClass { get; set; } = String.Empty;
        public Func<Task> Action { get; set; } = () => Task.CompletedTask;
    }
}
