using System.Security.Claims;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Models.ApiModels;
using CareerPath.Models.ViewModels;
using CareerPath.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPath.Areas.Student.Controllers;

[Area("Student")]
[Authorize]
public class DashboardController : Controller
{
    private readonly IUnitOfWork                  _unitOfWork;
    private readonly ILogger<DashboardController> _logger;
    private readonly IPythonApiServices           _pythonApiServices;

    public DashboardController(
        IUnitOfWork unitOfWork,
        ILogger<DashboardController> logger,
        IPythonApiServices pythonApiServices)
    {
        _unitOfWork        = unitOfWork;
        _logger            = logger;
        _pythonApiServices = pythonApiServices;
    }

    // ── GET /Student/Dashboard ───────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        var appUser = await _unitOfWork.ApplicationUser.GetByIdAsync(userId);
        if (appUser is null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        var vm = new DashboardViewModel
        {
            FirstName = appUser.FirstName ?? string.Empty,
            LastName  = appUser.LastName  ?? string.Empty,
        };

        var profile = await _unitOfWork.StudentProfile.GetByStudentId(userId);
        if (profile is null || !profile.OnboardingCompleted)
            return View(vm);

        vm.GitHubUsername = profile.GitHubUsername;
        vm.OnboardedAt    = profile.LastAnalyzedAt;

        // ── Latest analysis ──────────────────────────────────────────────
        var analysis = await _unitOfWork.StudentAnalysisResult
            .GetLatestWithFullDataAsync(profile.Id);

        if (analysis is null) return View(vm);
        vm.HasAnalysis = true;

        // ── Career predictions ───────────────────────────────────────────
        var predictions = analysis.CareerPredictions
            .OrderByDescending(p => p.MatchPercentage)
            .ToList();

        if (predictions.Any())
        {
            var top    = predictions.First();
            var career = _unitOfWork.Career.Get(c => c.Name == top.Career);
            var topGap = analysis.GapAnalyses
                .FirstOrDefault(g => string.Equals(g.Career, top.Career,
                                     StringComparison.OrdinalIgnoreCase));

            vm.TopCareerName      = top.Career;
            vm.TopCareerSlug      = career?.Slug ?? Slugify(top.Career);
            vm.TopCareerMatch     = top.MatchPercentage;
            vm.TopCareerReadiness = topGap?.ReadinessScore ?? 0;
            vm.TopCareerField     = top.Field;

            vm.Predictions = predictions.Take(4).Select(p =>
            {
                var c = _unitOfWork.Career.Get(x => x.Name == p.Career);
                return new CareerPredictionMini
                {
                    Name            = p.Career,
                    Slug            = c?.Slug ?? Slugify(p.Career),
                    MatchPercentage = p.MatchPercentage
                };
            }).ToList();

            // ── Gap summary + detail ─────────────────────────────────────
            if (topGap != null)
            {
                vm.CriticalGaps   = topGap.Gaps.Count(g => g.IsCritical);
                vm.MediumGaps     = topGap.Gaps.Count(g => !g.IsCritical);
                vm.TotalGaps      = vm.CriticalGaps + vm.MediumGaps;
                vm.TotalStrengths = topGap.Strengths.Count;

                vm.GapItems = topGap.Gaps
                    .OrderByDescending(g => g.IsCritical)
                    .Select(g => new DashGapItem
                    {
                        Feature    = FormatFeature(g.Feature),
                        Priority   = g.Priority ?? (g.IsCritical ? "high" : "medium"),
                        Advice     = g.Advice   ?? string.Empty,
                        IsCritical = g.IsCritical
                    }).ToList();

                vm.StrengthItems = topGap.Strengths
                    .Select(s => new DashStrengthItem
                    {
                        Feature      = FormatFeature(s.Feature),
                        StudentValue = s.StudentValue.ToString() ?? string.Empty
                    }).ToList();

                // ── Bar chart: student vs ideal ──────────────────────────
                var allItems = topGap.Gaps
                    .Concat(topGap.Strengths.Select(s => new GapAnalysisGap
                    {
                        Feature      = s.Feature,
                        StudentValue = s.StudentValue,
                        IdealValue   = s.IdealValue,
                        Priority = null,
                        Advice = null
                        
                    }))
                    .Where(x => !string.IsNullOrEmpty(x.Feature))
                    .Take(10);

                vm.SkillComparison = allItems.Select(x => new SkillComparisonItem
                {
                    Skill        = FormatFeature(x.Feature ?? string.Empty),
                    StudentValue = ParseDouble(x.StudentValue),
                    IdealValue   = ParseDouble(x.IdealValue)
                }).ToList();
            }
        }

        // ── Radar data ───────────────────────────────────────────────────
        var fv = analysis.FeatureVector;
        if (fv != null)
        {
            vm.RadarData = new SkillRadarData
            {
                CodingSkills              = Clamp10(fv.CodingSkills),
                CommunicationSkills       = Clamp10(fv.CommunicationSkills),
                ProblemSolvingSkills      = Clamp10(fv.ProblemSolvingSkills),
                TeamworkSkills            = Clamp10(fv.TeamworkSkills),
                AnalyticalSkills          = Clamp10(fv.AnalyticalSkills),
                PresentationSkills        = Clamp10(fv.PresentationSkills),
                NetworkingSkills          = Clamp10(fv.NetworkingSkills),
                ResearchExperience        = fv.ResearchExperience ? 10 : 0,
                Internships               = Clamp10(fv.Internships * 2.5),
                Projects                  = Clamp10(fv.Projects * 2.0),
                LeadershipPositions       = Clamp10((fv.LeadershipPositions ? 1 : 0) * 3.3),
                FieldSpecificCourses      = Clamp10(fv.FieldSpecificCourses),
                ExtracurricularActivities = Clamp10(fv.ExtracurricularActivities * 2.0),
                Gpa                       = Clamp10(fv.Gpa),
                OverallConfidence         = Clamp10((fv.OverallProfileConfidence ?? 0) * 10)
            };

            // ── Confidence scores chart ──────────────────────────────────
            if (fv.ConfidenceScores != null)
            {
                var cs = fv.ConfidenceScores;
                var confMap = new Dictionary<string, double>
                {
                    ["GPA"]           = cs.Gpa                  ?? 0,
                    ["Coding"]        = cs.CodingSkills          ?? 0,
                    ["Communication"] = cs.CommunicationSkills   ?? 0,
                    ["Problem Solving"] = cs.ProblemSolvingSkills ?? 0,
                    ["Teamwork"]      = cs.TeamworkSkills         ?? 0,
                    ["Analytical"]    = cs.AnalyticalSkills       ?? 0,
                    ["Presentation"]  = cs.PresentationSkills     ?? 0,
                    ["Networking"]    = cs.NetworkingSkills        ?? 0
                };

                vm.ConfidenceData = new ConfidenceChartData
                {
                    Labels = confMap.Keys.ToList(),
                    Values = confMap.Values.Select(v => Math.Round(v * 100, 1)).ToList()
                };
            }
        }

        // ── Readiness over time ──────────────────────────────────────────
        try
        {
            var allAnalyses = await _unitOfWork.StudentAnalysisResult
                .GetAllWithGapAnalysesAsync(profile.Id);

            vm.ReadinessTimeline = allAnalyses
                .OrderBy(a => a.CreatedAt)
                .Select(a =>
                {
                    var topGapForDate = a.GapAnalyses
                        .OrderByDescending(g => g.MatchPercentage)
                        .FirstOrDefault();
                    return new ReadinessDataPoint
                    {
                        Date      = a.CreatedAt.ToString("MMM d"),
                        Readiness = topGapForDate?.ReadinessScore ?? 0
                    };
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Dashboard] Could not load readiness timeline");
        }

        // ── Learning path progress ───────────────────────────────────────
        try
        {
            var lp = await _unitOfWork.LearningPath.GetLatestByProfileAsync(profile.Id);

            if (lp?.learning_path_model != null)
            {
                var mods = lp.learning_path_model.learning_modules ?? new List<LearningModules>();

                vm.HasLearningPath           = true;
                vm.LatestLearningPathCareer  = lp.learning_path_model.selected_career;
                vm.LatestLearningPathSlug    = Slugify(lp.learning_path_model.selected_career ?? string.Empty);
                vm.TotalModules              = mods.Count;
                vm.CompletedModules          = mods.Count(m => m.IsCompleted);
                vm.HighPriorityModules = mods.Count(m =>
                    string.Equals(m.priority, "high", StringComparison.OrdinalIgnoreCase));

                vm.Modules = mods.Select(m => new DashModuleItem
                {
                    Id          = m.Id,
                    Concept     = m.concept   ?? string.Empty,
                    Priority    = m.priority  ?? "low",
                    IsCompleted = m.IsCompleted
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Dashboard] Could not load learning path");
        }

        return View(vm);
    }

    // ── POST /Student/Dashboard/UpdateProfile ───────────────────────────
    // Called from the update modal — re-runs analysis with new resume/GitHub
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(IFormFile? resume, string? githubUsername)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return RedirectToAction("Index");

        var profile = await _unitOfWork.StudentProfile.GetByStudentId(userId);
        if (profile is null) return RedirectToAction("Index");

        if (resume == null && string.IsNullOrWhiteSpace(githubUsername))
        {
            TempData["Error"] = "Please provide a resume or GitHub username to update.";
            return RedirectToAction("Index");
        }

        try
        {
            // Re-run full analysis with updated inputs and empty answers
            // (Python API uses stored data to fill gaps)
            var result = await _pythonApiServices.FullAnalysisAsync(
                resume,
                string.IsNullOrWhiteSpace(githubUsername) ? profile.GitHubUsername : githubUsername,
                new Dictionary<string, string>());

            if (result is null)
            {
                TempData["Error"] = "Profile update failed. Please try again.";
                return RedirectToAction("Index");
            }

            // Save new analysis result
            var analysisResult = MapToStudentAnalysisResult(result, profile.Id);
            _unitOfWork.StudentAnalysisResult.Add(analysisResult);

            // Update metadata
            if (!string.IsNullOrWhiteSpace(githubUsername))
                profile.GitHubUsername = githubUsername;
            if (resume != null)
                profile.HasResume = true;

            profile.LastAnalyzedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Profile updated successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Dashboard/UpdateProfile]");
            TempData["Error"] = "An error occurred updating your profile.";
        }

        return RedirectToAction("Index");
    }

    // ── POST /Student/Dashboard/ToggleModule ────────────────────────────
    // AJAX endpoint — returns JSON
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleModule(int moduleId)
    {
        try
        {
            var module = await _unitOfWork.LearningModules.GetByIdAsync(moduleId);
            if (module is null) return NotFound();

            module.IsCompleted = !module.IsCompleted;
            module.CompletedAt = module.IsCompleted ? DateTime.UtcNow : null;
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, isCompleted = module.IsCompleted });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Dashboard/ToggleModule] moduleId={Id}", moduleId);
            return Json(new { success = false, error = ex.Message });
        }
    }

    // ── Private helpers ──────────────────────────────────────────────────

    private static double Clamp10(double v) => Math.Clamp(v, 0, 10);

    private static string Slugify(string s) =>
        s.ToLowerInvariant().Replace(' ', '-').Replace("_", "-");

    private static string FormatFeature(string f) =>
        string.Join(' ', f.Split('_')
            .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w));

    private static double ParseDouble(object? v)
    {
        if (v is null) return 0;
        return double.TryParse(v.ToString(), out var d) ? Math.Round(d, 2) : 0;
    }

    private static StudentAnalysisResult MapToStudentAnalysisResult(
        FullAnalysisResponse result, int profileId)
    {
        var fv = result.StudentProfile?.FeatureVector;
        return new StudentAnalysisResult
        {
            StudentProfileId  = profileId,
            Status            = result.Status            ?? "success",
            RecommendedCareer = result.RecommendedCareer ?? string.Empty,
            CreatedAt         = DateTime.UtcNow,
            FeatureVector = fv == null ? null : new FeatureVector
            {
                Field                     = fv.Field ?? string.Empty,
                Gpa                       = fv.Gpa,
                ExtracurricularActivities = fv.ExtracurricularActivities,
                Internships               = fv.Internships,
                Projects                  = fv.Projects,
                LeadershipPositions       = fv.LeadershipPositions,
                FieldSpecificCourses      = fv.FieldSpecificCourses,
                ResearchExperience        = fv.ResearchExperience,
                CodingSkills              = fv.CodingSkills,
                CommunicationSkills       = fv.CommunicationSkills,
                ProblemSolvingSkills      = fv.ProblemSolvingSkills,
                TeamworkSkills            = fv.TeamworkSkills,
                AnalyticalSkills          = fv.AnalyticalSkills,
                PresentationSkills        = fv.PresentationSkills,
                NetworkingSkills          = fv.NetworkingSkills,
                CareerIntent              = fv.CareerIntent,
                OverallProfileConfidence  = fv.OverallProfileConfidence,
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
            CareerPredictions = (result.CareerPredictions ?? new())
                .Select(p => new CareerPrediction
                {
                    Career          = p.Career,
                    MatchPercentage = p.MatchPercentage,
                    Field           = p.Field
                }).ToList(),
            GapAnalyses = (result.GapAnalyses ?? new())
                .Select(ga => new GapAnalysis
                {
                    Career          = ga.Career,
                    ReadinessScore  = ga.ReadinessScore,
                    MatchPercentage = ga.MatchPercentage,
                    Summary = ga.Summary == null ? null : new GapAnalysisSummary
                    {
                        TotalFeatures      = ga.Summary.TotalFeatures,
                        FeaturesMet        = ga.Summary.FeaturesMet,
                        MediumPriorityGaps = ga.Summary.MediumPriorityGaps,
                        HighPriorityGaps   = ga.Summary.HighPriorityGaps
                    },
                    Strengths = (ga.Strengths ?? new())
                        .Select(s => new GapAnalysisStrength
                        {
                            Feature      = s.Feature,
                            StudentValue = s.StudentValue,
                            IdealValue   = s.IdealValue,
                            Status       = s.Status
                        }).ToList(),
                    Gaps = (ga.Gaps ?? new())
                        .Select(g => new GapAnalysisGap
                        {
                            Feature      = g.Feature, StudentValue = g.StudentValue,
                            IdealValue   = g.IdealValue, Gap = g.Gap,
                            Priority     = g.Priority, Advice = g.Advice, IsCritical = false
                        })
                        .Concat((ga.CriticalGaps ?? new())
                            .Select(g => new GapAnalysisGap
                            {
                                Feature      = g.Feature, StudentValue = g.StudentValue,
                                IdealValue   = g.IdealValue, Gap = g.Gap,
                                Priority     = g.Priority, Advice = g.Advice, IsCritical = true
                            }))
                        .ToList()
                }).ToList()
        };
    }
}