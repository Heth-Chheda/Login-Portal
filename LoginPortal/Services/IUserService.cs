using LoginPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoginPortal.Services
{
    public interface IUserService
    {
        Task<bool> CheckIfUserExistsAsync(string email);
        Task<User_Details?> GetUserDetailsAsync(int id);
        Task<bool> IsEmailTakenAsync(string email);
        Task AddUserAsync(User_Details user);
        Task<List<User_Details>> GetAllUsersAsync();
        Task<User_Details?> GetUserByEmailAsync(string email);
        Task<User_Details?> UpdatePasswordAsync(string email, string hashedPassword);
        Task<User_Details?> UpdateUserDetailsAsync(int id, UserUpdateRequest updateRequest);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
