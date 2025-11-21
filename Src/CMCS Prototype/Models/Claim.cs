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
        [Range(0.1, 744, ErrorMessage = "Hours must be between 0.1 and 744")]
        public decimal TotalHours { get; set; }

        [Required]
        [Display(Name = "Hourly Rate (R)")]
        [Range(1, 1000, ErrorMessage = "Hourly rate must be between R1 and R1000")]
        public decimal HourlyRate { get; set; } = 180; 

        [Required]
        [Display(Name = "Total Amount (R)")]
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = "Submitted";

        [Display(Name = "Submission Date")]
        public DateTime DateSubmissions { get; set; } = DateTime.Now;
         
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        public int UserID { get; set; }

        public User User { get; set; }

        public List<Document> Documents { get; set; } = new List<Document>();


        public void CalculateTotalAmount()
        { 
            TotalAmount = TotalHours * HourlyRate;
        }
    } 
}