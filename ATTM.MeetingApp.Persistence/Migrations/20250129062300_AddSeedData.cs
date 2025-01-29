using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATTM.MeetingApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Meetings",
                columns: new[] { "Id", "Location", "Name", "Seats" },
                values: new object[] { new Guid("dc9930a3-079d-4f31-addd-f02141d7422f"), 4, "Meeting 1", 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Meetings",
                keyColumn: "Id",
                keyValue: new Guid("dc9930a3-079d-4f31-addd-f02141d7422f"));
        }
    }
}
