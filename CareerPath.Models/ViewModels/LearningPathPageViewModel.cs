namespace CareerPath.Models.ViewModels;

/// <summary>
/// Passed to the LearningPath/Index view.
/// CareerOptions always populated; GeneratedPath only when a slug was POSTed.
/// </summary>
public class LearningPathPageViewModel
{
    /// <summary>Top career predictions shown as selector cards.</summary>
    public List<CareerPredictionMini> CareerOptions { get; set; } = [];

    /// <summary>The slug that was submitted (used to highlight the active card).</summary>
    public string? SelectedSlug { get; set; }

    /// <summary>The generated learning path — null until a career is selected.</summary>
    public LearningPath? GeneratedPath { get; set; }
}

/// <summary>Lightweight career option for the selector row.</summary>
public class CareerPredictionMini
{
    public string Name            { get; set; } = string.Empty;
    public string Slug            { get; set; } = string.Empty;
    public double MatchPercentage { get; set; }
}