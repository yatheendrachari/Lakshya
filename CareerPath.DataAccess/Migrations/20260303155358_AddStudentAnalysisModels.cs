using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CareerPath.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAnalysisModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_student_profiles_application_users_student_id",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "feature_vector",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "discriminator",
                table: "AspNetUsers");

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

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "learning_paths",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_paths", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resources", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student_analysis_results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "text", nullable: false),
                    recommended_career = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_profile_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_analysis_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_analysis_results_student_profiles_student_profile_id",
                        column: x => x.student_profile_id,
                        principalTable: "student_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "summaries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    high_priority_modules = table.Column<int>(type: "integer", nullable: false),
                    medium_priority_modules = table.Column<int>(type: "integer", nullable: false),
                    start_with = table.Column<string>(type: "text", nullable: false),
                    estimated_modules = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_summaries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    resources_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_articles", x => x.id);
                    table.ForeignKey(
                        name: "fk_articles_resources_resources_id",
                        column: x => x.resources_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    resources_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                    table.ForeignKey(
                        name: "fk_courses_resources_resources_id",
                        column: x => x.resources_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "papers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    resources_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_papers", x => x.id);
                    table.ForeignKey(
                        name: "fk_papers_resources_resources_id",
                        column: x => x.resources_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "videos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    thumbnail = table.Column<string>(type: "text", nullable: true),
                    published_at = table.Column<string>(type: "text", nullable: true),
                    views = table.Column<int>(type: "integer", nullable: true),
                    is_fresh = table.Column<bool>(type: "boolean", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    channel = table.Column<string>(type: "text", nullable: true),
                    resources_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                    table.ForeignKey(
                        name: "fk_videos_resources_resources_id",
                        column: x => x.resources_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "career_predictions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    career = table.Column<string>(type: "text", nullable: false),
                    match_percentage = table.Column<double>(type: "double precision", nullable: false),
                    field = table.Column<string>(type: "text", nullable: false),
                    student_analysis_result_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_career_predictions", x => x.id);
                    table.ForeignKey(
                        name: "fk_career_predictions_student_analysis_results_student_analysi",
                        column: x => x.student_analysis_result_id,
                        principalTable: "student_analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feature_vectors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    field = table.Column<string>(type: "text", nullable: false),
                    gpa = table.Column<double>(type: "double precision", nullable: false),
                    extracurricular_activities = table.Column<int>(type: "integer", nullable: false),
                    internships = table.Column<int>(type: "integer", nullable: false),
                    projects = table.Column<int>(type: "integer", nullable: false),
                    leadership_positions = table.Column<bool>(type: "boolean", nullable: false),
                    field_specific_courses = table.Column<int>(type: "integer", nullable: false),
                    research_experience = table.Column<bool>(type: "boolean", nullable: false),
                    coding_skills = table.Column<int>(type: "integer", nullable: false),
                    communication_skills = table.Column<int>(type: "integer", nullable: false),
                    problem_solving_skills = table.Column<int>(type: "integer", nullable: false),
                    teamwork_skills = table.Column<int>(type: "integer", nullable: false),
                    analytical_skills = table.Column<int>(type: "integer", nullable: false),
                    presentation_skills = table.Column<int>(type: "integer", nullable: false),
                    networking_skills = table.Column<int>(type: "integer", nullable: false),
                    career_intent = table.Column<string>(type: "text", nullable: true),
                    overall_profile_confidence = table.Column<double>(type: "double precision", nullable: false),
                    student_analysis_result_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_vectors", x => x.id);
                    table.ForeignKey(
                        name: "fk_feature_vectors_student_analysis_results_student_analysis_r",
                        column: x => x.student_analysis_result_id,
                        principalTable: "student_analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gap_analyses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    career = table.Column<string>(type: "text", nullable: false),
                    readiness_score = table.Column<double>(type: "double precision", nullable: false),
                    match_percentage = table.Column<double>(type: "double precision", nullable: false),
                    student_analysis_result_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gap_analyses", x => x.id);
                    table.ForeignKey(
                        name: "fk_gap_analyses_student_analysis_results_student_analysis_resu",
                        column: x => x.student_analysis_result_id,
                        principalTable: "student_analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_path_models",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    selected_career = table.Column<string>(type: "text", nullable: false),
                    field = table.Column<string>(type: "text", nullable: false),
                    total_concepts = table.Column<int>(type: "integer", nullable: false),
                    summary_id = table.Column<int>(type: "integer", nullable: false),
                    learning_path_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_path_models", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_path_models_learning_paths_learning_path_id",
                        column: x => x.learning_path_id,
                        principalTable: "learning_paths",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learning_path_models_summaries_summary_id",
                        column: x => x.summary_id,
                        principalTable: "summaries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "confidence_scores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gpa = table.Column<double>(type: "double precision", nullable: false),
                    coding_skills = table.Column<double>(type: "double precision", nullable: false),
                    communication_skills = table.Column<double>(type: "double precision", nullable: false),
                    problem_solving_skills = table.Column<double>(type: "double precision", nullable: false),
                    teamwork_skills = table.Column<double>(type: "double precision", nullable: false),
                    analytical_skills = table.Column<double>(type: "double precision", nullable: false),
                    presentation_skills = table.Column<double>(type: "double precision", nullable: false),
                    networking_skills = table.Column<double>(type: "double precision", nullable: false),
                    feature_vector_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_confidence_scores", x => x.id);
                    table.ForeignKey(
                        name: "fk_confidence_scores_feature_vectors_feature_vector_id",
                        column: x => x.feature_vector_id,
                        principalTable: "feature_vectors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inconsistencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    feature_vector_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inconsistencies", x => x.id);
                    table.ForeignKey(
                        name: "fk_inconsistencies_feature_vectors_feature_vector_id",
                        column: x => x.feature_vector_id,
                        principalTable: "feature_vectors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sources_used",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resume = table.Column<bool>(type: "boolean", nullable: false),
                    git_hub = table.Column<bool>(type: "boolean", nullable: false),
                    questionnaire = table.Column<bool>(type: "boolean", nullable: false),
                    feature_vector_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sources_used", x => x.id);
                    table.ForeignKey(
                        name: "fk_sources_used_feature_vectors_feature_vector_id",
                        column: x => x.feature_vector_id,
                        principalTable: "feature_vectors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gap_analysis_gaps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    feature = table.Column<string>(type: "text", nullable: false),
                    student_value = table.Column<double>(type: "double precision", nullable: false),
                    ideal_value = table.Column<double>(type: "double precision", nullable: false),
                    gap = table.Column<double>(type: "double precision", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    advice = table.Column<string>(type: "text", nullable: false),
                    is_critical = table.Column<bool>(type: "boolean", nullable: false),
                    gap_analysis_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gap_analysis_gaps", x => x.id);
                    table.ForeignKey(
                        name: "fk_gap_analysis_gaps_gap_analyses_gap_analysis_id",
                        column: x => x.gap_analysis_id,
                        principalTable: "gap_analyses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gap_analysis_strengths",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    feature = table.Column<string>(type: "text", nullable: false),
                    student_value = table.Column<double>(type: "double precision", nullable: false),
                    ideal_value = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    gap_analysis_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gap_analysis_strengths", x => x.id);
                    table.ForeignKey(
                        name: "fk_gap_analysis_strengths_gap_analyses_gap_analysis_id",
                        column: x => x.gap_analysis_id,
                        principalTable: "gap_analyses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gap_analysis_summaries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    total_features = table.Column<int>(type: "integer", nullable: false),
                    features_met = table.Column<int>(type: "integer", nullable: false),
                    medium_priority_gaps = table.Column<int>(type: "integer", nullable: false),
                    high_priority_gaps = table.Column<int>(type: "integer", nullable: false),
                    gap_analysis_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gap_analysis_summaries", x => x.id);
                    table.ForeignKey(
                        name: "fk_gap_analysis_summaries_gap_analyses_gap_analysis_id",
                        column: x => x.gap_analysis_id,
                        principalTable: "gap_analyses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    concept = table.Column<string>(type: "text", nullable: false),
                    maps_to_gap = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    total_resources = table.Column<int>(type: "integer", nullable: false),
                    resource_id = table.Column<int>(type: "integer", nullable: false),
                    learning_path_model_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_modules", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_modules_learning_path_models_learning_path_model_id",
                        column: x => x.learning_path_model_id,
                        principalTable: "learning_path_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learning_modules_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_articles_resources_id",
                table: "articles",
                column: "resources_id");

            migrationBuilder.CreateIndex(
                name: "ix_career_predictions_student_analysis_result_id",
                table: "career_predictions",
                column: "student_analysis_result_id");

            migrationBuilder.CreateIndex(
                name: "ix_confidence_scores_feature_vector_id",
                table: "confidence_scores",
                column: "feature_vector_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_courses_resources_id",
                table: "courses",
                column: "resources_id");

            migrationBuilder.CreateIndex(
                name: "ix_feature_vectors_student_analysis_result_id",
                table: "feature_vectors",
                column: "student_analysis_result_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_gap_analyses_student_analysis_result_id",
                table: "gap_analyses",
                column: "student_analysis_result_id");

            migrationBuilder.CreateIndex(
                name: "ix_gap_analysis_gaps_gap_analysis_id",
                table: "gap_analysis_gaps",
                column: "gap_analysis_id");

            migrationBuilder.CreateIndex(
                name: "ix_gap_analysis_strengths_gap_analysis_id",
                table: "gap_analysis_strengths",
                column: "gap_analysis_id");

            migrationBuilder.CreateIndex(
                name: "ix_gap_analysis_summaries_gap_analysis_id",
                table: "gap_analysis_summaries",
                column: "gap_analysis_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inconsistencies_feature_vector_id",
                table: "inconsistencies",
                column: "feature_vector_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_modules_learning_path_model_id",
                table: "learning_modules",
                column: "learning_path_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_modules_resource_id",
                table: "learning_modules",
                column: "resource_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_learning_path_models_learning_path_id",
                table: "learning_path_models",
                column: "learning_path_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_learning_path_models_summary_id",
                table: "learning_path_models",
                column: "summary_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_papers_resources_id",
                table: "papers",
                column: "resources_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_used_feature_vector_id",
                table: "sources_used",
                column: "feature_vector_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_analysis_results_student_profile_id",
                table: "student_analysis_results",
                column: "student_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_resources_id",
                table: "videos",
                column: "resources_id");

            migrationBuilder.AddForeignKey(
                name: "fk_student_profiles_application_user_student_id",
                table: "student_profiles",
                column: "student_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_student_profiles_application_user_student_id",
                table: "student_profiles");

            migrationBuilder.DropTable(
                name: "articles");

            migrationBuilder.DropTable(
                name: "career_predictions");

            migrationBuilder.DropTable(
                name: "confidence_scores");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "gap_analysis_gaps");

            migrationBuilder.DropTable(
                name: "gap_analysis_strengths");

            migrationBuilder.DropTable(
                name: "gap_analysis_summaries");

            migrationBuilder.DropTable(
                name: "inconsistencies");

            migrationBuilder.DropTable(
                name: "learning_modules");

            migrationBuilder.DropTable(
                name: "papers");

            migrationBuilder.DropTable(
                name: "sources_used");

            migrationBuilder.DropTable(
                name: "videos");

            migrationBuilder.DropTable(
                name: "gap_analyses");

            migrationBuilder.DropTable(
                name: "learning_path_models");

            migrationBuilder.DropTable(
                name: "feature_vectors");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "learning_paths");

            migrationBuilder.DropTable(
                name: "summaries");

            migrationBuilder.DropTable(
                name: "student_analysis_results");

            migrationBuilder.AddColumn<string>(
                name: "feature_vector",
                table: "student_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "discriminator",
                table: "AspNetUsers",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_student_profiles_application_users_student_id",
                table: "student_profiles",
                column: "student_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
