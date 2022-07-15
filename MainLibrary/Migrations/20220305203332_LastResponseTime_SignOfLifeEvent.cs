using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class LastResponseTime_SignOfLifeEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PeerConnections",
                newName: "LastResponseTime");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_CreationTime",
                table: "PeerConnections",
                newName: "IX_PeerConnections_LastResponseTime");

            migrationBuilder.AddColumn<short>(
                name: "SignOfLifeEvent",
                table: "PeerNodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignOfLifeEvent",
                table: "PeerNodes");

            migrationBuilder.RenameColumn(
                name: "LastResponseTime",
                table: "PeerConnections",
                newName: "CreationTime");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_LastResponseTime",
                table: "PeerConnections",
                newName: "IX_PeerConnections_CreationTime");
        }
    }
}
