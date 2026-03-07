using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Models.ApiModels;
using CareerPath.Models.ViewModels;
using CareerPath.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CareerPath.Services;

public class PythonApiServices : IPythonApiServices
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PythonApiServices> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy        = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };
    public PythonApiServices(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<GapAnalysisViewModel?> GetGapAnalysisAsync(string careerName, object featureVector)
{
    // step 1 — build request body using the already-fetched feature vector
    var requestBody = new
    {
        career         = careerName,
        feature_vector = featureVector
    };

    // step 2 — call Python API
    var client = _httpClientFactory.CreateClient("PythonApi");

    var response = await client.PostAsync(
        "/gap",
        JsonContent.Create(requestBody, options: _jsonOptions));

    if (!response.IsSuccessStatusCode) return null;

    // step 3 — deserialize wrapper response
    var wrapper = await response.Content.ReadFromJsonAsync<PythonGapResponse>(_jsonOptions);

    if (wrapper?.GapAnalysis is null) return null;

    var result = wrapper.GapAnalysis;

    // step 4 — map inner gap_analysis object to GapAnalysisViewModel
    return new GapAnalysisViewModel
    {
        ReadinessScore = result.ReadinessScore,
        Strengths = result.Strengths.Select(s => new GapItemViewModel
        {
            Feature      = s.Feature,
            StudentValue = s.StudentValue,
            IdealValue   = s.IdealValue,
            Status       = s.Status
        }).ToList(),
        Gaps = result.Gaps.Select(g => new GapItemViewModel
        {
            Feature      = g.Feature,
            StudentValue = g.StudentValue,
            IdealValue   = g.IdealValue,
            Gap          = g.Gap,
            Priority     = g.Priority,
            Advice       = g.Advice
        }).ToList(),
        CriticalGaps = result.CriticalGaps.Select(g => new GapItemViewModel
        {
            Feature      = g.Feature,
            StudentValue = g.StudentValue,
            IdealValue   = g.IdealValue,
            Gap          = g.Gap,
            Priority     = g.Priority,
            Advice       = g.Advice
        }).ToList(),
        Summary = new GapSummaryViewModel
        {
            TotalFeatures      = result.Summary.TotalFeatures,
            FeaturesMet        = result.Summary.FeaturesMet,
            MediumPriorityGaps = result.Summary.MediumPriorityGaps,
            HighPriorityGaps   = result.Summary.HighPriorityGaps
        }
    };
} 
    
    public async Task<OnboardingViewModel?> PartialAnalysisAsync(
    IFormFile? resume,
    string? GitHubUserId,
    string studentId)
{
    var client = _httpClientFactory.CreateClient("PythonApi");

    // --- Step 1: Call /partial-analysis ---
    using var formData = new MultipartFormDataContent();

    if (!string.IsNullOrEmpty(GitHubUserId))
        formData.Add(new StringContent(GitHubUserId), "github_username");

    if (resume != null && resume.Length > 0)
    {
        var stream = resume.OpenReadStream();
        stream.Position = 0;
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        formData.Add(streamContent, "resume", resume.FileName);
    }

    var partialResponse = await client.PostAsync("/partial-analysis", formData);
    if (!partialResponse.IsSuccessStatusCode)
        return null;

    var partialJson = await partialResponse.Content.ReadAsStringAsync();
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
    var analysisResult = JsonSerializer.Deserialize<OnboardingAnalysisResponse>(partialJson, options);
    if (analysisResult == null)
        return null;

    // --- Step 2: Call /generate-questions with the correct body ---
    var questionsRequestBody = new
    {
        has_resume = analysisResult.Has_Resume,
        has_github = analysisResult.Has_Github,
        feature_vector = analysisResult.Feature_Vector   // already a FeatureVector object
    };

    var questionsJson = JsonSerializer.Serialize(questionsRequestBody, options);
    var questionsContent = new StringContent(questionsJson, Encoding.UTF8, "application/json");
    var questionsResponse = await client.PostAsync("/generate-questions", questionsContent);
    if (!questionsResponse.IsSuccessStatusCode)
        return null;

    var questionsResponseJson = await questionsResponse.Content.ReadAsStringAsync();
    var questionsResult = JsonSerializer.Deserialize<GenerateQuestionsResponse>(questionsResponseJson, options);

    // --- Step 3: Map to OnboardingViewModel ---
    return new OnboardingViewModel
    {
        AnalysisResponse = analysisResult,
        Questions = questionsResult?.Data?.Questions ?? new List<DynamicQuestion>()
    };
}

    
    public async Task<FullAnalysisResponse> FullAnalysisAsync(
        IFormFile? resume,
        string? githubUsername,
        Dictionary<string, string> answers)
    {
        var client = _httpClientFactory.CreateClient("PythonApi");

        using var formData = new MultipartFormDataContent();

        // resume file — required by Python endpoint
        if (resume is { Length: > 0 })
        {
            var stream        = resume.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            formData.Add(streamContent, "resume", resume.FileName);
        }

        // github_username — required by Python endpoint
        if (!string.IsNullOrWhiteSpace(githubUsername))
            formData.Add(new StringContent(githubUsername), "github_username");

        // questionnaire answers — serialized as JSON string, Python does json.loads()
        var questionsJson = answers.Any()
            ? JsonSerializer.Serialize(answers, _jsonOptions)
            : "";
        formData.Add(new StringContent(questionsJson), "questionnaire");

        var response = await client.PostAsync("/full-analysis", formData);

        if (!response.IsSuccessStatusCode) return null;

        // Deserialize the full response as a plain object —
        // the controller will serialize and store it as-is in FeatureVector
        return await response.Content.ReadFromJsonAsync<FullAnalysisResponse>(_jsonOptions);
    }

    public async Task<LearningPath?> LearningPathGenerationAsync(
    FeatureVector featureVector,
    string careerName,
    GapAnalysis? gapAnalysis)
{
    var client = _httpClientFactory.CreateClient("PythonApi");

    // ✅ Build complete feature vector — all fields Python expects
    var fvBody = new
    {
        field                      = featureVector.Field,
        gpa                        = featureVector.Gpa,
        extracurricular_activities = featureVector.ExtracurricularActivities,
        internships                = featureVector.Internships,
        projects                   = featureVector.Projects,
        leadership_positions       = featureVector.LeadershipPositions,
        field_specific_courses     = featureVector.FieldSpecificCourses,
        research_experience        = featureVector.ResearchExperience,
        coding_skills              = featureVector.CodingSkills,
        communication_skills       = featureVector.CommunicationSkills,
        problem_solving_skills     = featureVector.ProblemSolvingSkills,
        teamwork_skills            = featureVector.TeamworkSkills,
        analytical_skills          = featureVector.AnalyticalSkills,
        presentation_skills        = featureVector.PresentationSkills,
        networking_skills          = featureVector.NetworkingSkills
    };

    // ✅ Build gap analysis section if available
    object? gapBody = null;
    if (gapAnalysis != null)
    {
        var regularGaps  = gapAnalysis.Gaps.Where(g => !g.IsCritical).ToList();
        var criticalGaps = gapAnalysis.Gaps.Where(g =>  g.IsCritical).ToList();

        gapBody = new
        {
            career         = gapAnalysis.Career,
            readiness_score = gapAnalysis.ReadinessScore,
            gaps = regularGaps.Select(g => new
            {
                feature  = g.Feature,
                priority = g.Priority
            }),
            critical_gaps = criticalGaps.Select(g => new
            {
                feature  = g.Feature,
                priority = g.Priority
            })
        };
    }

    var requestBody = new
    {
        selected_career = careerName,
        include_papers  = featureVector.ResearchExperience,
        feature_vector  = fvBody,
        gap_analysis    = gapBody   // null if not found — Python should handle gracefully
    };

    var response = await client.PostAsJsonAsync("/learning-path", requestBody, _jsonOptions);

    if (!response.IsSuccessStatusCode)
    {
        _logger.LogError("[LearningPath] API returned {Status} for career={Career}",
            response.StatusCode, careerName);
        return null;
    }

    return await response.Content.ReadFromJsonAsync<LearningPath>(_jsonOptions);
}
}