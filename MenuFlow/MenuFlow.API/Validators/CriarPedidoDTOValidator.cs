using FluentValidation;
using MenuFlow.API.DTOs;

namespace MenuFlow.API.Validators;

public class CriarPedidoDTOValidator : AbstractValidator<CriarPedidoDTO>
{
    public CriarPedidoDTOValidator()
    {
        RuleFor(p => p.NomeCliente)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do cliente deve ter pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("O nome do cliente deve ter no máximo 100 caracteres.");

        RuleFor(p => p.TelefoneCliente)
            .NotEmpty().WithMessage("O telefone do cliente é obrigatório.")
            .MinimumLength(10).WithMessage("O telefone deve ter pelo menos 10 caracteres.")
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(p => p.EnderecoEntrega)
            .NotEmpty().WithMessage("O endereço de entrega é obrigatório.")
            .MinimumLength(5).WithMessage("O endereço deve ter pelo menos 5 caracteres.")
            .MaximumLength(200).WithMessage("O endereço deve ter no máximo 200 caracteres.");

        RuleFor(p => p.FormaPagamento)
            .IsInEnum().WithMessage("Forma de pagamento inválida.");

        RuleFor(p => p.Itens)
            .NotEmpty().WithMessage("O pedido precisa ter pelo menos um item.");

        RuleForEach(p => p.Itens)
            .SetValidator(new ItemPedidoDTOValidator());
    }
}