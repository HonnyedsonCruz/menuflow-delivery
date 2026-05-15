const checkoutForm = document.getElementById("checkout-form");
const checkoutItems = document.getElementById("checkout-items");
const checkoutSubtotal = document.getElementById("checkout-subtotal");
const checkoutDeliveryFee = document.getElementById("checkout-delivery-fee");
const checkoutTotal = document.getElementById("checkout-total");
const checkoutError = document.getElementById("checkout-error");
const finishOrderButton = document.getElementById("finish-order-button");
const toastElement = document.getElementById("toast");

const CHECKOUT_DELIVERY_FEE = 5;

document.addEventListener("DOMContentLoaded", () => {
  const cart = getCart();

  if (cart.length === 0) {
    window.location.href = "./carrinho.html";
    return;
  }

  renderCheckoutSummary();

  checkoutForm.addEventListener("submit", handleCheckoutSubmit);
});

function renderCheckoutSummary() {
  const cart = getCart();

  checkoutItems.innerHTML = "";

  cart.forEach(item => {
    const itemTotal = item.preco * item.quantidade;

    const div = document.createElement("div");
    div.className = "checkout-item";

    div.innerHTML = `
      <div>
        <strong>${item.nome}</strong>
        <span>${item.quantidade}x</span>
      </div>
      <p>${formatCurrency(itemTotal)}</p>
    `;

    checkoutItems.appendChild(div);
  });

  const subtotal = getCartTotal();
  const total = subtotal + CHECKOUT_DELIVERY_FEE;

  checkoutSubtotal.textContent = formatCurrency(subtotal);
  checkoutDeliveryFee.textContent = formatCurrency(CHECKOUT_DELIVERY_FEE);
  checkoutTotal.textContent = formatCurrency(total);
}

async function handleCheckoutSubmit(event) {
  event.preventDefault();

  hideError();

  const cart = getCart();

  if (cart.length === 0) {
    showError("Seu carrinho está vazio.");
    return;
  }

  const pedido = {
    nomeCliente: document.getElementById("nomeCliente").value.trim(),
    telefoneCliente: document.getElementById("telefoneCliente").value.trim(),
    enderecoEntrega: document.getElementById("enderecoEntrega").value.trim(),
    observacao: document.getElementById("observacao").value.trim(),
    formaPagamento: Number(document.querySelector('input[name="formaPagamento"]:checked').value),
    itens: cart.map(item => ({
      produtoId: item.id,
      quantidade: item.quantidade
    }))
  };

  try {
    setLoading(true);

    const response = await apiPost("/pedidos", pedido);

    if (!response.sucesso) {
      showError(response.mensagem || "Não foi possível criar o pedido.");
      return;
    }

    const pedidoCriado = response.dados;

    clearCart();

    showToast("Pedido criado com sucesso!");
    setTimeout(() => {
  const telefone = pedido.telefoneCliente;
  window.location.href = `./pedido.html?telefone=${encodeURIComponent(telefone)}`;
}, 800);
  } catch (error) {
    showError("Erro ao finalizar pedido. Verifique se a API está rodando.");
  } finally {
    setLoading(false);
  }
}

function setLoading(isLoading) {
  finishOrderButton.disabled = isLoading;
  finishOrderButton.textContent = isLoading ? "Enviando pedido..." : "Confirmar pedido";
}

function showError(message) {
  checkoutError.textContent = message;
  checkoutError.classList.remove("hidden");
}

function hideError() {
  checkoutError.textContent = "";
  checkoutError.classList.add("hidden");
}

function showToast(message) {
  toastElement.textContent = message;
  toastElement.classList.remove("hidden");

  setTimeout(() => {
    toastElement.classList.add("hidden");
  }, 2200);
}

function formatCurrency(value) {
  return value.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}