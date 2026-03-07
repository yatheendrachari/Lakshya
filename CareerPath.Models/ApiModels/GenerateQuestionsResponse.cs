using System.Text.Json.Serialization;
using CareerPath.Models.ViewModels;

namespace CareerPath.Models.ApiModels;

// GenerateQuestionsResponse.cs
public class GenerateQuestionsResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("data")]
    public GenerateQuestionsData Data { get; set; }
}

public class GenerateQuestionsData
{
    [JsonPropertyName("question_count")]
    public int QuestionCount { get; set; }

    [JsonPropertyName("questions")]
    public List<DynamicQuestion> Questions { get; set; } = new();
}