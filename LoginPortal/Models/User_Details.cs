using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginPortal.Models
{
    [Table("userDetails")]  // Explicitly specify the table name
    public class User_Details
    {
        [Key]
        [Column("user_id")]  // Explicitly specify the column name
        public int UserId { get; set; } // Primary Key

        [Required]
        [Column("email")]  // Explicitly specify the column name
        public string Email { get; set; } = String.Empty; // Unique, Not Null

        [Required]
        [Column("hashed_password")]  // Explicitly specify the column name
        public string HashedPassword { get; set; } = String.Empty; // Not Null

        [Required]
        [Column("first_name")]  // Explicitly specify the column name
        public string FirstName { get; set; } = String.Empty; // Not Null

        [Required]
        [Column("last_name")]  // Explicitly specify the column name
        public string LastName { get; set; } = String.Empty; // Not Null

        [Required]
        [Column("phone_number")]  // Explicitly specify the column name
        public string PhoneNumber { get; set; } = String.Empty;

        [Required]
        [Column("address")]  // Explicitly specify the column name
        public string Address { get; set; } = String.Empty;

        [Required]
        [Column("date_of_birth")]  // Explicitly specify the column name
        public DateTime DateOfBirth { get; set; }

        [Column("profile_picture_url")]  // Explicitly specify the column name
        public string ProfilePictureUrl { get; set; } = String.Empty;

        [Column("is_verified")]  // Explicitly specify the column name
        public bool IsVerified { get; set; } = false; // BIT column, maps to bool By default is set to false

        [Required]
        [Column("role")]  // Explicitly specify the column name
        public string Role { get; set; } = "USER"; // Role can be 'ADMIN' or 'USER', By default is set to 'USER'

        [Column("created_at")]  // Explicitly specify the column name
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default: CURRENT_TIMESTAMP

        [Column("updated_at")]  // Explicitly specify the column name
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;// Default: CURRENT_TIMESTAMP
    }

}
