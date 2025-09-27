using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ExternalExerciseId",
                table: "Exercises",
                column: "ExternalExerciseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_NormalizedMuscleGroup",
                table: "Exercises",
                column: "NormalizedMuscleGroup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exercises_ExternalExerciseId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_NormalizedMuscleGroup",
                table: "Exercises");
        }
    }
}
