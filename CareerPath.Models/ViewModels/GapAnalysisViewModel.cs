using System.Text.Json.Serialization;

namespace CareerPath.Models.ViewModels
{
    public class GapAnalysisViewModel
    {
        [JsonPropertyName("readiness_score")]
        public float ReadinessScore { get; set; }

        [JsonPropertyName("strengths")]
        public List<GapItemViewModel> Strengths { get; set; } = new();

        [JsonPropertyName("gaps")]
        public List<GapItemViewModel> Gaps { get; set; } = new();

        [JsonPropertyName("critical_gaps")]
        public List<GapItemViewModel> CriticalGaps { get; set; } = new();

        [JsonPropertyName("summary")]
        public GapSummaryViewModel Summary { get; set; } = new();
    }

    public class GapItemViewModel
    {
        [JsonPropertyName("feature")]
        public string Feature { get; set; } = string.Empty;

        [JsonPropertyName("student_value")]
        public float StudentValue { get; set; }

        [JsonPropertyName("ideal_value")]
        public float IdealValue { get; set; }

        [JsonPropertyName("gap")]
        public float Gap { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = string.Empty;

        [JsonPropertyName("advice")]
        public string Advice { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class GapSummaryViewModel
    {
        [JsonPropertyName("total_features")]
        public int TotalFeatures { get; set; }

        [JsonPropertyName("features_met")]
        public int FeaturesMet { get; set; }

        [JsonPropertyName("medium_priority_gaps")]
        public int MediumPriorityGaps { get; set; }

        [JsonPropertyName("high_priority_gaps")]
        public int HighPriorityGaps { get; set; }
    }
}