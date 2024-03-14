using System.ComponentModel.DataAnnotations;

namespace KYC360_InternshipAssessment.Models
{
    public class UpdateRequest
    {
        [Required]
        public List<Address>? Addresses { get; set; }

        [Required]
        public bool Deceased { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public List<Name> Names { get; set; }
    }
}
