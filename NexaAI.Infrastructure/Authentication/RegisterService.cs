using Microsoft.AspNetCore.Identity;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Infrastructure.Identity;

namespace NexaAI.Infrastructure.Authentication;

public class RegisterService : IRegisterService
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResult> RegisterAsync(string name, string surname, string email, string password)
    {
        var user = new AppUser
        {
            Name = name,
            Surname = surname,
            Email = email,
            UserName = email
        };
        
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new RegisterResult
            {
                Succeeded = false,
                Message = string.Join(", ", result.Errors.Select(x => x.Description))
            };
        }

        return new RegisterResult
        {
            Succeeded = true,
            Message = "Kullanıcı başarıyla oluşturuldu."
        };
    }
}