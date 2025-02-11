using LoginPortal.Models;
using LoginPortal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoginPortal.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Default constructor
        public UserService()
        {
        }
        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public virtual async Task<bool> CheckIfUserExistsAsync(string email)
        {
            return await _context.UserDetails.AnyAsync(u => u.Email == email);
        }

        public virtual async Task<User_Details?> GetUserDetailsAsync(int id)
        {
            return await _context.UserDetails.FindAsync(id);
        }

        public virtual async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _context.UserDetails.AnyAsync(u => u.Email == email);
        }

        public virtual async Task AddUserAsync(User_Details user)
        {
            _context.UserDetails.Add(user);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<List<User_Details>> GetAllUsersAsync()
        {
            return await _context.UserDetails.ToListAsync();
        }

        public virtual async Task<User_Details?> GetUserByEmailAsync(string email)
        {
            return await _context.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
        }

        public virtual async Task<User_Details?> UpdatePasswordAsync(string email, string hashedPassword)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return null;

            user.HashedPassword = hashedPassword;
            await _context.SaveChangesAsync();
            return user;
        }

        public virtual async Task<User_Details?> UpdateUserDetailsAsync(int id, UserUpdateRequest updateRequest)
        {
            var existingUser = await _context.UserDetails.FindAsync(id);
            if (existingUser == null) return null;

            existingUser.FirstName = updateRequest.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updateRequest.LastName ?? existingUser.LastName;
            existingUser.PhoneNumber = updateRequest.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Address = updateRequest.Address ?? existingUser.Address;
            existingUser.ProfilePictureUrl = updateRequest.ProfilePictureUrl ?? existingUser.ProfilePictureUrl;
            existingUser.Role = updateRequest.Role ?? existingUser.Role;
            existingUser.IsVerified = updateRequest.IsVerified;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public virtual async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserDetailsAsync(id);
            if (user == null) return false;

            _context.UserDetails.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _context.UserDetails.AnyAsync(u => u.Email == email);
        }

        public virtual async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? null : await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public virtual async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }

}
