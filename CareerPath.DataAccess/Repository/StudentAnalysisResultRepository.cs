using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CareerPath.DataAccess.Repository;

public class StudentAnalysisResultRepository:Repository<StudentAnalysisResult>,IStudentAnalysisResultRepository
{
    ApplicationDbContext _db;
    
    public StudentAnalysisResultRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(StudentProfile obj)
    {
        _db.StudentProfiles.Update(obj);
    }
    
    public async Task<StudentAnalysisResult?> GetLatestWithFeatureVectorAsync(int studentProfileId)
    {
        return await _db.StudentAnalysisResults
            .Include(r => r.FeatureVector)
            .ThenInclude(fv => fv.ConfidenceScores)
            .Where(r => r.StudentProfileId == studentProfileId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }
    
    // Interface

// Implementation
    public async Task<StudentAnalysisResult?> GetLatestWithFullDataAsync(int studentProfileId)
    {
        return await _db.StudentAnalysisResults
            .Include(r => r.FeatureVector)
            .Include(r => r.CareerPredictions)
            .Include(r => r.GapAnalyses)
            .ThenInclude(ga => ga.Strengths)
            .Include(r => r.GapAnalyses)
            .ThenInclude(ga => ga.Gaps)
            .Where(r => r.StudentProfileId == studentProfileId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<StudentAnalysisResult>> GetAllWithGapAnalysesAsync(int profileId)
    {
        return await _db.StudentAnalysisResults
            .Include(a => a.GapAnalyses)
            .ThenInclude(g => g.Gaps)
            .Where(a => a.StudentProfileId == profileId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }
}