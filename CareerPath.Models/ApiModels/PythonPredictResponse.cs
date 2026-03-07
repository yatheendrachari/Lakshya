using System.Text.Json.Serialization;

namespace CareerPath.Models.ApiModels
{
    // ------------------------------------------------------------
    // Represents the raw JSON that the Python /predict endpoint
    // returns. Keep this separate from CareerPathViewModel so that
    // the view model stays clean and independent of the API shape.
    // ------------------------------------------------------------
    public class PythonPredictionResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("predictions")]
        public List<PythonPredictionItem> Predictions { get; set; } = new();
    }

    public class PythonPredictionItem
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        // The Python model must return the career slug so we can
        // look up the full Career row in the database.
        [JsonPropertyName("career_slug")]
        public string CareerSlug { get; set; } = string.Empty;

        // 0–100 match percentage
        [JsonPropertyName("match_score")]
        public int MatchScore { get; set; }

        // Feature names the student already meets
        [JsonPropertyName("strengths")]
        public List<string> Strengths { get; set; } = new();

        // Gaps with advice
        [JsonPropertyName("gaps")]
        public List<PythonGapItem> Gaps { get; set; } = new();

        // 0–100 readiness against required skills
        [JsonPropertyName("readiness_score")]
        public int ReadinessScore { get; set; }

        // e.g. "6 months"
        [JsonPropertyName("time_to_ready")]
        public string TimeToReady { get; set; } = string.Empty;
    }

    public class PythonGapPredItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("advice")]
        public string Advice { get; set; } = string.Empty;
    }
}