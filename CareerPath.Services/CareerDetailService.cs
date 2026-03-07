using CareerPath.DataAccess.Repository.IRepository;
using CareerPath.Models.ViewModels;
using CareerPath.Services.IServices;

namespace CareerPath.Services;

public class CareerDetailService:ICareerDetailServices
{
    public readonly IUnitOfWork _unitOfWork;
    public CareerDetailService(IUnitOfWork unitOfWork)
    {
        _unitOfWork=unitOfWork;
    }
    public async Task<CareerDetailViewModel?> GetCareerDetailAsync(string slug)
        {
            // ask repository for entity with all related tables loaded
            var career =  _unitOfWork.Career.Get(c=>c.Slug==slug,includeProperties: "Responsibilities,RequiredSkills,ProgressionSteps,TopCompanies,RelatedCareers");

            if (career == null)
                return null;

            // map entity → ViewModel
            // the service is the only place this conversion happens
            return new CareerDetailViewModel
            {
                CareerName      = career.Name,
                Field           = career.Field,
                SubField        = career.SubField ?? string.Empty,
                FieldIcon       = career.FieldIcon ?? "📋",
                Slug            = career.Slug,
                Description     = career.Description ?? string.Empty,
                DemandLevel     = career.DemandLevel ?? "Medium",
                ExperienceLevel = career.ExperienceLevel ?? "Entry",
                WorkMode        = career.WorkMode ?? "Hybrid",
                AverageSalary   = career.AverageSalary ?? "Not available",
                JobOpenings     = career.JobOpenings ?? "Not available",
                GrowthRate      = career.GrowthRate ?? "Not available",
                TypicalDegree   = career.TypicalDegree ?? "Not specified",

                // map List<CareerResponsibility> → List<string>
                Responsibilities = career.Responsibilities
                    .Select(r => r.Responsibility)
                    .ToList(),

                // map List<CareerRequiredSkill> → List<SkillViewModel>
                RequiredSkills = career.RequiredSkills
                    .Select(s => new SkillViewModel
                    {
                        Name       = s.SkillName,
                        Level      = s.Level,
                        LevelLabel = s.LevelLabel ?? string.Empty
                    })
                    .ToList(),

                // map List<CareerProgressionStep> → List<ProgressionStepViewModel>
                // already ordered by OrderIndex in the repository query
                CareerProgression = career.ProgressionSteps
                    .Select(p => new ProgressionStepViewModel
                    {
                        Title      = p.Title,
                        YearsRange = p.YearsRange ?? string.Empty
                    })
                    .ToList(),

                // map List<CareerTopCompany> → List<string>
                TopCompanies = career.TopCompanies
                    .Select(c => c.CompanyName)
                    .ToList(),

                // map List<CareerRelated> → List<RelatedCareerViewModel>
                RelatedCareers = career.RelatedCareers
                    .Select(r => new RelatedCareerViewModel
                    {
                        Name = r.RelatedCareerName,
                        Slug = r.RelatedCareerSlug
                    })
                    .ToList(),

                // gap analysis is null until student requests it
                GapAnalysis = null
            };
        }
    
}