using CareerPath.Models;

namespace CareerPath.DataAccess.Repository.IRepository;

public interface IStudentAnalysisResultRepository:IRepository<StudentAnalysisResult>
{
    void Update(StudentProfile obj);

    Task<StudentAnalysisResult?> GetLatestWithFullDataAsync(int studentProfileId);

    Task<StudentAnalysisResult?> GetLatestWithFeatureVectorAsync(int studentProfileId);
    
// IStudentAnalysisResultRepository  
    Task<List<StudentAnalysisResult>> GetAllWithGapAnalysesAsync(int profileId);


}