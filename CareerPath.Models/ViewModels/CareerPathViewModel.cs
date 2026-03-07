




// Models/CareerPathViewModel.cs

using System.Collections.ObjectModel;
using CareerPath.Models.ViewModels; // gives us SkillViewModel, ProgressionStepViewModel

namespace CareerPath.Models.ViewModels
{
    public class CareerPathViewModel
    {
        public bool HasPredictions { get; set; } = false;
        public List<CareerPredictionCardViewModel> Predictions { get; set; } = new();
    }

    public class CareerPredictionCardViewModel
    {
        // ── Core match fields ──────────────────────────────────────
        public int    Rank          { get; set; }
        public string Name          { get; set; } = string.Empty;
        public string Slug          { get; set; } = string.Empty;
        public int    Match         { get; set; }   // 0–100
        public string Field         { get; set; } = string.Empty;
        public string SubField      { get; set; } = string.Empty;
        public string FieldIcon     { get; set; } = string.Empty;  // emoji, e.g. "💻"

        // ── Match-specific ─────────────────────────────────────────
        public List<string>  Strengths { get; set; } = new();
        public List<GapItem> Gaps      { get; set; } = new();
        public int    Readiness        { get; set; }   // 0–100 student readiness
        public string TimeToReady      { get; set; } = string.Empty;  // "6 months"

        // ── Salary / market ────────────────────────────────────────
        public string Salary        { get; set; } = string.Empty;  // "$120K – $160K"
        public string AverageSalary { get; set; } = string.Empty;  // "$140K"
        public string DemandLevel   { get; set; } = string.Empty;  // "High"|"Medium"|"Low"
        public string Growth        { get; set; } = string.Empty;  // "+25% by 2030"
        public string JobOpenings   { get; set; } = string.Empty;  // "50,000+ openings"

        // ── Career detail fields (for the right-side panel) ────────
        public string Description   { get; set; } = string.Empty;
        public string Degree        { get; set; } = string.Empty;
        public string Experience    { get; set; } = string.Empty;
        public string WorkMode      { get; set; } = string.Empty;

        public List<string>                  Responsibilities   { get; set; } = new();
        public List<SkillViewModel>          RequiredSkills     { get; set; } = new();
        public List<ProgressionStepViewModel> CareerProgression { get; set; } = new();
        public List<string>                  TopCompanies       { get; set; } = new();
    }

    public class GapItem
    {
        public string Name   { get; set; } = string.Empty;
        public string Advice { get; set; } = string.Empty;
    }
}