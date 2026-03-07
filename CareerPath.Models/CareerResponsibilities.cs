using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPath.Models;

public class CareerResponsibilities
{
    public int Id { get; set; }
    public string Responsibility{ get; set; }
    public int CareerId { get; set; }
    [ForeignKey(nameof(CareerId))]
    public Career Career { get; set; }
}