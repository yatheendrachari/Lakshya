namespace CareerPath.Models.ApiModels;

public class PythonLearningPathModel
{
    public  string status { get; set; }
    public LearningPathModel learning_path { get; set; }
}

public class LearningPathModel
{
    public string selected_career { get; set; }
    public string field { get; set; }
    public int total_concepts { get; set; }
    public List<LearningModules>  learning_modules { get; set; }
    public Summary summary  { get; set; }
}

public class LearningModule
{
    public string concept { get; set; }
    public string maps_to_gap { get; set; }
    public string level { get; set; }
    public string priority { get; set; }
    public Resources resources { get; set; }
    public int total_resources { get; set; }
    
}

public class Resources
{
    public List<Videos> videos { get; set; }
    public List<OtherResources>  courses { get; set; }
    public List<OtherResources>? articles { get; set; }
    public List<OtherResources>? papers { get; set; }
}

public class Videos
{
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
}

public class OtherResources
{
    public string? type { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? source { get; set; }
    public string? description { get; set; }
}

public class Summary{
public int high_priority_modules{ get; set; }
public int medium_priority_modules{ get; set; }
public string start_with { get; set; }
public int estimated_modules{ get; set; }
}

