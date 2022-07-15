using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class TheFirst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Peers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeerConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    LastMessageTimeSent = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerConnections_Peers_PeerId",
                        column: x => x.PeerId,
                        principalTable: "Peers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeerMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerMessages_Peers_PeerId",
                        column: x => x.PeerId,
                        principalTable: "Peers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeerConnections_ConnectionId",
                table: "PeerConnections",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerConnections_LastMessageTimeSent",
                table: "PeerConnections",
                column: "LastMessageTimeSent");

            migrationBuilder.CreateIndex(
                name: "IX_PeerConnections_PeerId",
                table: "PeerConnections",
                column: "PeerId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_PeerId",
                table: "PeerMessages",
                column: "PeerId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_RequestTime",
                table: "PeerMessages",
                column: "RequestTime");

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_ResponseTime",
                table: "PeerMessages",
                column: "ResponseTime");

            migrationBuilder.CreateIndex(
                name: "IX_Peers_Name",
                table: "Peers",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeerConnections");

            migrationBuilder.DropTable(
                name: "PeerMessages");

            migrationBuilder.DropTable(
                name: "Peers");
        }
    }
}
