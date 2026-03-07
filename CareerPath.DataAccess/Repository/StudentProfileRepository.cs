using CareerPath.DataAccess.Repository.IRepository;

using CareerPath.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.DataAccess.Repository;

public class StudentProfileRepository:Repository<StudentProfile>,IStudentProfileRepository
{
    ApplicationDbContext _db;
    public StudentProfileRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(StudentProfile profile)
    {
        _db.StudentProfiles.Update(profile);

    }

    public async Task<StudentProfile> GetByStudentId(string studentId)
    {
        return await _db.StudentProfiles
            .FirstOrDefaultAsync(x => x.StudentId == studentId);
    }

    public async Task<StudentProfile> GetWithProfileAsync(string studentId)
    {
        
        return await _db.StudentProfiles
            .FirstOrDefaultAsync(x => x.StudentId == studentId);;
    }

    
}