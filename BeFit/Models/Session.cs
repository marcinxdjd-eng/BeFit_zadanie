using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    public class Session : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
        [Display(Name = "Data i czas rozpoczęcia")]
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana.")]
        [Display(Name = "Data i czas zakończenia")]
        [DataType(DataType.DateTime)]
        public DateTime End { get; set; }

        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (End <= Start)
            {
                yield return new ValidationResult(
                    "Data zakończenia musi być późniejsza niż data rozpoczęcia.",
                    new[] { nameof(End) });
            }
        }
    }
}