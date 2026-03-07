
namespace CareerPath.Models.ViewModels
{
    // ------------------------------------------------------------------
    // Used by: Careers/Detail view
    // ------------------------------------------------------------------
    public class CareerDetailViewModel
    {
        public string CareerName        { get; set; } = string.Empty;
        public string Field             { get; set; } = string.Empty;
        public string SubField          { get; set; } = string.Empty;
        public string FieldIcon         { get; set; } = string.Empty;
        public string Slug              { get; set; } = string.Empty;
        public string Description       { get; set; } = string.Empty;
        public string DemandLevel       { get; set; } = string.Empty;
        public string ExperienceLevel   { get; set; } = string.Empty;
        public string WorkMode          { get; set; } = string.Empty;
        public string AverageSalary     { get; set; } = string.Empty;
        public string JobOpenings       { get; set; } = string.Empty;
        public string GrowthRate        { get; set; } = string.Empty;
        public string TypicalDegree     { get; set; } = string.Empty;

        public List<string> Responsibilities            { get; set; } = new();
        public List<SkillViewModel> RequiredSkills      { get; set; } = new();
        public List<ProgressionStepViewModel> CareerProgression { get; set; } = new();
        public List<string> TopCompanies                { get; set; } = new();
        public List<RelatedCareerViewModel> RelatedCareers { get; set; } = new();

        // null until student clicks Run Gap Analysis
        public GapAnalysisViewModel? GapAnalysis        { get; set; }
    }

    // ------------------------------------------------------------------
    // Used by: Careers/Index (catalog) view
    // lightweight — only what the catalog card needs
    // ------------------------------------------------------------------
    public class CareerCatalogItemViewModel
    {
        public string Name          { get; set; } = string.Empty;
        public string Field         { get; set; } = string.Empty;
        public string FieldIcon     { get; set; } = string.Empty;
        public string Slug          { get; set; } = string.Empty;
        public string DemandLevel   { get; set; } = string.Empty;
        public string Description   { get; set; } = string.Empty;
        public List<SkillViewModel> RequiredSkills { get; set; } = new(); // ✅ add this
    }

    // ------------------------------------------------------------------
    // Nested ViewModels — used inside CareerDetailViewModel
    // ------------------------------------------------------------------
    public class SkillViewModel
    {
        public string Name          { get; set; } = string.Empty;
        public int Level            { get; set; }
        public string LevelLabel    { get; set; } = string.Empty;
    }

    public class ProgressionStepViewModel
    {
        public string Title         { get; set; } = string.Empty;
        public string YearsRange    { get; set; } = string.Empty;
    }

    public class RelatedCareerViewModel
    {
        public string Name          { get; set; } = string.Empty;
        public string Slug          { get; set; } = string.Empty;
    }

    
}