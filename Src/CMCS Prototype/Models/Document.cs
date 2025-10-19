using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_Prototype.Models
{
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public int ClaimID { get; set; }

        public Claim Claim { get; set; }
    }
}