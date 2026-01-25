using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daloy_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLessonToUseBlobContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentHtml",
                table: "Lessons",
                newName: "LessonType");

            migrationBuilder.AddColumn<string>(
                name: "ContentBlobPath",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "Lessons",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentBlobPath",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "LessonType",
                table: "Lessons",
                newName: "ContentHtml");
        }
    }
}
