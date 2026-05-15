using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class CategoriaDTOValidator : AbstractValidator<CategoriaDTO>
{
    public CategoriaDTOValidator()
    {
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(c => c.Descricao)
            .MaximumLength(300).WithMessage("A descrição deve ter no máximo 300 caracteres.");
    }
}