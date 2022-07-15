using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class Removed_PeerMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeerMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeerMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PeerNodeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RequestData = table.Column<string>(type: "TEXT", nullable: false),
                    RequestMethod = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RequestTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResponseTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerMessages_PeerNodes_PeerNodeId",
                        column: x => x.PeerNodeId,
                        principalTable: "PeerNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_PeerNodeId",
                table: "PeerMessages",
                column: "PeerNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_RequestTime",
                table: "PeerMessages",
                column: "RequestTime");

            migrationBuilder.CreateIndex(
                name: "IX_PeerMessages_ResponseTime",
                table: "PeerMessages",
                column: "ResponseTime");
        }
    }
}
