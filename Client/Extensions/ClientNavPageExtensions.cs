using CentralStation.Client.Models;

namespace CentralStation.Client.Extensions
{
    public static class ClientNavPageExtensions
    {
        public static IEnumerable<ClientNavPage> AutoOrder(this IEnumerable<ClientNavPage> pages)
        {
            int order = 0;
            foreach (ClientNavPage page in pages)
            {
                page.Order = order++;
                page.Children.AutoOrder();
            }
            return pages;
        }

        public static ClientNavPage AddChildren(this ClientNavPage parent, params ClientNavPage[] children)
        {
            foreach (var child in children)
            {
                child.ParentId = parent.Id;
                parent.Children.Add(child);
            }
            return parent;
        }
    }
}
