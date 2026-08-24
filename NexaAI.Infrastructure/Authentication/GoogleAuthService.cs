using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Infrastructure.Identity;

namespace NexaAI.Infrastructure.Authentication;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<GoogleUserResult> LoginAsync(string idToken)
    {
        try
        {
            var clientId =
                _configuration["Google:ClientId"];

            var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[]
                    {
                        clientId
                    }
                };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            if (!payload.EmailVerified)
            {
                return new GoogleUserResult
                {
                    Succeeded = false,
                    Message = "Google e-posta adresi doğrulanmamış."
                };
            }

            var user =
                await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new AppUser
                {
                    Name = payload.GivenName ?? string.Empty,
                    Surname = payload.FamilyName ?? string.Empty,

                    Email = payload.Email,
                    UserName = payload.Email,

                    EmailConfirmed = true
                };

                var createResult =
                    await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    return new GoogleUserResult
                    {
                        Succeeded = false,

                        Message = string.Join(
                            ", ",
                            createResult.Errors
                                .Select(x => x.Description))
                    };
                }
            }

            return new GoogleUserResult
            {
                Succeeded = true,

                Message = "Google hesabı doğrulandı.",

                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname
            };
        }
        catch (InvalidJwtException)
        {
            return new GoogleUserResult
            {
                Succeeded = false,
                Message = "Google oturumu doğrulanamadı."
            };
        }
    }
}