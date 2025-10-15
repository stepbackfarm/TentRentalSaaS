﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TentRentalSaaS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsUsedToLoginToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "LoginTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "LoginTokens");
        }
    }
}
