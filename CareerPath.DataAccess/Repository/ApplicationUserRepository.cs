using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.DataAccess.Repository;

public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
{
    private ApplicationDbContext _db;
    public ApplicationUserRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<ApplicationUser> GetByIdAsync(string? userId)
    {
        var student = await _db.StudentProfiles
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x => x.StudentId == userId);

        return student?.Student;
    }
}