using System.ComponentModel.DataAnnotations.Schema;

namespace CareerPath.Models;

public class CareerProgressionSteps
{
    public int Id  { get; set; }
    public string Title { get; set; }
    public int CareerId { get; set; }
    [ForeignKey("CareerId")]
    public Career Career { get; set; }
    public string YearsRange { get; set; }
    public int OrderIndex { get; set; }
}