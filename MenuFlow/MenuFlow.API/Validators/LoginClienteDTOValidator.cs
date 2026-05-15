using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class LoginClienteDTOValidator : AbstractValidator<LoginClienteDTO>
{
    public LoginClienteDTOValidator()
    {
        RuleFor(c => c.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório.");

        RuleFor(c => c.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}