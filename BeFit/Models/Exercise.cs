using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Range(0, 1000, ErrorMessage = "Obciążenie musi być w zakresie od 0 do 1000 kg.")]
        [Display(Name = "Obciążenie (kg)")]
        public int Weight { get; set; }

        [Range(1, 100, ErrorMessage = "Liczba serii musi być w zakresie od 1 do 100.")]
        [Display(Name = "Liczba serii")]
        public int NumOfSeries { get; set; }

        [Range(1, 1000, ErrorMessage = "Liczba powtórzeń w serii musi być w zakresie od 1 do 1000.")]
        [Display(Name = "Liczba powtórzeń w serii")]
        public int NumOfReps { get; set; }

        [Required(ErrorMessage = "Wybór typu ćwiczenia jest wymagany.")]
        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Display(Name = "Typ ćwiczenia")]
        public ExerciseType? ExerciseType { get; set; }

        [Required(ErrorMessage = "Wybór sesji treningowej jest wymagany.")]
        [Display(Name = "Sesja treningowa")]
        public int SessionId { get; set; }

        [Display(Name = "Sesja treningowa")]
        public Session? Session { get; set; }

        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }
    }
}