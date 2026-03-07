using CareerPath.Models;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICareerRepository Career { get;}
    IStudentProfileRepository StudentProfile { get; }
    ILearningPathRepository LearningPath { get; }
    
    IStudentAnalysisResultRepository StudentAnalysisResult { get; }
    
    IApplicationUserRepository ApplicationUser { get; }
    
    // If you don't already have it:
    LearningModulesRepository LearningModules { get; }
    
    Task SaveAsync();
}