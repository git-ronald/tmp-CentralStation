using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class MajorRefactorPeerRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastResponseTime",
                table: "PeerConnections",
                newName: "LastMessageTime");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_LastResponseTime",
                table: "PeerConnections",
                newName: "IX_PeerConnections_LastMessageTime");

            migrationBuilder.AddColumn<string>(
                name: "RequestData",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestMethod",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PeerNodes_Name",
                table: "PeerNodes",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PeerNodes_Name",
                table: "PeerNodes");

            migrationBuilder.DropColumn(
                name: "RequestData",
                table: "PeerMessages");

            migrationBuilder.DropColumn(
                name: "RequestMethod",
                table: "PeerMessages");

            migrationBuilder.RenameColumn(
                name: "LastMessageTime",
                table: "PeerConnections",
                newName: "LastResponseTime");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_LastMessageTime",
                table: "PeerConnections",
                newName: "IX_PeerConnections_LastResponseTime");
        }
    }
}
