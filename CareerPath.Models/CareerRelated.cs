using System.ComponentModel.DataAnnotations.Schema;
using CareerPath.Models;

public class CareerRelated
{
    public int Id { get; set; }

    public int CareerId { get; set; }
    [ForeignKey(nameof(CareerId))]
    public Career Career { get; set; }

    public int? RelatedCareerId { get; set; }      // nullable for existing data
    [ForeignKey(nameof(RelatedCareerId))]
    public Career? RelatedCareer { get; set; }

    public string? RelatedCareerSlug { get; set; }  // keep for now
    public string? RelatedCareerName { get; set; }  // keep for now
}