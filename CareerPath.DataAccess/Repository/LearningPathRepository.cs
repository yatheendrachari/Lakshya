using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.DataAccess.Repository;

public class LearningPathRepository:Repository<LearningPath>,ILearningPathRepository
{
    private ApplicationDbContext _db; 
    public LearningPathRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }

    public void Update(LearningPath learningPath)
    {
        _db.Update(learningPath);
    }
    
    // Interface

// Implementation
    public async Task<LearningPath?> GetByProfileAndCareerAsync(int profileId, string careerName)
    {
        return await _db.LearningPaths
            .Include(lp => lp.learning_path_model)
            .Where(lp => lp.StudentProfileId == profileId
                         && lp.learning_path_model != null
                         && lp.learning_path_model.selected_career == careerName)
            .FirstOrDefaultAsync();
    }
    
    public async Task<LearningPath?> GetLatestByProfileAsync(int profileId)
    {
        return await _db.LearningPaths
            .Include(lp => lp.learning_path_model)
            .ThenInclude(lpm => lpm.learning_modules)
            .ThenInclude(m => m.Resources)
            .Include(lp => lp.learning_path_model)
            .ThenInclude(lpm => lpm.Summary)
            .Where(lp => lp.StudentProfileId == profileId)
            .OrderByDescending(lp => lp.Id)
            .FirstOrDefaultAsync();
    }
}