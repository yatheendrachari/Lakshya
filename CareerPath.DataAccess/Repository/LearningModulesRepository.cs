using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.DataAccess.Repository;

public class LearningModulesRepository:Repository<LearningModules>,ILearningModulesRepository
{
    ApplicationDbContext _db;
    public LearningModulesRepository(ApplicationDbContext db):base(db)
    {
        _db = db;
    }
    
    public async Task<LearningModules?> GetByIdAsync(int id)
    {
        return await _db.LearningModules.FirstOrDefaultAsync(m => m.Id == id);
    }
    
}