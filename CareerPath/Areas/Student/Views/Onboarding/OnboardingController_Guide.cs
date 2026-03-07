// /*
//  * OnboardingController Implementation Guide
//  * 
//  * This is the mental model for how the controller should work.
//  * All form submissions are server-side, JS only handles visual transitions.
//  */
//
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using CareerGuidance.Models.ViewModels;
// using CareerGuidance.Services.Interfaces;
// using System.Security.Claims;
//
// namespace CareerGuidance.Controllers
// {
//     [Authorize]
//     public class OnboardingController : Controller
//     {
//         private readonly IPythonApiService _pythonApiService;
//         private readonly IUnitOfWork _unitOfWork;
//
//         public OnboardingController(IPythonApiService pythonApiService, IUnitOfWork unitOfWork)
//         {
//             _pythonApiService = pythonApiService;
//             _unitOfWork = unitOfWork;
//         }
//
//         // ========================================
//         // GET /Onboarding
//         // Shows step 1 (upload form)
//         // ========================================
//         public IActionResult Index()
//         {
//             // Check if already onboarded
//             var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             var profile = _unitOfWork.StudentProfiles.GetByStudentId(studentId);
//
//             if (profile?.OnboardingCompleted == true)
//             {
//                 // Already onboarded, redirect to home
//                 return RedirectToAction("Index", "Home");
//             }
//
//             // Show onboarding with step 1
//             var viewModel = new OnboardingViewModel
//             {
//                 CurrentStep = 1
//             };
//
//             return View(viewModel);
//         }
//
//         // ========================================
//         // POST /Onboarding/Analyze
//         // Receives: resume (IFormFile), githubUsername (string)
//         // Calls Python /partial-analysis
//         // Returns: Same view with CurrentStep = 2 and questions
//         // ========================================
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Analyze(IFormFile? resume, string? githubUsername)
//         {
//             var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//             // Validate at least one input
//             if (resume == null && string.IsNullOrEmpty(githubUsername))
//             {
//                 var errorModel = new OnboardingViewModel
//                 {
//                     CurrentStep = 1,
//                     ErrorMessage = "Please provide at least a resume or GitHub username."
//                 };
//                 return View("Index", errorModel);
//             }
//
//             // Validate file size if resume provided
//             if (resume != null && resume.Length > 5 * 1024 * 1024) // 5MB
//             {
//                 var errorModel = new OnboardingViewModel
//                 {
//                     CurrentStep = 1,
//                     ErrorMessage = "Resume file must be less than 5MB."
//                 };
//                 return View("Index", errorModel);
//             }
//
//             try
//             {
//                 // Call Python API /partial-analysis
//                 var result = await _pythonApiService.PartialAnalysisAsync(resume, githubUsername);
//
//                 // Get or create student profile
//                 var profile = _unitOfWork.StudentProfiles.GetByStudentId(studentId);
//                 if (profile == null)
//                 {
//                     profile = new StudentProfile
//                     {
//                         StudentId = studentId,
//                         GitHubUsername = githubUsername,
//                         HasResume = resume != null
//                     };
//                     _unitOfWork.StudentProfiles.Add(profile);
//                 }
//                 else
//                 {
//                     profile.GitHubUsername = githubUsername;
//                     profile.HasResume = resume != null;
//                 }
//
//                 // Save feature vector
//                 profile.FeatureVectorJson = JsonSerializer.Serialize(result.FeatureVector);
//                 profile.LastAnalyzedAt = DateTime.UtcNow;
//
//                 await _unitOfWork.SaveAsync();
//
//                 // Map questions to ViewModel
//                 var questions = result.Questions.Select(q => new DynamicQuestion
//                 {
//                     Id = q.Id,
//                     Feature = q.Feature,
//                     Trigger = q.Trigger,
//                     Question = q.Question
//                 }).ToList();
//
//                 // Return view with step 2 (loading) which auto-transitions to step 3
//                 var viewModel = new OnboardingViewModel
//                 {
//                     CurrentStep = 2,
//                     Questions = questions
//                 };
//
//                 return View("Index", viewModel);
//             }
//             catch (Exception ex)
//             {
//                 // Log error
//                 var errorModel = new OnboardingViewModel
//                 {
//                     CurrentStep = 1,
//                     ErrorMessage = "Analysis failed. Please try again."
//                 };
//                 return View("Index", errorModel);
//             }
//         }
//
//         // ========================================
//         // POST /Onboarding/Complete
//         // Receives: answers dictionary from questionnaire form
//         // Calls Python /full-analysis with saved feature_vector + answers
//         // Redirects to home on success
//         // ========================================
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Complete(Dictionary<string, string> answers)
//         {
//             var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//             var profile = _unitOfWork.StudentProfiles.GetByStudentId(studentId);
//
//             if (profile == null || string.IsNullOrEmpty(profile.FeatureVectorJson))
//             {
//                 return RedirectToAction("Index");
//             }
//
//             try
//             {
//                 // Deserialize saved feature vector
//                 var featureVector = JsonSerializer.Deserialize<Dictionary<string, object>>(profile.FeatureVectorJson);
//
//                 // Call Python /full-analysis with feature_vector + questionnaire answers
//                 var result = await _pythonApiService.FullAnalysisAsync(featureVector, answers);
//
//                 // Update profile with final feature vector
//                 profile.FeatureVectorJson = JsonSerializer.Serialize(result.StudentProfile);
//                 profile.OnboardingCompleted = true;
//
//                 await _unitOfWork.SaveAsync();
//
//                 // Redirect to home with success message
//                 TempData["OnboardingComplete"] = "Profile complete! Get your career predictions now.";
//                 return RedirectToAction("Index", "Home");
//             }
//             catch (Exception ex)
//             {
//                 // Log error
//                 TempData["Error"] = "Failed to complete profile. Please try again.";
//                 return RedirectToAction("Index");
//             }
//         }
//     }
// }
//
//
// /*
//  * MENTAL MODEL SUMMARY FOR YOUR BACKEND:
//  * 
//  * Flow 1: User arrives at /Onboarding
//  *   → Controller checks if onboarded → redirects if yes
//  *   → Returns Index view with CurrentStep = 1
//  *   → View shows upload form
//  * 
//  * Flow 2: User submits upload form
//  *   → POST to /Onboarding/Analyze with resume + github
//  *   → Controller validates inputs
//  *   → Calls Python /partial-analysis via PythonApiService
//  *   → Saves feature_vector to StudentProfile.FeatureVectorJson
//  *   → Returns Index view with CurrentStep = 2 and Questions
//  *   → View shows loading step, auto-transitions to questionnaire
//  * 
//  * Flow 3: User submits questionnaire
//  *   → POST to /Onboarding/Complete with answers dictionary
//  *   → Controller retrieves saved feature_vector from DB
//  *   → Calls Python /full-analysis with feature_vector + answers
//  *   → Updates StudentProfile with final result
//  *   → Sets OnboardingCompleted = true
//  *   → Redirects to home with success message
//  * 
//  * Key Points:
//  *   - All processing server-side, no AJAX
//  *   - JS only for visual transitions and file preview
//  *   - Questions rendered by Razor, not dynamically by JS
//  *   - Feature vector stored as JSON string in StudentProfile
//  *   - Questionnaire answers submitted as Dictionary<string, string>
//  */
