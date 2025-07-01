using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var identityUser = new ApplicationUser
            {
                Name = registerRequestDto.Name,
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("user was registered! Please Login.");
                    }
                }
            }

            return BadRequest("Something went wrong!!");


        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var identityUser = await userManager.FindByEmailAsync(loginRequestDto.Email);
            if (identityUser != null)
            {
                var isPasswordValid = await userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);
                if (isPasswordValid)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);
                    if ( roles != null && roles.Any() )
                    {
                        var jwtToken = tokenRepository.CreateJwtToken(identityUser, roles.ToList());
                        var response = new LoginResponseDto
                        {

                            JwtToken = jwtToken,
                            UserId = identityUser.Id,
                            Role = roles.First() 

                        };
                        return Ok(response);
                    }
                    return Ok(new { Message = "Login successful!", UserId = identityUser.Id });
                }
            }
            return BadRequest("Invalid email or password.");
        }

        [HttpPost("changePassword")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            // Convert the string ID to Guid explicitly
            if (!Guid.TryParse(changePasswordDto.Id.ToString(), out Guid userId))
            {
                return BadRequest("Invalid user ID format.");
            }

            var identityUser = await userManager.FindByIdAsync(userId.ToString());
            if (identityUser == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ChangePasswordAsync(identityUser, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            return BadRequest("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        [HttpGet("getUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }

            var identityUser = await userManager.FindByIdAsync(userGuid.ToString());
            if (identityUser == null)
            {
                return NotFound("User not found.");
            }

            var userProfileDto = new UserProfileDto
            {
                Id = userGuid,
                Name = identityUser.Name,
                Email = identityUser.Email,
                Address = identityUser.Address
            };

            return Ok(userProfileDto);
        }

        [HttpPut("updateUserProfile")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileDto updateUserProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            // Convert the string ID to Guid explicitly
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }
            var identityUser = await userManager.FindByIdAsync(userGuid.ToString());
            if (identityUser == null)
            {
                return NotFound("User not found.");
            }
            identityUser.Name = updateUserProfileDto.Name;
            identityUser.Email = updateUserProfileDto.Email;
            identityUser.Address = updateUserProfileDto.Address;
            var result = await userManager.UpdateAsync(identityUser);
            if (result.Succeeded)
            {
                return Ok("User profile updated successfully.");
            }
            return BadRequest("Failed to update user profile: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
