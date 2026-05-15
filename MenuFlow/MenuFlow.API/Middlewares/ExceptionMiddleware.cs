using MenuFlow.API.Exceptions;
using MenuFlow.API.Responses;
using System.Net;
using System.Text.Json;

namespace MenuFlow.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RegraDeNegocioException ex)
        {
            await TratarErroAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado na aplicação.");

            var mensagem = _environment.IsDevelopment()
                ? $"Ocorreu um erro interno no servidor: {ex.Message}"
                : "Ocorreu um erro interno no servidor.";

            await TratarErroAsync(context, HttpStatusCode.InternalServerError, mensagem);
        }
    }

    private static async Task TratarErroAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string mensagem)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Falha(mensagem);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}