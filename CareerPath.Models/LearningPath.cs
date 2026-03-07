using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Models;

public class LearningPath
{
    public int Id { get; set; }
    public  string status { get; set; }
    public int StudentProfileId { get; set; }          
    public StudentProfile? StudentProfile { get; set; }
    [JsonPropertyName("learning_path")]
    public LearningPathModel? learning_path_model { get; set; }
    
}

public class LearningPathModel
{
    public int Id { get; set; }
    public string selected_career { get; set; }
    public string field { get; set; }
    public int total_concepts { get; set; }
    public List<LearningModules>  learning_modules { get; set; }
    
    public int SummaryId { get; set; }
    [ForeignKey("SummaryId")]
    public Summary Summary  { get; set; }
    
    public int LearningPathId { get; set; }
    [ForeignKey("LearningPathId")]
    public LearningPath LearningPath { get; set; }
    
}

public class LearningModules
{
    public int Id { get; set; }
    public string concept { get; set; }
    public string maps_to_gap { get; set; }
    public string level { get; set; }
    public string priority { get; set; }
    public int total_resources { get; set; }
    public bool IsCompleted { get; set; } = false;  
    public DateTime? CompletedAt { get; set; }
    
    public int ResourceId { get; set; }
    [ForeignKey("ResourceId")]
    public Resources Resources { get; set; }
    public int LearningPathModelId { get; set; }
    [ForeignKey("LearningPathModelId")]
    public LearningPathModel  LearningPathModel { get; set; }
}

public class Resources
{
    public int Id { get; set; }
    
    public LearningModules? LearningModule { get; set; }

    
    public List<Videos>? Videos { get; set; } = [];
    public List<Course>?  Courses { get; set; }= [];
    public List<Article>? Articles { get; set; }= [];
    public List<Paper>? Papers { get; set; }= [];
    
}

public class Videos
{
    public int Id { get; set; }
    public string? type { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? description { get; set; }
    public string? thumbnail { get; set; }
    public string? published_at { get; set; }
    public int? views { get; set; }
    public bool? is_fresh { get; set; }
    public string? source { get; set; }
    public string? channel { get; set; }
    
    
    public int ResourcesId { get; set; }     
    [ForeignKey("ResourcesId")]
    public Resources Resource { get; set; }
}

public class OtherResource
{
    public int Id { get; set; }
    public string? type { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? source { get; set; }
    public string? description { get; set; }
    
    public int ResourcesId { get; set; }
    [ForeignKey("ResourcesId")]
    public Resources Resource { get; set; }
}

public class Course : OtherResource { }
public class Article : OtherResource { }
public class Paper : OtherResource { }

public class Summary{
    public int Id { get; set; }
    public int high_priority_modules{ get; set; }
    public int medium_priority_modules{ get; set; }
    public string start_with { get; set; }
    public int estimated_modules{ get; set; }
    
    public LearningPathModel? LearningPathModel { get; set; }

}

