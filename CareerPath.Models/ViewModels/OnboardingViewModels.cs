using System.Text.Json.Serialization;

namespace CareerPath.Models.ViewModels
{
    public class OnboardingViewModel
    {
        public int CurrentStep { get; set; } = 1;
        public string? ErrorMessage { get; set; }
        public string? GitHubUsername { get; set; }   // carried through to step 3 form
        public OnboardingAnalysisResponse? AnalysisResponse { get; set; }
        public List<DynamicQuestion> Questions { get; set; } = new();
    }
    

public class OnboardingAnalysisResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("feature_vector")]
    public FeatureVector Feature_Vector { get; set; }

    [JsonPropertyName("has_resume")]
    public bool Has_Resume { get; set; }

    [JsonPropertyName("has_github")]
    public bool Has_Github { get; set; }
}






    public class DynamicQuestion
    {
        public string Id { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}
