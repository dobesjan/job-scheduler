using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMonitoring.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class entityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "WebPages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MonitoredEntityTypeId",
                table: "WebPages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "WebpageMetrics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EntityTypeId",
                table: "WebpageMetrics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasError",
                table: "WebpageMetrics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EntityTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Web page" });

            migrationBuilder.CreateIndex(
                name: "IX_WebPages_MonitoredEntityTypeId",
                table: "WebPages",
                column: "MonitoredEntityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WebPages_EntityTypes_MonitoredEntityTypeId",
                table: "WebPages",
                column: "MonitoredEntityTypeId",
                principalTable: "EntityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebPages_EntityTypes_MonitoredEntityTypeId",
                table: "WebPages");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropIndex(
                name: "IX_WebPages_MonitoredEntityTypeId",
                table: "WebPages");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "WebPages");

            migrationBuilder.DropColumn(
                name: "MonitoredEntityTypeId",
                table: "WebPages");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "WebpageMetrics");

            migrationBuilder.DropColumn(
                name: "EntityTypeId",
                table: "WebpageMetrics");

            migrationBuilder.DropColumn(
                name: "HasError",
                table: "WebpageMetrics");
        }
    }
}
