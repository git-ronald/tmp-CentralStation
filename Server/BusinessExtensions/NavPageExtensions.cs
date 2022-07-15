using CentralStation.Data.Models;

namespace CentralStation.Server.BusinessExtensions
{
    public static class NavPageExtensions
    {
        public static NavPage AddChildren(this NavPage parent, params NavPage[] children)
        {
            foreach (var child in children)
            {
                child.Parent = parent;
                child.ParentId = parent.Id;

                parent.Children.Add(child);
            }
            return parent;
        }
    }
}
