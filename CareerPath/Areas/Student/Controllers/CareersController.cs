using System.Security.Claims;
using System.Text.Json;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using CareerPath.Models.ViewModels;
using CareerPath.Services;
using CareerPath.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPath.Areas.Student.Controllers;

[Area("Student")]
public class CareersController : Controller
{
    private readonly IPythonApiServices  _pythonApiService;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly CareerDetailService _careerService;

    public CareersController(
        IUnitOfWork          unitOfWork,
        CareerDetailService  careerService,
        IPythonApiServices   pythonApiService)
    {
        _unitOfWork       = unitOfWork;
        _careerService    = careerService;
        _pythonApiService = pythonApiService;
    }

    // -------------------------------------------------------------------------
    // GET /Student/Careers
    // -------------------------------------------------------------------------
    public IActionResult Index()
    {
        var careerList = _unitOfWork.Career.GetAll(
            includeProperties: "RequiredSkills"   // tells EF to JOIN the skills table
        ).ToList();

        var vm = careerList.Select(c => new CareerCatalogItemViewModel
        {
            Name        = c.Name,
            Field       = c.Field,
            FieldIcon   = c.FieldIcon,
            Slug        = c.Slug,
            DemandLevel = c.DemandLevel,
            Description = c.Description,
            RequiredSkills = c.RequiredSkills
                .Take(4)                          // only top 4 on the card
                .Select(s => new SkillViewModel
                {
                    Name       = s.SkillName ?? string.Empty,
                    Level      = s.Level,
                    LevelLabel = s.LevelLabel ?? string.Empty
                }).ToList()
        }).ToList();

        return View(vm);
    }

    // -------------------------------------------------------------------------
    // GET /Student/Careers/Details/{slug}
    // -------------------------------------------------------------------------
    public async Task<IActionResult> Details(string slug)
    {
        var vm = await _careerService.GetCareerDetailAsync(slug);
        if (vm == null) return NotFound();
        return View(vm);
    }

    // -------------------------------------------------------------------------
    // POST /Student/Careers/GapAnalysis
    //
    // Responsibility split:
    //   CONTROLLER  — DB access (student fetch, feature vector deserialization)
    //   SERVICE     — HTTP communication with Python API
    //   VIEW        — renders _GapAnalysisResult partial when GapAnalysis != null
    // -------------------------------------------------------------------------
    [HttpPost]
[Authorize]
public async Task<IActionResult> GapAnalysis(string careerName, string slug)
{
    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(studentId))
        return RedirectToAction("Login", "Account", new { area = "Identity" });

    var careerDetail = await _careerService.GetCareerDetailAsync(slug);
    if (careerDetail == null) return NotFound();

    // ✅ Get profile with latest analysis result + feature vector
    var profile = await _unitOfWork.StudentProfile.GetByStudentId(studentId);
    if (profile == null)
    {
        TempData["Error"] = "Please complete your profile first.";
        return RedirectToAction("Index", "Onboarding", new { area = "Student" });
    }

    // ✅ Get the most recent analysis result with feature vector loaded
    var latestAnalysis = await _unitOfWork.StudentAnalysisResult
        .GetLatestWithFeatureVectorAsync(profile.Id);

    if (latestAnalysis?.FeatureVector == null)
    {
        TempData["Error"] = "Please complete your profile first to run a gap analysis.";
        return RedirectToAction("Index", "Onboarding", new { area = "Student" });
    }

    // ✅ Reconstruct the feature vector dictionary from the DB entity
    // Python's /gap endpoint expects these exact snake_case keys
    var fv = latestAnalysis.FeatureVector;
    var featureVector = new Dictionary<string, object>
    {
        ["field"]                      = fv.Field,
        ["gpa"]                        = fv.Gpa,
        ["extracurricular_activities"] = fv.ExtracurricularActivities,
        ["internships"]                = fv.Internships,
        ["projects"]                   = fv.Projects,
        ["leadership_positions"]       = fv.LeadershipPositions,
        ["field_specific_courses"]     = fv.FieldSpecificCourses,
        ["research_experience"]        = fv.ResearchExperience,
        ["coding_skills"]              = fv.CodingSkills,
        ["communication_skills"]       = fv.CommunicationSkills,
        ["problem_solving_skills"]     = fv.ProblemSolvingSkills,
        ["teamwork_skills"]            = fv.TeamworkSkills,
        ["analytical_skills"]          = fv.AnalyticalSkills,
        ["presentation_skills"]        = fv.PresentationSkills,
        ["networking_skills"]          = fv.NetworkingSkills,
        ["career_intent"]              = fv.CareerIntent ?? string.Empty
    };

    try
    {
        var gapAnalysis = await _pythonApiService.GetGapAnalysisAsync(careerName, featureVector);

        if (gapAnalysis == null)
        {
            TempData["Error"] = "The analysis service returned an empty result. Please try again.";
            return View("Details", careerDetail);
        }

        careerDetail.GapAnalysis = gapAnalysis;
        return View("Details", careerDetail);
    }
    catch (HttpRequestException)
    {
        TempData["Error"] = "Could not reach the analysis service. Please try again later.";
        return View("Details", careerDetail);
    }
    catch (Exception)
    {
        TempData["Error"] = "Gap analysis failed. Please try again.";
        return View("Details", careerDetail);
    }
}

    
}

