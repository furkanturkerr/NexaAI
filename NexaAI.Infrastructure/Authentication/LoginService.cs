using Microsoft.AspNetCore.Identity;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Infrastructure.Identity;

namespace NexaAI.Infrastructure.Authentication;

public class LoginService : ILoginService
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<AppUser> _userManager;

    public LoginService(IJwtService jwtService, UserManager<AppUser> userManager)
    {
        _jwtService = jwtService;
        _userManager = userManager;
    }

    public async Task<LoginUserResult> LoginAsync(string userNameOrEmail, string password)
    {
        var user = await _userManager.FindByNameAsync(userNameOrEmail);

        if (user == null)
            user = await _userManager.FindByEmailAsync(userNameOrEmail);

        if (user == null)
        {
            return new LoginUserResult
            {
                Succeeded = false,
                Message = "Kullanıcı Bulunamadı."
            };
        }

        var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
        
        if (!passwordCheck)
        {
            return new LoginUserResult
            {
                Succeeded = false,
                Message = "Kullanıcı adı, e-posta veya şifre hatalı."
            };
        }
        
        return new LoginUserResult
        {
            Succeeded = true,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Message = "Giriş başarılı."
        };
    }
}