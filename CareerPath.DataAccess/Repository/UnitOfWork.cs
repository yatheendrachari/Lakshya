using CareerPath.Data;
using CareerPath.Data.Repository;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;

namespace CareerPath.DataAccess.Repository;

public class UnitOfWork:IUnitOfWork
{
    private ApplicationDbContext _db;
    public ICareerRepository Career { get; private set; }
    public IStudentProfileRepository StudentProfile { get; private set; }
    public ILearningPathRepository LearningPath { get; private set; }
    public IStudentAnalysisResultRepository StudentAnalysisResult { get; private set; }
    public IApplicationUserRepository ApplicationUser{get; private set;}
    public LearningModulesRepository LearningModules { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Career=new CareerRepository(_db);
        StudentProfile=new StudentProfileRepository(_db);
        LearningPath=new LearningPathRepository(_db);
        StudentAnalysisResult = new StudentAnalysisResultRepository(_db);
        ApplicationUser=new ApplicationUserRepository(_db);
        LearningModules = new LearningModulesRepository(_db);
    }

    
    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
    
}