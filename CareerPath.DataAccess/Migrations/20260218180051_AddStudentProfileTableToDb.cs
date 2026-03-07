using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CareerPath.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfileTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<string>(type: "text", nullable: false),
                    git_hub_username = table.Column<string>(type: "text", nullable: true),
                    has_resume = table.Column<bool>(type: "boolean", nullable: false),
                    onboarding_completed = table.Column<bool>(type: "boolean", nullable: false),
                    feature_vector = table.Column<string>(type: "text", nullable: true),
                    last_analyzed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_profiles_application_users_student_id",
                        column: x => x.student_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_student_profiles_student_id",
                table: "student_profiles",
                column: "student_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_profiles");
        }
    }
}
