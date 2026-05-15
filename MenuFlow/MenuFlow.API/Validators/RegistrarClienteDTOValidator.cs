using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class RegistrarClienteDTOValidator : AbstractValidator<RegistrarClienteDTO>
{
    public RegistrarClienteDTOValidator()
    {
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(c => c.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .MinimumLength(10).WithMessage("O telefone deve ter pelo menos 10 caracteres.")
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(c => c.Endereco)
            .NotEmpty().WithMessage("O endereço é obrigatório.")
            .MinimumLength(5).WithMessage("O endereço deve ter pelo menos 5 caracteres.")
            .MaximumLength(200).WithMessage("O endereço deve ter no máximo 200 caracteres.");

        RuleFor(c => c.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
    }
}