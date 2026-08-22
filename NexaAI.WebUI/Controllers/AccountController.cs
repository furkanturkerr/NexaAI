using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.AccountDtos;

namespace NexaAI.WebUI.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // GET
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Default");
        }
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Default");
        }
        
        var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync("http://localhost:5015/api/Auth/login", dto);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        if (!response.IsSuccessStatusCode || result == null || !result.Succeeded || string.IsNullOrEmpty(result.Token))
        {
            ModelState.AddModelError("", result?.Message ?? "Giriş işlemi başarısız.");
            return View(dto);
        }
        
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        
        var claims = jwtToken.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
        
        claims.Add(new Claim("access_token", result.Token));
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                ExpiresUtc = new DateTimeOffset(jwtToken.ValidTo),
                AllowRefresh = false
            });
        
        return RedirectToAction("Index", "Default");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Default");
        }
        
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Default");
        }
        
        var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync("http://localhost:5015/api/Auth/register", dto);
        
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (!response.IsSuccessStatusCode || result == null || !result.Succeeded)
        {
            ModelState.AddModelError("", result?.Message ?? "Kayıt işlemi başarısız.");
            return View(dto);
        }

        TempData["RegisterSuccess"] = "Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.";

        return RedirectToAction(nameof(Login));
    }
    
    
}