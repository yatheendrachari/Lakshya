using CareerPath.Models;
using CareerPath.Models.ApiModels;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface ILearningPathRepository:IRepository<LearningPath>
{
    public void Update(LearningPath learningPath);
    
    Task<LearningPath?> GetByProfileAndCareerAsync(int profileId, string careerName);


// ILearningModuleRepository (new or existing)
    Task<LearningPath?> GetLatestByProfileAsync(int profileId);
}