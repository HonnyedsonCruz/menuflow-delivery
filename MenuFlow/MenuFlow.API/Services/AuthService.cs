using MenuFlow.API.Data;
using MenuFlow.API.DTOs;
using MenuFlow.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MenuFlow.API.Exceptions;

namespace MenuFlow.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<object> RegistrarAsync(RegistrarUsuarioDTO usuarioDTO)
    {
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == usuarioDTO.Email);

        if (emailExiste)
            throw new RegraDeNegocioException("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = usuarioDTO.Nome,
            Email = usuarioDTO.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha),
            Role = "Admin",
            CriadoEm = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new
        {
            mensagem = "Usuário administrador criado com sucesso.",
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Role
        };
    }
    public async Task<object> RegistrarClienteAsync(RegistrarClienteDTO clienteDTO)
    {
        var telefoneExiste = await _context.Usuarios
            .AnyAsync(u => u.Telefone == clienteDTO.Telefone);

        if (telefoneExiste)
            throw new RegraDeNegocioException("Telefone já cadastrado.");

        var usuario = new Usuario
        {
            Nome = clienteDTO.Nome,
            Telefone = clienteDTO.Telefone,
            Endereco = clienteDTO.Endereco,
            Email = string.Empty,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(clienteDTO.Senha),
            Role = "Cliente",
            CriadoEm = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new
        {
            mensagem = "Cliente cadastrado com sucesso.",
            usuario.Id,
            usuario.Nome,
            usuario.Telefone,
            usuario.Endereco,
            usuario.Role
        };
    }
    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

        if (usuario is null)
            return null;

        var senhaValida = BCrypt.Net.BCrypt.Verify(loginDTO.Senha, usuario.SenhaHash);

        if (!senhaValida)
            return null;

        var token = GerarToken(usuario);

        return new AuthResponseDTO
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Role = usuario.Role
        };
    }
    public async Task<AuthResponseDTO?> LoginClienteAsync(LoginClienteDTO loginDTO)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Telefone == loginDTO.Telefone &&
                u.Role == "Cliente");

        if (usuario is null)
            return null;

        var senhaValida = BCrypt.Net.BCrypt.Verify(loginDTO.Senha, usuario.SenhaHash);

        if (!senhaValida)
            return null;

        var token = GerarToken(usuario);

        return new AuthResponseDTO
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Role = usuario.Role
        };
    }
    private string GerarToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}