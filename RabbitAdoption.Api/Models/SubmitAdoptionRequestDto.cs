using System.ComponentModel.DataAnnotations;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Api.Models
{
    public class SubmitAdoptionRequestDto
    {
        [Required]
        public string AdopterName { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public string Color { get; set; }

        public int? Age { get; set; }

        [Required]
        [EnumDataType(typeof(AdoptionPriority))]
        public AdoptionPriority Priority { get; set; }

        public int YearsRabbitExperience { get; set; }
    }
}
