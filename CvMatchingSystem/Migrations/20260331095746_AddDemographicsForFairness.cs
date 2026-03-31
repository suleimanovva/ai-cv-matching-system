using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvMatchingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDemographicsForFairness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Candidates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Candidates");
        }
    }
}
