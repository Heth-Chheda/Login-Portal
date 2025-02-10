using System.ComponentModel.DataAnnotations;

namespace LoginPortal.Models
{
    public class VerificationTokens
    {
        [Key]
        public int TokenId { get; set; }  // Primary key (Identity column)
        public int UserId { get; set; }  // Foreign key to User_Details
        [Required]

        public string Token { get; set; } = String.Empty;
        [Required]

        public string VerificationType { get; set; } = String.Empty;  // 'EMAIL' or 'PHONE'

        public bool IsUsed { get; set; }
        [Required]

        public DateTime CreatedAt { get; set; }
        [Required]

        public DateTime ExpiresAt { get; set; }

        public  virtual User_Details? User { get; set; }  // Navigation property (if needed for related data)
    }
}
