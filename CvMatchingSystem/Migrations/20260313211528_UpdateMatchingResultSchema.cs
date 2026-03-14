using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvMatchingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchingResultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiExplanation",
                table: "MatchingResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchedSkillsJson",
                table: "MatchingResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MissingSkillsJson",
                table: "MatchingResults",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiExplanation",
                table: "MatchingResults");

            migrationBuilder.DropColumn(
                name: "MatchedSkillsJson",
                table: "MatchingResults");

            migrationBuilder.DropColumn(
                name: "MissingSkillsJson",
                table: "MatchingResults");
        }
    }
}
