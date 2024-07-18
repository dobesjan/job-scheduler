using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMonitoring.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class entityParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "WebPages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interval",
                table: "WebPages");
        }
    }
}
