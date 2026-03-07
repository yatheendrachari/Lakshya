using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CareerPath.Models;

public class ApplicationUser:IdentityUser
{ 
    [Required]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? state { get; set; }
    [Required]
    public string country { get; set; }
    public StudentProfile? Profile { get; set; }

}
