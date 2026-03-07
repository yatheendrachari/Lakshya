// Models/ApiModels/FullAnalysisResponse.cs
namespace CareerPath.Models.ApiModels;

public class FullAnalysisResponse
{
    public string? Status { get; set; }
    public string? RecommendedCareer { get; set; }
    public StudentProfileResponse? StudentProfile { get; set; }
    public List<CareerPredictionResponse>? CareerPredictions { get; set; }
    public List<GapAnalysisResponse>? GapAnalyses { get; set; }
}

public class StudentProfileResponse
{
    public FeatureVectorResponse? FeatureVector { get; set; }
    public SourcesUsedResponse? SourcesUsed { get; set; }
}

public class FeatureVectorResponse
{
    public string? Field { get; set; }
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
    public double OverallProfileConfidence { get; set; }
    public ConfidenceScoresResponse? ConfidenceScores { get; set; }
    public List<string>? InconsistenciesDetected { get; set; }
}

public class ConfidenceScoresResponse
{
    public double Gpa { get; set; }
    public double CodingSkills { get; set; }
    public double CommunicationSkills { get; set; }
    public double ProblemSolvingSkills { get; set; }
    public double TeamworkSkills { get; set; }
    public double AnalyticalSkills { get; set; }
    public double PresentationSkills { get; set; }
    public double NetworkingSkills { get; set; }
}

public class SourcesUsedResponse
{
    public bool Resume { get; set; }
    public bool Github { get; set; }
    public bool Questionnaire { get; set; }
}

public class CareerPredictionResponse
{
    public string Career { get; set; } = string.Empty;
    public double MatchPercentage { get; set; }
    public string Field { get; set; } = string.Empty;
}

public class GapAnalysisResponse
{
    public string Career { get; set; } = string.Empty;
    public double ReadinessScore { get; set; }
    public double MatchPercentage { get; set; }
    public List<GapStrengthResponse>? Strengths { get; set; }
    public List<GapItemResponse>? Gaps { get; set; }
    public List<GapItemResponse>? CriticalGaps { get; set; }
    public GapSummaryResponse? Summary { get; set; }
}

public class GapStrengthResponse
{
    public string Feature { get; set; } = string.Empty;
    public double StudentValue { get; set; }
    public double IdealValue { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GapItemResponse
{
    public string Feature { get; set; } = string.Empty;
    public double StudentValue { get; set; }
    public double IdealValue { get; set; }
    public double Gap { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Advice { get; set; } = string.Empty;
}

public class GapSummaryResponse
{
    public int TotalFeatures { get; set; }
    public int FeaturesMet { get; set; }
    public int MediumPriorityGaps { get; set; }
    public int HighPriorityGaps { get; set; }
}