using CareerPath.DataAccess;
using CareerPath.DataAccess.Repository;
using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models;

namespace CareerPath.Data.Repository;

public class CareerRepository:Repository<Career>,ICareerRepository
{
    private ApplicationDbContext _db;
    public CareerRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Career career)
    {
        _db.Career.Update(career);
    }
}