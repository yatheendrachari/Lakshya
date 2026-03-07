namespace CareerPath.Models.ApiModels;

public class PythonApiLearningPathPostBody
{
    public string selected_career { get; set; }
    public bool include_papers { get; set; }
    public featureVector feature_vector { get; set; }
    public GapAnalysis gap_analysis { get; set; }
    public string critical_gaps { get; set; }
}

public class featureVector
{
    public string field { get; set; }
    public float gpa { get; set; }
    public bool reasearch_experience { get; set; }
    
}

// public class GapAnalysis
// {
//     public int career { get; set; }
//     public float readiness_score { get; set; }
//     public List<Gaps>  gaps { get; set; }
// }

public class Gaps
{
    public string feature { get; set; }
    public string priority { get; set; }
}