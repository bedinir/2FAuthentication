using Microsoft.AspNetCore.Identity;
using TwoStepAuthentication.Models;
using TwoStepAuthentication.Repositories;
using TwoStepAuthentication.Services.Interfaces;

namespace TwoStepAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly I2FactorAuthentication _factorAuthentication;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        public AuthService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,I2FactorAuthentication factorAuthentication, RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _factorAuthentication = factorAuthentication;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public async Task<ResponseData<LoginResponse>> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Username);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (user == null || isValid == false)
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }


            if (user.Is2FAEnabled)
            {
                try
                {
                    var code = await _factorAuthentication.GenerateAndSendTwoFactorTokenAsync(user);
                    return new ResponseData<LoginResponse>
                    {
                        Success = true,
                        Message = "Two-factor authentication required.",
                        Data = new LoginResponse
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Is2FAEnabled = true
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseData<LoginResponse>
                    {
                        Success = false,
                        Message = $"Error generating or sending 2FA token: {ex.Message}"
                    };
                }
            }

            var token = await _factorAuthentication.CreateToken(user);

            if (string.IsNullOrEmpty(token))
            {
                return new ResponseData<LoginResponse>
                {
                    Success = false,
                    Message = "Failed to generate authentication token."
                };
            }
            

            return new ResponseData<LoginResponse>
            {
                Success = true,
                Message = "Login successful.",
                Data = new LoginResponse
                {
                    Token = token,
                    UserId = user.Id,
                    UserName = user.UserName,
                    Is2FAEnabled = false
                }
            };
        }

        public async Task<bool> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            //var user = await _userManager.FindByEmailAsync(email);
            //if(user == null)
            //{
            //    return false;
            //}
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return false;
            }
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, AppUser user)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<ResponseData<RegisterResponse>> Register(RegisterRequest registerRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return new ResponseData<RegisterResponse>
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            // Create a new user instance.
            // If you have a static factory method, you can use it (e.g., AppUser.Create(...)),
            // otherwise create a new AppUser directly.
            var user = new AppUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.FirstName,
                Lastname = registerRequest.LastName
            };

            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, registerRequest.Password);
            user.PasswordHash = hashedPassword;
            // Create the user using the password provided in the registration request.
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                // Combine errors into a single message
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return new ResponseData<RegisterResponse>
                {
                    Success = false,
                    Message = errors
                };
            }

            // Optionally, assign a default role to the user (e.g., "User")
            var roleAssigned = await AssignRole(user.Email, "User");
            if (!roleAssigned)
            {
                return new ResponseData<RegisterResponse>
                {
                    Success = false,
                    Message = "Failed to assign role to the user."
                };
            }

            // Create a response object for the registered user
            var registerResponse = new RegisterResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return new ResponseData<RegisterResponse>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = registerResponse
            };
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Refresh token is missing.");
            }

            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                throw new Exception("Unable to retrieve user for refresh token");
            }

            if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                throw new Exception("Refresh token is expired.");
            }

            var token = await _factorAuthentication.CreateToken(user);
            var refreshTokenValue = _factorAuthentication.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await _userManager.UpdateAsync(user);

            _factorAuthentication.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", token);
            _factorAuthentication.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }
    }
}
