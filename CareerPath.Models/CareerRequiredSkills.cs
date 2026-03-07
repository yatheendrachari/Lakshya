using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPath.Models;

public class CareerRequiredSkills
{
    
    public int Id { get; set; }
    public int CareerId { get; set; }
    [ForeignKey(nameof(CareerId))]   
    public Career Career { get; set; }
    public string? SkillName { get; set; }
    public int Level { get; set; }
    public string? LevelLabel { get; set; }
}