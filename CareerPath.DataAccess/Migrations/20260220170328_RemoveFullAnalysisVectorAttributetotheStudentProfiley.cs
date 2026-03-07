using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPath.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFullAnalysisVectorAttributetotheStudentProfiley : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_analysis_vector",
                table: "student_profiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "full_analysis_vector",
                table: "student_profiles",
                type: "text",
                nullable: true);
        }
    }
}
