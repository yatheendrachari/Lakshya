using System.Security.Claims;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPath.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize]
    public class CareerPathController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CareerPathController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CareerPathViewModel { HasPredictions = false });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate()
        {
            // ── 1. Auth check ────────────────────────────────────────────────
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            // ── 2. Load profile ──────────────────────────────────────────────
            var profile = await _unitOfWork.StudentProfile.GetByStudentId(userId);
            if (profile is null || !profile.OnboardingCompleted)
            {
                TempData["Error"] = "Please complete onboarding before generating career predictions.";
                return RedirectToAction("Index", "Onboarding");
            }

            // ── 3. Load latest analysis result with everything included ──────
            var analysis = await _unitOfWork.StudentAnalysisResult
                .GetLatestWithFullDataAsync(profile.Id);

            Console.WriteLine($"[Generate] profile.Id={profile.Id}, analysis={analysis?.Id}, predictions={analysis?.CareerPredictions?.Count}");

            
            if (analysis == null || !analysis.CareerPredictions.Any())
            {
                TempData["Error"] = "No career predictions found. Please re-run onboarding.";
                return View("Index", new CareerPathViewModel { HasPredictions = false });
            }

            // ── 4. Index gap analyses by career name for quick lookup ────────
            var gapByCareer = analysis.GapAnalyses
                .ToDictionary(g => g.Career, StringComparer.OrdinalIgnoreCase);

            // ── 5. Enrich with Career DB data ────────────────────────────────
            var careerNames = analysis.CareerPredictions.Select(p => p.Career).ToList();

            var dbCareers = _unitOfWork.Career
                .GetAll(
                    c => careerNames.Contains(c.Name),
                    includeProperties: "RequiredSkills,ProgressionSteps,Responsibilities,TopCompanies"
                )
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            // ── 6. Build view model cards ────────────────────────────────────
            var cards = analysis.CareerPredictions
                .OrderByDescending(p => p.MatchPercentage)
                .Select((item, index) =>
                {
                    dbCareers.TryGetValue(item.Career, out var career);
                    gapByCareer.TryGetValue(item.Career, out var gap);

                    var strengths = gap?.Strengths
                        .Select(s => FormatFeatureName(s.Feature))
                        .ToList() ?? new();

                    // ✅ IsCritical flag distinguishes critical vs medium gaps
                    var gaps = (gap?.Gaps ?? new())
                        .OrderByDescending(g => g.IsCritical)
                        .Select(g => new GapItem
                        {
                            Name   = FormatFeatureName(g.Feature),
                            Advice = g.Advice
                        })
                        .ToList();

                    var requiredSkills = career?.RequiredSkills?
                        .Select(s => new SkillViewModel
                        {
                            Name       = s.SkillName,
                            Level      = s.Level,
                            LevelLabel = s.LevelLabel
                        })
                        .ToList() ?? new();

                    var progression = career?.ProgressionSteps?
                        .Select(p => new ProgressionStepViewModel
                        {
                            Title      = p.Title,
                            YearsRange = p.YearsRange
                        })
                        .ToList() ?? new();

                    return new CareerPredictionCardViewModel
                    {
                        Rank              = index + 1,
                        Name              = career?.Name            ?? item.Career,
                        Slug              = career?.Slug            ?? string.Empty,
                        Match             = (int)Math.Round(item.MatchPercentage),
                        Field             = career?.Field           ?? item.Field,
                        SubField          = career?.SubField        ?? string.Empty,
                        FieldIcon         = career?.FieldIcon       ?? string.Empty,
                        Description       = career?.Description     ?? string.Empty,
                        AverageSalary     = career?.AverageSalary   ?? string.Empty,
                        Salary            = career?.AverageSalary   ?? string.Empty,
                        DemandLevel       = career?.DemandLevel     ?? string.Empty,
                        Growth            = career?.GrowthRate      ?? string.Empty,
                        JobOpenings       = career?.JobOpenings     ?? string.Empty,
                        Degree            = career?.TypicalDegree   ?? string.Empty,
                        Experience        = career?.ExperienceLevel ?? string.Empty,
                        WorkMode          = career?.WorkMode        ?? string.Empty,
                        Strengths         = strengths,
                        Gaps              = gaps,
                        Readiness         = (int)Math.Round(gap?.ReadinessScore ?? 0),
                        TimeToReady       = string.Empty,
                        Responsibilities = career?.Responsibilities?
                            .Select(r => r.Responsibility)
                            .ToList() ?? new(),

                        TopCompanies = career?.TopCompanies?
                            .Select(c => c.CompanyName)
                            .ToList() ?? new(),                        RequiredSkills    = requiredSkills,
                        CareerProgression = progression,
                    };
                })
                .ToList();

            return View("Index", new CareerPathViewModel
            {
                HasPredictions = true,
                Predictions    = cards
            });
        }

        private static string FormatFeatureName(string feature) =>
            string.Join(' ', feature.Split('_')
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w));
    }
}