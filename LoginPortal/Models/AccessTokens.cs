using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginPortal.Models
{
    [Table("access_tokens")]
    public class AccessTokens
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }  // Primary key (Identity column)
        [Column("user_id")]
        public int UserId { get; set; }  // Foreign key to User_Details
        [Column("access_token")]
        public string AccessToken { get; set; } = String.Empty;
        [Column("refresh_token")]
        public string RefreshToken { get; set; } = String.Empty;
        [Column("issued_at")]
        public DateTime IssuedAt { get; set; }
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        public  virtual User_Details? User { get; set; }  // Navigation property (if needed for related data)
    }
}
