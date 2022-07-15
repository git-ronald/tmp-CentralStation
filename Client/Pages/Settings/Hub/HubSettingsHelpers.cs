using CentralStation.Client.Extensions;
using CentralStation.Client.Models;
using CentralStation.Client.Shared.DataGrid;
using CoreLibrary.Helpers;
using Microsoft.AspNetCore.Components;

namespace CentralStation.Client.Pages.Settings.Hub
{
    public static class HubSettingsHelpers
    {
        public static IEnumerable<DataColumnDefinition<PeerConnectionRow>> DefineConnectionsDataGrid()
        {
            yield return new("Peer name", row => row.PeerName);
            yield return new("IP", row => row.IP);
            yield return new("Last message time", row => row.LastMessageTime.ToStandardFormat());
        }

        public static RenderFragment BuildConfirmDeleteNodeMessage(PeerNodeRow row)
        {
            string BuildText()
            {
                if (row.HubConnectionIds.Length == 0)
                {
                    return $"Delete node with zero connections?";
                }
                return $"Delete node with message time {row.LastMessageTime.ToStandardFormat()}?";
            }
            return BuildText().BuildFragment();
        }

        public static RenderFragment BuildConfirmDeleteConnectionMessage(PeerConnectionRow row)
        {
            return $"Delete connection with message time {row.LastMessageTime.ToStandardFormat()}?".BuildFragment();
        }
    }
}
