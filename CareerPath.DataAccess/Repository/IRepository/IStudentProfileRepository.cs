using CareerPath.Models;
using CareerPath.DataAccess.Repository.IRepository;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface IStudentProfileRepository:IRepository<StudentProfile>
{
    void Update(StudentProfile obj);
    Task<StudentProfile> GetByStudentId(string studentId);
    Task<StudentProfile> GetWithProfileAsync(string studentId);
}