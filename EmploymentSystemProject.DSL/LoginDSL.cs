using EmploymentSystemProject.Entities;
using EmploymentSystemProject.Exceptions;
using EmploymentSystemProject.Helpers;
using EmploymentSystemProject.Repo;
using System;
using System.Threading.Tasks;

namespace EmploymentSystemProject.DSL
{
    public class LoginDSL
    {
        private readonly LoginRepo _loginRepo;
        private readonly TokenManager _tokenManager;

        public LoginDSL(LoginRepo loginRepo, TokenManager tokenManager)
        {
            _loginRepo = loginRepo;
            _tokenManager = tokenManager;
        }

        // User Registration
        public async Task RegisterAsync(UserLoginDTO registrationDTO)
        {
            File_Logger.WriteToLogFile(ActionType.Information, "RegisterAsync method called.");

            if (registrationDTO == null)
            {
                File_Logger.WriteToLogFile(ActionType.Information, "Registration details are null.");
                throw new CustomValidationException("Registration details are required.");
            }

            File_Logger.WriteToLogFile(ActionType.Information, $"Registration attempt for username: {registrationDTO.Username}");

            if (string.IsNullOrWhiteSpace(registrationDTO.Username) ||
                string.IsNullOrWhiteSpace(registrationDTO.Password) ||
                string.IsNullOrWhiteSpace(registrationDTO.Role))
            {
                File_Logger.WriteToLogFile(ActionType.Information, "One or more required fields are empty.");
                throw new CustomValidationException("All fields (Username, Password, Role) are required.");
            }

            if (registrationDTO.Role != "Employer" && registrationDTO.Role != "Applicant")
            {
                File_Logger.WriteToLogFile(ActionType.Information, $"Invalid role: {registrationDTO.Role}");
                throw new CustomValidationException("Invalid role. Role must be 'Employer' or 'Applicant'.");
            }

            try
            {
                await _loginRepo.RegisterAsync(registrationDTO);
                File_Logger.WriteToLogFile(ActionType.Information, $"User registered successfully: {registrationDTO.Username}");
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ex, $"Error registering user: {registrationDTO.Username}");
                throw;
            }
        }

        // User Login
        public async Task<string> LoginAsync(UserLoginDTO loginDTO)
        {
            File_Logger.WriteToLogFile(ActionType.Information, "LoginAsync method called.");

            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Username) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                File_Logger.WriteToLogFile(ActionType.Information, "Username or password is empty.");
                throw new CustomValidationException("Username and password must not be empty.");
            }

            File_Logger.WriteToLogFile(ActionType.Information, $"Login attempt for username: {loginDTO.Username}");

            try
            {
                var user = await _loginRepo.GetUserByCredentialsAsync(loginDTO.Username, loginDTO.Password);

                if (user == null)
                {
                    File_Logger.WriteToLogFile(ActionType.Information, $"Invalid login credentials for username: {loginDTO.Username}");
                    throw new UnauthorizedException("Invalid username or password.");
                }

                File_Logger.WriteToLogFile(ActionType.Information, $"Login successful for username: {loginDTO.Username}. Generating token...");

                var token = _tokenManager.GetToken(user);
                File_Logger.WriteToLogFile(ActionType.Information, $"Token generated successfully for username: {loginDTO.Username}");

                return token.Token;
            }
            catch (Exception ex)
            {
                File_Logger.WriteToLogFile(ActionType.Exception, $"Error during login for username: {loginDTO.Username} with {ex}");
                throw;
            }
        }
    }
}
