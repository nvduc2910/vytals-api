using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Vytals.Migrations
{
    public partial class addcurrentpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPoint",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPointInGame",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MathType",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Point",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPointInGame",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MathType",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPoint",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }
    }
}
