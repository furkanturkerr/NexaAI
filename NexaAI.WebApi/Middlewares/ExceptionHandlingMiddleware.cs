using FluentValidation;

namespace NexaAI.WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = 400;

            var errors = exception.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToArray());

            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Validasyon hatası oluştu.",
                Errors = errors
            });
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Beklenmeyen bir hata oluştu."
            });
        }
    }
}