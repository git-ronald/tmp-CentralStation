using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class PeerNodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PeerMessages; DELETE FROM PeerConnections;");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerConnections_Peers_PeerId",
                table: "PeerConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerMessages_Peers_PeerId",
                table: "PeerMessages");

            migrationBuilder.DropIndex(
                name: "IX_PeerMessages_PeerId",
                table: "PeerMessages");

            migrationBuilder.DropColumn(
                name: "PeerId",
                table: "PeerMessages");

            migrationBuilder.RenameColumn(
                name: "PeerId",
                table: "PeerConnections",
                newName: "PeerNodeId");

            migrationBuilder.RenameColumn(
                name: "LastMessageTimeSent",
                table: "PeerConnections",
                newName: "CreationTime");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_PeerId",
                table: "PeerConnections",
                newName: "IX_PeerConnections_PeerNodeId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_LastMessageTimeSent",
                table: "PeerConnections",
                newName: "IX_PeerConnections_CreationTime");

            migrationBuilder.AddColumn<Guid>(
                name: "PeerNodeId",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PeerNodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LastIP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerNodes_Peers_PeerId",
                        column: x => x.PeerId,
                        principalTable: "Peers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_PeerNodeId",
                table: "PeerMessages",
                column: "PeerNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerNodes_CreationTime",
                table: "PeerNodes",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_PeerNodes_PeerId",
                table: "PeerNodes",
                column: "PeerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerConnections_PeerNodes_PeerNodeId",
                table: "PeerConnections",
                column: "PeerNodeId",
                principalTable: "PeerNodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerMessages_PeerNodes_PeerNodeId",
                table: "PeerMessages",
                column: "PeerNodeId",
                principalTable: "PeerNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PeerMessages; DELETE FROM PeerConnections;");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerConnections_PeerNodes_PeerNodeId",
                table: "PeerConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerMessages_PeerNodes_PeerNodeId",
                table: "PeerMessages");

            migrationBuilder.DropTable(
                name: "PeerNodes");

            migrationBuilder.DropIndex(
                name: "IX_PeerMessages_PeerNodeId",
                table: "PeerMessages");

            migrationBuilder.DropColumn(
                name: "PeerNodeId",
                table: "PeerMessages");

            migrationBuilder.RenameColumn(
                name: "PeerNodeId",
                table: "PeerConnections",
                newName: "PeerId");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PeerConnections",
                newName: "LastMessageTimeSent");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_PeerNodeId",
                table: "PeerConnections",
                newName: "IX_PeerConnections_PeerId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerConnections_CreationTime",
                table: "PeerConnections",
                newName: "IX_PeerConnections_LastMessageTimeSent");

            migrationBuilder.AddColumn<Guid>(
                name: "PeerId",
                table: "PeerMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_PeerId",
                table: "PeerMessages",
                column: "PeerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerConnections_Peers_PeerId",
                table: "PeerConnections",
                column: "PeerId",
                principalTable: "Peers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerMessages_Peers_PeerId",
                table: "PeerMessages",
                column: "PeerId",
                principalTable: "Peers",
                principalColumn: "Id");
        }
    }
}
