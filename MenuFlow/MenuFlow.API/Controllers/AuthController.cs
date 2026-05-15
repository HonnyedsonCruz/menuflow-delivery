using MenuFlow.API.DTOs;
using MenuFlow.API.Responses;
using MenuFlow.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize]
    [HttpPost("registrar")]
    public async Task<ActionResult<ApiResponse<object>>> Registrar(RegistrarUsuarioDTO usuarioDTO)
    {
        var resultado = await _authService.RegistrarAsync(usuarioDTO);

        return Created("", ApiResponse<object>.Ok(
            resultado,
            "Usuário administrador criado com sucesso."
        ));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login(LoginDTO loginDTO)
    {
        var resultado = await _authService.LoginAsync(loginDTO);

        if (resultado is null)
            return Unauthorized(ApiResponse<AuthResponseDTO>.Falha("E-mail ou senha inválidos."));

        return Ok(ApiResponse<AuthResponseDTO>.Ok(
            resultado,
            "Login realizado com sucesso."
        ));
    }
    [HttpPost("registrar-cliente")]
public async Task<ActionResult<ApiResponse<object>>> RegistrarCliente(RegistrarClienteDTO clienteDTO)
{
    var resultado = await _authService.RegistrarClienteAsync(clienteDTO);

    return Created("", ApiResponse<object>.Ok(
        resultado,
        "Cliente cadastrado com sucesso."
    ));
}

[HttpPost("login-cliente")]
public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> LoginCliente(LoginClienteDTO loginDTO)
{
    var resultado = await _authService.LoginClienteAsync(loginDTO);

    if (resultado is null)
        return Unauthorized(ApiResponse<AuthResponseDTO>.Falha("Telefone ou senha inválidos."));

    return Ok(ApiResponse<AuthResponseDTO>.Ok(
        resultado,
        "Login realizado com sucesso."
    ));
}
}