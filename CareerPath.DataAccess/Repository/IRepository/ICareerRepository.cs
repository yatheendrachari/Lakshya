using CareerPath.Models;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface ICareerRepository:IRepository<Career>
{
    void Update(Career obj);
}