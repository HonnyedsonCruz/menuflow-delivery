using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class ProdutoDTOValidator : AbstractValidator<ProdutoDTO>
{
    public ProdutoDTOValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(p => p.Descricao)
            .NotEmpty().WithMessage("A descrição do produto é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(p => p.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

        RuleFor(p => p.ImagemUrl)
            .MaximumLength(500).WithMessage("A URL da imagem deve ter no máximo 500 caracteres.");

        RuleFor(p => p.CategoriaId)
            .GreaterThan(0).WithMessage("A categoria é obrigatória.");
    }
}