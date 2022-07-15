using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class NavPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NavPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PeerNodeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    NavLinkMatch = table.Column<int>(type: "INTEGER", nullable: false),
                    IsInNavMenu = table.Column<bool>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavPages_NavPages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "NavPages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NavPages_PeerNodes_PeerNodeId",
                        column: x => x.PeerNodeId,
                        principalTable: "PeerNodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavPages_IsInNavMenu",
                table: "NavPages",
                column: "IsInNavMenu");

            migrationBuilder.CreateIndex(
                name: "IX_NavPages_Order",
                table: "NavPages",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_NavPages_ParentId",
                table: "NavPages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_NavPages_PeerNodeId",
                table: "NavPages",
                column: "PeerNodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavPages");
        }
    }
}
