using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class ItemPedidoDTOValidator : AbstractValidator<ItemPedidoDTO>
{
    public ItemPedidoDTOValidator()
    {
        RuleFor(i => i.ProdutoId)
            .GreaterThan(0).WithMessage("O produto é obrigatório.");

        RuleFor(i => i.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
            .LessThanOrEqualTo(50).WithMessage("A quantidade máxima por item é 50.");
    }
}