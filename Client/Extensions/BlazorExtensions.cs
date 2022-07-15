using Microsoft.AspNetCore.Components;

namespace CentralStation.Client.Extensions
{
    public static class BlazorExtensions
    {
        public static RenderFragment BuildFragment(this string text, int sequence = 0) => builder =>
        {
            builder.AddContent(sequence, new MarkupString(text));
        };
    }
}
