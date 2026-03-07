using CareerPath.Models;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface IApplicationUserRepository:IRepository<ApplicationUser>
{
      Task<ApplicationUser> GetByIdAsync(string? userId);

}