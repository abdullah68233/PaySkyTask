using EmploymentSystemProject.Entities;
using EmploymentSystemProject.Exceptions;
using EmploymentSystemProject.Repo;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystemProject.Helpers
{
    public class TokenManager
    {
        private const string JwtSecret = "IkNKRlZlOWlzSmpkRmE5b1NzU0FxRll6WXBmdFBLTW0=";
        private readonly LoginRepo _loginRepo;
        public TokenManager(LoginRepo loginRepo)
        {
            _loginRepo = loginRepo;
        }
        public UserTokenDTO GetToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User details are required to generate a token.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new UserTokenDTO(tokenHandler.WriteToken(token), tokenDescriptor.Expires.GetValueOrDefault(), DateTime.UtcNow);
        }

        private UserTokenDTO CreateToken(UserLoginDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecret);

            // Include claims for user information and role
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new UserTokenDTO(
                tokenHandler.WriteToken(token),
                tokenDescriptor.Expires.GetValueOrDefault(),
                DateTime.UtcNow
            );
        }

        private void ValidateUserCredentials(UserLoginDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                throw new CustomValidationException("Username and Password must not be empty.");

            // Use LoginRepo to retrieve user details
            var userDetails = _loginRepo.GetUserByCredentialsAsync(user.Username, user.Password).Result;
            if (userDetails == null)
                throw new UnauthorizedException("Invalid username or password.");

            if (string.IsNullOrEmpty(userDetails.Role) || (userDetails.Role != "Employer" && userDetails.Role != "Applicant"))
                throw new CustomValidationException("Invalid role. Role must be 'Employer' or 'Applicant'.");
        }



        public bool Validate(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    if (string.IsNullOrEmpty(role) || (role != "Employer" && role != "Applicant"))
                        throw new UnauthorizedException("Invalid role in token.");

                    return true;
                }

                throw new UnauthorizedException("Invalid token format.");
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedException("Token has expired.");
            }
            catch (Exception)
            {
                throw new UnauthorizedException("Invalid token.");
            }
        }
    }
}
