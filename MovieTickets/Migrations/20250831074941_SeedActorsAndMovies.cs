using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieTickets.Migrations
{
    /// <inheritdoc />
    public partial class SeedActorsAndMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "Bio", "FirstName", "LastName", "Name", "News", "ProfilePicture" },
                values: new object[,]
                {
                    { 1, null, "Robert", "Downey Jr.", null, "", null },
                    { 2, null, "Chris", "Evans", null, "", null },
                    { 3, null, "Scarlett", "Johansson", null, "", null },
                    { 4, null, "Leonardo", "DiCaprio", null, "", null },
                    { 5, null, "Tom", "Hanks", null, "", null }
                });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "Price" },
                values: new object[] { new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 100m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "Price" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m });
        }
    }
}
