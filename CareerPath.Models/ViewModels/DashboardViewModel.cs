namespace CareerPath.Models.ViewModels;

// ═══════════════════════════════════════════════════════════════
//  DASHBOARD VIEW MODEL
// ═══════════════════════════════════════════════════════════════
public class DashboardViewModel
{
    // ── Profile ──────────────────────────────────────────────
    public string  FirstName       { get; set; } = string.Empty;
    public string  LastName        { get; set; } = string.Empty;
    public string? GitHubUsername  { get; set; }
    public DateTime? OnboardedAt   { get; set; }

    // ── Top career ───────────────────────────────────────────
    public string? TopCareerName      { get; set; }
    public string? TopCareerSlug      { get; set; }
    public double  TopCareerMatch     { get; set; }
    public double  TopCareerReadiness { get; set; }
    public string? TopCareerField     { get; set; }

    // ── All predictions mini list ────────────────────────────
    public List<CareerPredictionMini> Predictions { get; set; } = new();

    // ── Gap summary counts ───────────────────────────────────
    public int TotalGaps      { get; set; }
    public int CriticalGaps   { get; set; }
    public int MediumGaps     { get; set; }
    public int TotalStrengths { get; set; }

    // ── Gap detail for inline section ────────────────────────
    public List<DashGapItem>      GapItems      { get; set; } = new();
    public List<DashStrengthItem> StrengthItems { get; set; } = new();

    // ── Chart: radar (15 skills 0-10) ────────────────────────
    public SkillRadarData? RadarData { get; set; }

    // ── Chart: skills vs ideal bar chart ────────────────────
    public List<SkillComparisonItem> SkillComparison { get; set; } = new();

    // ── Chart: confidence scores per skill ──────────────────
    public ConfidenceChartData? ConfidenceData { get; set; }

    // ── Chart: readiness over timeline chart ───────────────
    public List<ReadinessDataPoint> ReadinessTimeline { get; set; } = new();

    // ── Learning path progress ───────────────────────────────
    public string? LatestLearningPathCareer { get; set; }
    public string? LatestLearningPathSlug   { get; set; }
    public int     TotalModules             { get; set; }
    public int     CompletedModules         { get; set; }
    public int     HighPriorityModules      { get; set; }
    public List<DashModuleItem> Modules     { get; set; } = new();

    // ── State flags ──────────────────────────────────────────
    public bool HasAnalysis     { get; set; }
    public bool HasLearningPath { get; set; }
}

// ── Supporting types ─────────────────────────────────────────

public class SkillRadarData
{
    public double CodingSkills              { get; set; }
    public double CommunicationSkills       { get; set; }
    public double ProblemSolvingSkills      { get; set; }
    public double TeamworkSkills            { get; set; }
    public double AnalyticalSkills          { get; set; }
    public double PresentationSkills        { get; set; }
    public double NetworkingSkills          { get; set; }
    public double ResearchExperience        { get; set; }
    public double Internships               { get; set; }
    public double Projects                  { get; set; }
    public double LeadershipPositions       { get; set; }
    public double FieldSpecificCourses      { get; set; }
    public double ExtracurricularActivities { get; set; }
    public double Gpa                       { get; set; }
    public double OverallConfidence         { get; set; }
}

public class SkillComparisonItem
{
    public string Skill        { get; set; } = string.Empty;
    public double StudentValue { get; set; }
    public double IdealValue   { get; set; }
}

public class ConfidenceChartData
{
    public List<string> Labels { get; set; } = new();
    public List<double> Values { get; set; } = new();
}

public class ReadinessDataPoint
{
    public string Date      { get; set; } = string.Empty;
    public double Readiness { get; set; }
}

public class DashModuleItem
{
    public int    Id          { get; set; }
    public string Concept     { get; set; } = string.Empty;
    public string Priority    { get; set; } = string.Empty;
    public bool   IsCompleted { get; set; }
}

public class DashGapItem
{
    public string Feature    { get; set; } = string.Empty;
    public string Priority   { get; set; } = string.Empty;
    public string Advice     { get; set; } = string.Empty;
    public bool   IsCritical { get; set; }
}

public class DashStrengthItem
{
    public string Feature      { get; set; } = string.Empty;
    public string StudentValue { get; set; } = string.Empty;
}