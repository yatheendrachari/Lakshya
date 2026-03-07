using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPath.Models;

public class CareerTopCompanies
{
    public int Id { get; set; }
    public int CareerId { get; set; }
    [ForeignKey(nameof(CareerId))]
    public Career Career { get; set; }
    public string CompanyName { get; set; }
}