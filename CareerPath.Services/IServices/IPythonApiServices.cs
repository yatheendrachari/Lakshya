using CareerPath.Models;
using CareerPath.Models.ApiModels;
using CareerPath.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CareerPath.Services.IServices;

public interface IPythonApiServices
{
    Task<GapAnalysisViewModel?> GetGapAnalysisAsync(
        string careerName,
        object featureVector);
    public Task<OnboardingViewModel> PartialAnalysisAsync(IFormFile resume,string? GitHubUserId, string? studentId);

    public Task<FullAnalysisResponse> FullAnalysisAsync(IFormFile resume, string? GitHubUserId,
        Dictionary<string, string> answers);
    
    Task<LearningPath?> LearningPathGenerationAsync(
        FeatureVector featureVector,
        string careerName,
        GapAnalysis? gapAnalysis);}





