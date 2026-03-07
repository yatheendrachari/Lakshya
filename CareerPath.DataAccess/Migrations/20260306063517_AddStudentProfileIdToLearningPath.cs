using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPath.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfileIdToLearningPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "student_profile_id",
                table: "learning_paths",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "ix_learning_paths_student_profile_id",
                table: "learning_paths",
                column: "student_profile_id");

            migrationBuilder.AddForeignKey(
                name: "fk_learning_paths_student_profiles_student_profile_id",
                table: "learning_paths",
                column: "student_profile_id",
                principalTable: "student_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_learning_paths_student_profiles_student_profile_id",
                table: "learning_paths");

            migrationBuilder.DropIndex(
                name: "ix_learning_paths_student_profile_id",
                table: "learning_paths");

            migrationBuilder.DropColumn(
                name: "student_profile_id",
                table: "learning_paths");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
