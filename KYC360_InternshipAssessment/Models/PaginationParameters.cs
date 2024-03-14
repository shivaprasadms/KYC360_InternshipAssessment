using System.ComponentModel.DataAnnotations;

namespace KYC360_InternshipAssessment.Models
{
    public class PaginationParameters
    {
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed.")]
        public int PageNumber { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed.")]
        public int PageSize { get; set; } = 50;
    }
}
