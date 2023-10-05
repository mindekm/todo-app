﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addcompositeindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_ExternalId",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ExternalId_CreatedBy",
                table: "Notes",
                columns: new[] { "ExternalId", "CreatedBy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_ExternalId_CreatedBy",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ExternalId",
                table: "Notes",
                column: "ExternalId",
                unique: true);
        }
    }
}
