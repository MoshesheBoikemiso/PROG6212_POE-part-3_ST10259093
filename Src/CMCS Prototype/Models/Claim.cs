using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace CMCS_Prototype.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        [Display(Name = "Claim Month")]
        public DateTime ClaimMonth { get; set; }

        [Required]
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Submitted";

        [Display(Name = "Submission Date")]
        public DateTime DateSubmissions { get; set; } = DateTime.Now;

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public List<Document> Documents { get; set; } = new List<Document>();
    }
}