using Microsoft.EntityFrameworkCore.Migrations;

namespace server.Migrations
{
    public partial class AddedStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Procedures",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Doctors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "DateTimePairs",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "DateTimePairs",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DateTimePairs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DateTimePairs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Procedures",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Doctors",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "DateTimePairs",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "DateTimePairs",
                newName: "date");
        }
    }
}
