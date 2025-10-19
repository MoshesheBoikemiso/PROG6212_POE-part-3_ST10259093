using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CMCS_Prototype.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserRole { get; set; }

        public List<Claim> ClaimsSubmitted { get; set; } = new List<Claim>();
    }
}