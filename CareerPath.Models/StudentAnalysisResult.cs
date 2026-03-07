using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPath.Models;

// Root table — one per analysis run, linked to StudentProfile
public class StudentAnalysisResult
{
    public int Id { get; set; }
    public required string Status { get; set; }
    public required string RecommendedCareer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK to StudentProfile
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;

    // Children
    public FeatureVector? FeatureVector { get; set; }
    public List<CareerPrediction> CareerPredictions { get; set; } = [];
    public List<GapAnalysis> GapAnalyses { get; set; } = [];
}

// ── Feature Vector ───────────────────────────────────────────────────────────

public class FeatureVector
{
    public int Id { get; set; }
    public required string Field { get; set; }
    public double Gpa { get; set; }
    public int ExtracurricularActivities { get; set; }
    public int Internships { get; set; }
    public int Projects { get; set; }
    public bool LeadershipPositions { get; set; }
    public int FieldSpecificCourses { get; set; }
    public bool ResearchExperience { get; set; }
    public int CodingSkills { get; set; }
    public int CommunicationSkills { get; set; }
    public int ProblemSolvingSkills { get; set; }
    public int TeamworkSkills { get; set; }
    public int AnalyticalSkills { get; set; }
    public int PresentationSkills { get; set; }
    public int NetworkingSkills { get; set; }
    public string? CareerIntent { get; set; }
    public double? OverallProfileConfidence { get; set; }

    // FK to StudentAnalysisResult (one-to-one)
    public int StudentAnalysisResultId { get; set; }
    public StudentAnalysisResult StudentAnalysisResult { get; set; } = null!;

    // Children
    public SourcesUsed? SourcesUsed { get; set; }
    public ConfidenceScores? ConfidenceScores { get; set; }
    public List<InconsistencyDetected> Inconsistencies { get; set; } = [];
}

public class SourcesUsed
{
    public int Id { get; set; }
    public bool Resume { get; set; }
    public bool GitHub { get; set; }
    public bool Questionnaire { get; set; }

    public int FeatureVectorId { get; set; }
    public FeatureVector FeatureVector { get; set; } = null!;
}

public class ConfidenceScores
{
    public int Id { get; set; }
    public double? Gpa { get; set; }
    public double? CodingSkills { get; set; }
    public double? CommunicationSkills { get; set; }
    public double? ProblemSolvingSkills { get; set; }
    public double? TeamworkSkills { get; set; }
    public double? AnalyticalSkills { get; set; }
    public double? PresentationSkills { get; set; }
    public double? NetworkingSkills { get; set; }

    public int FeatureVectorId { get; set; }
    public FeatureVector FeatureVector { get; set; } = null!;
}

public class InconsistencyDetected
{
    public int Id { get; set; }
    public required string Description { get; set; }

    public int FeatureVectorId { get; set; }
    public FeatureVector FeatureVector { get; set; } = null!;
}

// ── Career Predictions ───────────────────────────────────────────────────────

public class CareerPrediction
{
    public int Id { get; set; }
    public required string Career { get; set; }
    public double MatchPercentage { get; set; }
    public required string Field { get; set; }

    public int StudentAnalysisResultId { get; set; }
    public StudentAnalysisResult StudentAnalysisResult { get; set; } = null!;
}

// ── Gap Analysis ─────────────────────────────────────────────────────────────

public class GapAnalysis
{
    public int Id { get; set; }
    public required string Career { get; set; }
    public double ReadinessScore { get; set; }
    public double MatchPercentage { get; set; }

    public int StudentAnalysisResultId { get; set; }
    public StudentAnalysisResult StudentAnalysisResult { get; set; } = null!;

    // Children
    public GapAnalysisSummary? Summary { get; set; }
    public List<GapAnalysisStrength> Strengths { get; set; } = [];
    public List<GapAnalysisGap> Gaps { get; set; } = [];  // includes critical gaps
}

public class GapAnalysisStrength
{
    public int Id { get; set; }
    public required string Feature { get; set; }
    public double StudentValue { get; set; }
    public double IdealValue { get; set; }
    public required string Status { get; set; }

    public int GapAnalysisId { get; set; }
    public GapAnalysis GapAnalysis { get; set; } = null!;
}

public class GapAnalysisGap
{
    public int Id { get; set; }
    public required string Feature { get; set; }
    public double StudentValue { get; set; }
    public double IdealValue { get; set; }
    public double Gap { get; set; }
    public required string? Priority { get; set; }   // "medium" | "high"
    public required string? Advice { get; set; }
    public bool IsCritical { get; set; }            // true = was in critical_gaps array

    public int GapAnalysisId { get; set; }
    public GapAnalysis GapAnalysis { get; set; } = null!;
}

public class GapAnalysisSummary
{
    public int Id { get; set; }
    public int TotalFeatures { get; set; }
    public int FeaturesMet { get; set; }
    public int MediumPriorityGaps { get; set; }
    public int HighPriorityGaps { get; set; }

    public int GapAnalysisId { get; set; }
    public GapAnalysis GapAnalysis { get; set; } = null!;
}