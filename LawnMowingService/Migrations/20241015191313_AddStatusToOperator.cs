﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LawnMowingService.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Operators",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Operators");
        }
    }
}
