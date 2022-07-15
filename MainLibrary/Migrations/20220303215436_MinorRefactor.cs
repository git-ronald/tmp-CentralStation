using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class MinorRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeerConnections_Peers_PeerId",
                table: "PeerConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerMessages_Peers_PeerId",
                table: "PeerMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeerId",
                table: "PeerMessages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeerId",
                table: "PeerConnections",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "PeerConnections",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeerConnections_Peers_PeerId",
                table: "PeerConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerMessages_Peers_PeerId",
                table: "PeerMessages");

            migrationBuilder.DropColumn(
                name: "IP",
                table: "PeerConnections");

            migrationBuilder.AlterColumn<Guid>(
                name: "PeerId",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PeerId",
                table: "PeerConnections",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerConnections_Peers_PeerId",
                table: "PeerConnections",
                column: "PeerId",
                principalTable: "Peers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerMessages_Peers_PeerId",
                table: "PeerMessages",
                column: "PeerId",
                principalTable: "Peers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
