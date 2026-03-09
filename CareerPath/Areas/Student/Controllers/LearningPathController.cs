
using System.Diagnostics;
using System.Security.Claims;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Models.ViewModels;
using CareerPath.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPath.Areas.Student.Controllers;

[Area("Student")]
[Authorize]
public class LearningPathController : Controller
{
    private readonly ILogger<LearningPathController> _logger;
    private readonly IPythonApiServices _pythonApiServices;
    private readonly IUnitOfWork _unitOfWork;

    public LearningPathController(
        ILogger<LearningPathController> logger,
        IPythonApiServices pythonApiServices,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _pythonApiServices = pythonApiServices;
        _unitOfWork = unitOfWork;
    }

    // ── GET /Student/LearningPath ────────────────────────────────────────────
    // Shows the 4 career selector cards, no roadmap yet
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = await BuildBaseViewModelAsync();
        return View(vm);
    }

    // ── POST /Student/LearningPath/Generate ─────────────────────────────────
    // Slug comes from the hidden input on the selector card form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(string slug)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (studentId is null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        var vm = await BuildBaseViewModelAsync();
        vm.SelectedSlug = slug;

        var profile = await _unitOfWork.StudentProfile.GetByStudentId(studentId);
        if (profile is null || !profile.OnboardingCompleted)
        {
            TempData["Error"] = "Please complete onboarding first.";
            return View("Index", vm);
        }

        // ✅ Load FULL data — we need both feature vector AND gap analyses
        var analysis = await _unitOfWork.StudentAnalysisResult
            .GetLatestWithFullDataAsync(profile.Id);

        if (analysis?.FeatureVector == null)
        {
            TempData["Error"] = "No profile data found. Please re-run onboarding.";
            return View("Index", vm);
        }

        // ✅ Find the career name from the slug to match against gap analyses
        var career = _unitOfWork.Career.Get(c => c.Slug == slug);
        var careerName = career?.Name ?? slug;

        // ✅ Get the gap analysis for this specific career
        var gapAnalysis = analysis.GapAnalyses
            .FirstOrDefault(g => string.Equals(g.Career, careerName,
                StringComparison.OrdinalIgnoreCase));

        try
        {
            var result = await _pythonApiServices.LearningPathGenerationAsync(
                analysis.FeatureVector, careerName, gapAnalysis);

            if (result is null)
            {
                TempData["Error"] = "Learning path generation failed. Please try again.";
                return View("Index", vm);
            }
            
            try
            {
                // Remove old learning path for this profile+career if one exists
                var existing = await _unitOfWork.LearningPath
                    .GetByProfileAndCareerAsync(profile.Id, careerName);
                if (existing != null)
                    _unitOfWork.LearningPath.Remove(existing);

                result.StudentProfileId = profile.Id;
                _unitOfWork.LearningPath.Add(result);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                // Don't fail the page if save fails — user still sees the result
                _logger.LogError(ex, "[LearningPath] Failed to persist learning path for slug={Slug}", slug);
            }

            vm.GeneratedPath = result;
            return View("Index", vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LearningPath/Generate] slug={Slug}", slug);
            TempData["Error"] = "An error occurred generating your learning path.";
            return View("Index", vm);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    // ── PRIVATE HELPERS ──────────────────────────────────────────────────────

    /// Builds the base VM with the career selector cards populated from the
    /// student's latest analysis result. Safe to call even before any path is generated.
    private async Task<LearningPathPageViewModel> BuildBaseViewModelAsync()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (studentId is null)
            return new LearningPathPageViewModel();

        var profile = await _unitOfWork.StudentProfile.GetByStudentId(studentId);
        if (profile is null)
            return new LearningPathPageViewModel();

        var analysis = await _unitOfWork.StudentAnalysisResult
            .GetLatestWithFullDataAsync(profile.Id);

        if (analysis is null)
            return new LearningPathPageViewModel();

        // Map career predictions → lightweight selector cards
        var options = analysis.CareerPredictions
            .OrderByDescending(p => p.MatchPercentage)
            .Take(4)
            .Select(p =>
            {
                // Try to find the slug from the Career table
                var career = _unitOfWork.Career
                    .Get(c => c.Name == p.Career);

                return new CareerPredictionMini
                {
                    Name            = p.Career,
                    Slug            = career?.Slug ?? SlugifyName(p.Career),
                    MatchPercentage = p.MatchPercentage
                };
            })
            .ToList();

        return new LearningPathPageViewModel { CareerOptions = options };
    }

    /// Fallback slug generation in case Career table lookup fails
    private static string SlugifyName(string name) =>
        name.ToLowerInvariant()
            .Replace(' ', '-')
            .Replace("_", "-");
}