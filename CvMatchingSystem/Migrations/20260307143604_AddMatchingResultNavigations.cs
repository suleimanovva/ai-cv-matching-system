using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvMatchingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchingResultNavigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchingResults_CandidateId",
                table: "MatchingResults",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingResults_JobId",
                table: "MatchingResults",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingResults_Candidates_CandidateId",
                table: "MatchingResults",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingResults_JobPostings_JobId",
                table: "MatchingResults",
                column: "JobId",
                principalTable: "JobPostings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingResults_Candidates_CandidateId",
                table: "MatchingResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchingResults_JobPostings_JobId",
                table: "MatchingResults");

            migrationBuilder.DropIndex(
                name: "IX_MatchingResults_CandidateId",
                table: "MatchingResults");

            migrationBuilder.DropIndex(
                name: "IX_MatchingResults_JobId",
                table: "MatchingResults");
        }
    }
}
