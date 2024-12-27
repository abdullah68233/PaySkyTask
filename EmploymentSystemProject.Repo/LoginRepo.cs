using EmploymentSystemProject.Entities;
using EnploymentSystemProject.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Repo
{
    public class LoginRepo
    {
        private readonly EmploymentDbContext _context;

        public LoginRepo(EmploymentDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAsync(UserLoginDTO registrationDTO)
        {
            var user = new User
            {
                Username = registrationDTO.Username,
                Password = registrationDTO.Password,
                Role = registrationDTO.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByCredentialsAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}
