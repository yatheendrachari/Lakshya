using System.Security.Claims;
using System.Text.Json;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Models.ApiModels;
using CareerPath.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerPath.Services.IServices;
using GapAnalysis = CareerPath.Models.GapAnalysis;

namespace CareerPath.Areas.Student.Controllers;

[Area("Student")]
[Authorize]
public class OnboardingController : Controller
{
    private readonly IPythonApiServices _pythonApiService;
    private readonly IUnitOfWork _unitOfWork;

    public OnboardingController(IPythonApiServices pythonApiService, IUnitOfWork unitOfWork)
    {
        _pythonApiService = pythonApiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await _unitOfWork.StudentProfile.GetByStudentId(studentId);

        if (profile?.OnboardingCompleted == true)
            return RedirectToAction("Index", "Home");

        return View(new OnboardingViewModel { CurrentStep = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(IFormFile? resume, string? githubUsername)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (resume == null && string.IsNullOrEmpty(githubUsername))
            return View("Index", new OnboardingViewModel
            {
                CurrentStep  = 1,
                ErrorMessage = "Please provide at least a resume or GitHub username."
            });

        if (resume != null && resume.Length > 5 * 1024 * 1024)
            return View("Index", new OnboardingViewModel
            {
                CurrentStep  = 1,
                ErrorMessage = "Resume file must be less than 5MB."
            });

        try
        {
            var result = await _pythonApiService.PartialAnalysisAsync(resume, githubUsername, studentId);

            // ✅ Only update StudentProfile metadata — no JSON stored here anymore
            var profile = await _unitOfWork.StudentProfile.GetByStudentId(studentId);
            if (profile == null)
            {
                profile = new StudentProfile
                {
                    StudentId      = studentId,
                    GitHubUsername = githubUsername,
                    HasResume      = resume != null
                };
                _unitOfWork.StudentProfile.Add(profile);
            }
            else
            {
                profile.GitHubUsername = githubUsername;
                profile.HasResume      = resume != null;
            }

            await _unitOfWork.SaveAsync();

            return View("Index", new OnboardingViewModel
            {
                CurrentStep    = 2,
                GitHubUsername = githubUsername,
                Questions      = result?.Questions ?? new List<DynamicQuestion>()
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Onboarding/Analyze] {ex}");
            return View("Index", new OnboardingViewModel
            {
                CurrentStep  = 1,
                ErrorMessage = $"Analysis failed: {ex.Message}"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(
        IFormFile? resume,
        string? githubUsername,
        Dictionary<string, string> answers)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile   = await _unitOfWork.StudentProfile.GetByStudentId(studentId);

        if (profile == null) return RedirectToAction("Index");

        if (resume == null && string.IsNullOrEmpty(githubUsername))
        {
            TempData["Error"] = "Resume or GitHub username is required to complete your profile.";
            return RedirectToAction("Index");
        }

        try
        {
            var result = await _pythonApiService.FullAnalysisAsync(resume, githubUsername, answers);

            if (result == null)
            {
                TempData["Error"] = "Analysis failed. Please try again.";
                return RedirectToAction("Index");
            }

            // ✅ Save structured StudentAnalysisResult instead of raw JSON string
            var analysisResult = MapToStudentAnalysisResult(result, profile.Id);
            _unitOfWork.StudentAnalysisResult.Add(analysisResult);

            // ✅ Only update profile metadata
            profile.OnboardingCompleted = true;
            profile.LastAnalyzedAt      = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            TempData["OnboardingComplete"] = "Profile complete! Get your career predictions now.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Onboarding/Complete] {ex}");
            TempData["Error"] = $"Failed to complete profile: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    // ✅ Maps the raw API response object into your normalized EF models
    private StudentAnalysisResult MapToStudentAnalysisResult(FullAnalysisResponse result, int profileId)
    {
        var fv = result.StudentProfile?.FeatureVector;

        return new StudentAnalysisResult
        {
            StudentProfileId  = profileId,
            Status            = result.Status ?? "success",
            RecommendedCareer = result.RecommendedCareer ?? string.Empty,
            CreatedAt         = DateTime.UtcNow,

            FeatureVector = fv == null ? null : new FeatureVector
            {
                Field                    = fv.Field ?? string.Empty,
                Gpa                      = fv.Gpa,
                ExtracurricularActivities = fv.ExtracurricularActivities,
                Internships              = fv.Internships,
                Projects                 = fv.Projects,
                LeadershipPositions      = fv.LeadershipPositions,
                FieldSpecificCourses     = fv.FieldSpecificCourses,
                ResearchExperience       = fv.ResearchExperience,
                CodingSkills             = fv.CodingSkills,
                CommunicationSkills      = fv.CommunicationSkills,
                ProblemSolvingSkills     = fv.ProblemSolvingSkills,
                TeamworkSkills           = fv.TeamworkSkills,
                AnalyticalSkills         = fv.AnalyticalSkills,
                PresentationSkills       = fv.PresentationSkills,
                NetworkingSkills         = fv.NetworkingSkills,
                CareerIntent             = fv.CareerIntent,
                OverallProfileConfidence = fv.OverallProfileConfidence,

                SourcesUsed = result.StudentProfile?.SourcesUsed == null ? null : new SourcesUsed
                {
                    Resume        = result.StudentProfile.SourcesUsed.Resume,
                    GitHub        = result.StudentProfile.SourcesUsed.Github,
                    Questionnaire = result.StudentProfile.SourcesUsed.Questionnaire
                },

                ConfidenceScores = fv.ConfidenceScores == null ? null : new ConfidenceScores
                {
                    Gpa                  = fv.ConfidenceScores.Gpa,
                    CodingSkills         = fv.ConfidenceScores.CodingSkills,
                    CommunicationSkills  = fv.ConfidenceScores.CommunicationSkills,
                    ProblemSolvingSkills = fv.ConfidenceScores.ProblemSolvingSkills,
                    TeamworkSkills       = fv.ConfidenceScores.TeamworkSkills,
                    AnalyticalSkills     = fv.ConfidenceScores.AnalyticalSkills,
                    PresentationSkills   = fv.ConfidenceScores.PresentationSkills,
                    NetworkingSkills     = fv.ConfidenceScores.NetworkingSkills
                },

                Inconsistencies = (fv.InconsistenciesDetected ?? new List<string>())
                    .Select(i => new InconsistencyDetected { Description = i })
                    .ToList()
            },

            CareerPredictions = (result.CareerPredictions ?? new List<CareerPredictionResponse>())
                .Select(p => new CareerPrediction
                {
                    Career           = p.Career,
                    MatchPercentage  = p.MatchPercentage,
                    Field            = p.Field
                }).ToList(),

            GapAnalyses = (result.GapAnalyses ?? new List<GapAnalysisResponse>())
                .Select(ga => new GapAnalysis
                {
                    Career         = ga.Career,
                    ReadinessScore = ga.ReadinessScore,
                    MatchPercentage = ga.MatchPercentage,

                    Summary = ga.Summary == null ? null : new GapAnalysisSummary
                    {
                        TotalFeatures      = ga.Summary.TotalFeatures,
                        FeaturesMet        = ga.Summary.FeaturesMet,
                        MediumPriorityGaps = ga.Summary.MediumPriorityGaps,
                        HighPriorityGaps   = ga.Summary.HighPriorityGaps
                    },

                    Strengths = (ga.Strengths ?? new List<GapStrengthResponse>())
                        .Select(s => new GapAnalysisStrength
                        {
                            Feature      = s.Feature,
                            StudentValue = s.StudentValue,
                            IdealValue   = s.IdealValue,
                            Status       = s.Status
                        }).ToList(),

                    // ✅ Gaps + CriticalGaps merged into one table with IsCritical flag
                    Gaps = (ga.Gaps ?? new List<GapItemResponse>())
                        .Select(g => new GapAnalysisGap
                        {
                            Feature      = g.Feature,
                            StudentValue = g.StudentValue,
                            IdealValue   = g.IdealValue,
                            Gap          = g.Gap,
                            Priority     = g.Priority,
                            Advice       = g.Advice,
                            IsCritical   = false
                        })
                        .Concat((ga.CriticalGaps ?? new List<GapItemResponse>())
                            .Select(g => new GapAnalysisGap
                            {
                                Feature      = g.Feature,
                                StudentValue = g.StudentValue,
                                IdealValue   = g.IdealValue,
                                Gap          = g.Gap,
                                Priority     = g.Priority,
                                Advice       = g.Advice,
                                IsCritical   = true
                            }))
                        .ToList()
                }).ToList()
        };
    }
}