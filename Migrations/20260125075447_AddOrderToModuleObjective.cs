using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daloy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToModuleObjective : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ModuleObjectives",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "ModuleObjectives");
        }
    }
}
