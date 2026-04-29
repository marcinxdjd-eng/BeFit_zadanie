using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa ćwiczenia musi mieć od 2 do 100 znaków.")]
        [Display(Name = "Nazwa ćwiczenia")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}