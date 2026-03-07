using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CareerPath.Models.ViewModels;

namespace CareerPath.Models;

public class Career
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Field { get; set; }
    public string SubField { get; set; }
    public string FieldIcon { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string DemandLevel { get; set; }
    public string ExperienceLevel { get; set; }
    public string WorkMode { get; set; }
    public string AverageSalary  { get; set; }
    public string JobOpenings { get; set; }
    public string GrowthRate  { get; set; }
    public string TypicalDegree  { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<CareerResponsibilities> Responsibilities { get; set; } = new ();
    public ICollection<CareerRequiredSkills> RequiredSkills { get; set; } = new List<CareerRequiredSkills>();
    public ICollection<CareerProgressionSteps> ProgressionSteps { get; set; } = new List<CareerProgressionSteps>();
    public List<CareerTopCompanies> TopCompanies { get; set; } = new List<CareerTopCompanies>();
    public ICollection<CareerRelated> RelatedCareers { get; set; } = new List<CareerRelated>();
}