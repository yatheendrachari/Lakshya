using System.Text.Json.Serialization;

namespace CareerPath.Models.ApiModels
{
    // ---------------------------------------------------------------
    // Response from POST /predict
    // ---------------------------------------------------------------
    public class PythonPredictResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("predictions")]
        public List<PythonCareerPrediction> Predictions { get; set; } = new();
    }

    public class PythonCareerPrediction
    {
        // The career name as returned by Python — e.g. "Software Engineer"
        [JsonPropertyName("career")]
        public string Career { get; set; } = string.Empty;

        // 0–100 float from Python — we cast to int for the view model
        [JsonPropertyName("match_percentage")]
        public float MatchPercentage { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;
    }
}