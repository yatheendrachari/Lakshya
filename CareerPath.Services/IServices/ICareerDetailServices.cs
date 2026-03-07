using CareerPath.Models.ViewModels;

namespace CareerPath.Services.IServices;

public interface ICareerDetailServices
{
    public Task<CareerDetailViewModel?> GetCareerDetailAsync(string slug);

}