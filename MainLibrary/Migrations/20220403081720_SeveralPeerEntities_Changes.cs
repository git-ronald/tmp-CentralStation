using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralStation.Data.Migrations
{
    public partial class SeveralPeerEntities_Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLeading",
                table: "PeerNodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestTime",
                table: "PeerMessages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLeading",
                table: "PeerNodes");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "PeerMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestTime",
                table: "PeerMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
