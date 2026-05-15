using MenuFlow.API.DTOs;

namespace MenuFlow.API.Services;

public interface IAuthService
{
    Task<object> RegistrarAsync(RegistrarUsuarioDTO usuarioDTO);
    Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO);

    Task<object> RegistrarClienteAsync(RegistrarClienteDTO clienteDTO);
    Task<AuthResponseDTO?> LoginClienteAsync(LoginClienteDTO loginDTO);
}