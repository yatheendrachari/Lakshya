using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPath.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class someshitididntunderstand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop the OLD cascade FK first — this was missing!
            migrationBuilder.DropForeignKey(
                name: "fk_career_related_career_career_id",
                table: "career_related");

            // 2. Add the new related_career_id column
            migrationBuilder.AddColumn<int>(
                name: "related_career_id",
                table: "career_related",
                type: "integer",
                nullable: true);

            // 3. Index for the new FK
            migrationBuilder.CreateIndex(
                name: "ix_career_related_related_career_id",
                table: "career_related",
                column: "related_career_id");

            // 4. Re-add career_id FK with Restrict instead of Cascade
            migrationBuilder.AddForeignKey(
                name: "fk_career_related_career_career_id",
                table: "career_related",
                column: "career_id",
                principalTable: "career",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // 5. Add the new related_career_id FK
            migrationBuilder.AddForeignKey(
                name: "fk_career_related_career_related_career_id",
                table: "career_related",
                column: "related_career_id",
                principalTable: "career",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_career_related_career_career_id",
                table: "career_related");

            migrationBuilder.DropForeignKey(
                name: "fk_career_related_career_related_career_id",
                table: "career_related");

            migrationBuilder.DropIndex(
                name: "ix_career_related_related_career_id",
                table: "career_related");

            migrationBuilder.DropColumn(
                name: "related_career_id",
                table: "career_related");

            migrationBuilder.AddForeignKey(
                name: "fk_career_related_career_career_id",
                table: "career_related",
                column: "career_id",
                principalTable: "career",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
