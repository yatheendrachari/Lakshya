using System.ComponentModel.DataAnnotations.Schema;
using CareerPath.Models.ViewModels;

namespace CareerPath.Models
{
    
    public class StudentProfile
    {
        public int Id { get; set; }
        public string StudentId { get; set; }  // FK to ApplicationUser.Id
    
        // onboarding data
        public string? GitHubUsername { get; set; }
        public bool HasResume { get; set; }
        public bool OnboardingCompleted { get; set; }
    
        // Python API results
        [NotMapped]
        public List<DynamicQuestion> Questions { get; set; }
        public DateTime? LastAnalyzedAt { get; set; }
    
        // metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
        // navigation
        public ApplicationUser Student { get; set; }
        
        public List<StudentAnalysisResult> AnalysisResults { get; set; } = [];

    }
}


   
