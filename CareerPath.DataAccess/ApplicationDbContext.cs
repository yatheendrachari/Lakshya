using CareerPath.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  // ✅ Fixed
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Existing
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Career> Career { get; set; }
        public DbSet<CareerResponsibilities> CareerResponsibilities { get; set; }
        public DbSet<CareerProgressionSteps> CareerProgressionSteps { get; set; }
        public DbSet<CareerRequiredSkills> CareerRequiredSkills { get; set; }
        public DbSet<CareerRelated> CareerRelated { get; set; }
        public DbSet<CareerTopCompanies> CareerTopCompanies { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        
        public DbSet<StudentAnalysisResult> StudentAnalysisResults { get; set; }
        public DbSet<FeatureVector> FeatureVectors { get; set; }
        public DbSet<SourcesUsed> SourcesUsed { get; set; }
        public DbSet<ConfidenceScores> ConfidenceScores { get; set; }
        public DbSet<InconsistencyDetected> Inconsistencies { get; set; }
        public DbSet<CareerPrediction> CareerPredictions { get; set; }
        public DbSet<GapAnalysis> GapAnalyses { get; set; }
        public DbSet<GapAnalysisStrength> GapAnalysisStrengths { get; set; }
        public DbSet<GapAnalysisGap> GapAnalysisGaps { get; set; }
        public DbSet<GapAnalysisSummary> GapAnalysisSummaries { get; set; }

        // LearningPath family — add ALL of them
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<LearningPathModel> LearningPathModels { get; set; }
        public DbSet<LearningModules> LearningModules { get; set; }
        public DbSet<Resources> Resources { get; set; }
        public DbSet<Videos> Videos { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<Summary> Summaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Existing CareerRelated config ──────────────────────────────
            modelBuilder.Entity<CareerRelated>()
                .HasOne(cr => cr.Career)
                .WithMany(c => c.RelatedCareers)
                .HasForeignKey(cr => cr.CareerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareerRelated>()
                .HasOne(cr => cr.RelatedCareer)
                .WithMany()
                .HasForeignKey(cr => cr.RelatedCareerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── LearningPath ↔ LearningPathModel (one-to-one) ─────────────
            modelBuilder.Entity<LearningPathModel>()
                .HasOne(lpm => lpm.LearningPath)
                .WithOne(lp => lp.learning_path_model)
                .HasForeignKey<LearningPathModel>(lpm => lpm.LearningPathId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── LearningPathModel ↔ Summary (one-to-one) ──────────────────
            modelBuilder.Entity<LearningPathModel>()
                .HasOne(lpm => lpm.Summary)
                .WithOne(s => s.LearningPathModel)
                .HasForeignKey<LearningPathModel>(lpm => lpm.SummaryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── LearningPathModel ↔ LearningModules (one-to-many) ─────────
            modelBuilder.Entity<LearningModules>()
                .HasOne(lm => lm.LearningPathModel)
                .WithMany(lpm => lpm.learning_modules)
                .HasForeignKey(lm => lm.LearningPathModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── LearningModule ↔ Resource (one-to-one) ────────────────────
            modelBuilder.Entity<LearningModules>()
                .HasOne(lm => lm.Resources)
                .WithOne(r => r.LearningModule)
                .HasForeignKey<LearningModules>(lm => lm.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Resource ↔ child collections ──────────────────────────────
            modelBuilder.Entity<Videos>()
                .HasOne(v => v.Resource)
                .WithMany(r => r.Videos)
                .HasForeignKey(v => v.ResourcesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Resource)
                .WithMany(r => r.Courses)
                .HasForeignKey(c => c.ResourcesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Resource)
                .WithMany(r => r.Articles)
                .HasForeignKey(a => a.ResourcesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Paper>()
                .HasOne(p => p.Resource)
                .WithMany(r => r.Papers)
                .HasForeignKey(p => p.ResourcesId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            // In OnModelCreating:

// StudentProfile → StudentAnalysisResult (one-to-many, a student can be re-analyzed)
            modelBuilder.Entity<StudentAnalysisResult>()
                .HasOne(r => r.StudentProfile)
                .WithMany(sp => sp.AnalysisResults)   // ← this is what was missing
                .HasForeignKey(r => r.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

// StudentAnalysisResult → FeatureVector (one-to-one)
modelBuilder.Entity<FeatureVector>()
    .HasOne(fv => fv.StudentAnalysisResult)
    .WithOne(r => r.FeatureVector)
    .HasForeignKey<FeatureVector>(fv => fv.StudentAnalysisResultId)
    .OnDelete(DeleteBehavior.Cascade);

// FeatureVector → SourcesUsed (one-to-one)
modelBuilder.Entity<SourcesUsed>()
    .HasOne(s => s.FeatureVector)
    .WithOne(fv => fv.SourcesUsed)
    .HasForeignKey<SourcesUsed>(s => s.FeatureVectorId)
    .OnDelete(DeleteBehavior.Cascade);

// FeatureVector → ConfidenceScores (one-to-one)
modelBuilder.Entity<ConfidenceScores>()
    .HasOne(cs => cs.FeatureVector)
    .WithOne(fv => fv.ConfidenceScores)
    .HasForeignKey<ConfidenceScores>(cs => cs.FeatureVectorId)
    .OnDelete(DeleteBehavior.Cascade);

// FeatureVector → Inconsistencies (one-to-many)
modelBuilder.Entity<InconsistencyDetected>()
    .HasOne(i => i.FeatureVector)
    .WithMany(fv => fv.Inconsistencies)
    .HasForeignKey(i => i.FeatureVectorId)
    .OnDelete(DeleteBehavior.Cascade);

// StudentAnalysisResult → CareerPredictions (one-to-many)
modelBuilder.Entity<CareerPrediction>()
    .HasOne(cp => cp.StudentAnalysisResult)
    .WithMany(r => r.CareerPredictions)
    .HasForeignKey(cp => cp.StudentAnalysisResultId)
    .OnDelete(DeleteBehavior.Cascade);

// StudentAnalysisResult → GapAnalyses (one-to-many)
modelBuilder.Entity<GapAnalysis>()
    .HasOne(ga => ga.StudentAnalysisResult)
    .WithMany(r => r.GapAnalyses)
    .HasForeignKey(ga => ga.StudentAnalysisResultId)
    .OnDelete(DeleteBehavior.Cascade);

// GapAnalysis → Summary (one-to-one)
modelBuilder.Entity<GapAnalysisSummary>()
    .HasOne(s => s.GapAnalysis)
    .WithOne(ga => ga.Summary)
    .HasForeignKey<GapAnalysisSummary>(s => s.GapAnalysisId)
    .OnDelete(DeleteBehavior.Cascade);

// GapAnalysis → Strengths (one-to-many)
modelBuilder.Entity<GapAnalysisStrength>()
    .HasOne(s => s.GapAnalysis)
    .WithMany(ga => ga.Strengths)
    .HasForeignKey(s => s.GapAnalysisId)
    .OnDelete(DeleteBehavior.Cascade);

// GapAnalysis → Gaps (one-to-many, covers both gaps + critical_gaps via IsCritical)
modelBuilder.Entity<GapAnalysisGap>()
    .HasOne(g => g.GapAnalysis)
    .WithMany(ga => ga.Gaps)
    .HasForeignKey(g => g.GapAnalysisId)
    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}