namespace BeFit.Models
{
    public class ExerciseStatisticsViewModel
    {
        public string ExerciseTypeName { get; set; } = string.Empty;
        public int TimesPerformed { get; set; }
        public int TotalReps { get; set; }
        public double AverageWeight { get; set; }
        public int MaxWeight { get; set; }
    }
}