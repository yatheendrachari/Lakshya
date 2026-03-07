using System.Text.Json.Serialization;

namespace CareerPath.Models.ApiModels
{
    public class PythonGapResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("gap_analysis")]
        public PythonGapAnalysis GapAnalysis { get; set; } = new();
    }

    public class PythonGapAnalysis
    {
        [JsonPropertyName("career")]
        public string Career { get; set; } = string.Empty;

        [JsonPropertyName("readiness_score")]
        public float ReadinessScore { get; set; }

        [JsonPropertyName("strengths")]
        public List<PythonGapItem> Strengths { get; set; } = new();

        [JsonPropertyName("gaps")]
        public List<PythonGapItem> Gaps { get; set; } = new();

        [JsonPropertyName("critical_gaps")]
        public List<PythonGapItem> CriticalGaps { get; set; } = new();

        [JsonPropertyName("summary")]
        public PythonGapSummary Summary { get; set; } = new();
    }

    public class PythonGapItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
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

    public class PythonGapSummary
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