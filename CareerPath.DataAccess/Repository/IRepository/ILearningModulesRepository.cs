using CareerPath.Models;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface ILearningModulesRepository:IRepository<LearningModules>
{
    Task<LearningModules?> GetByIdAsync(int id);

}