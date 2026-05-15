const cartItemsContainer = document.getElementById("cart-items");
const cartEmpty = document.getElementById("cart-empty");
const subtotalElement = document.getElementById("subtotal");
const deliveryFeeElement = document.getElementById("delivery-fee");
const totalElement = document.getElementById("total");
const checkoutButton = document.getElementById("checkout-button");
const clearCartButton = document.getElementById("clear-cart-button");

const DELIVERY_FEE = 5;

document.addEventListener("DOMContentLoaded", () => {
  renderCartPage();

  clearCartButton.addEventListener("click", () => {
    clearCart();
    renderCartPage();
  });
});

function renderCartPage() {
  const cart = getCart();

  cartItemsContainer.innerHTML = "";

  if (cart.length === 0) {
    cartEmpty.classList.remove("hidden");
    checkoutButton.classList.add("disabled-link");
    clearCartButton.classList.add("hidden");

    updateSummary(0);
    return;
  }

  cartEmpty.classList.add("hidden");
  checkoutButton.classList.remove("disabled-link");
  clearCartButton.classList.remove("hidden");

  cart.forEach(item => {
    const cartItem = createCartItem(item);
    cartItemsContainer.appendChild(cartItem);
  });

  updateSummary(getCartTotal());
}

function createCartItem(item) {
  const div = document.createElement("div");
  div.className = "cart-item";

  const formattedPrice = item.preco.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });

  const itemTotal = (item.preco * item.quantidade).toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });

  div.innerHTML = `
    <div class="cart-item-image">
      ${
        item.imagemUrl
          ? `<img src="${item.imagemUrl}" alt="${item.nome}" />`
          : `<span>🍽️</span>`
      }
    </div>

    <div class="cart-item-info">
      <h3>${item.nome}</h3>
      <p>${formattedPrice} cada</p>

      <div class="quantity-control">
        <button class="quantity-btn decrease">−</button>
        <span>${item.quantidade}</span>
        <button class="quantity-btn increase">+</button>
      </div>
    </div>

    <div class="cart-item-actions">
      <strong>${itemTotal}</strong>
      <button class="remove-button">Remover</button>
    </div>
  `;

  div.querySelector(".decrease").addEventListener("click", () => {
    updateCartItemQuantity(item.id, item.quantidade - 1);
    renderCartPage();
  });

  div.querySelector(".increase").addEventListener("click", () => {
    updateCartItemQuantity(item.id, item.quantidade + 1);
    renderCartPage();
  });

  div.querySelector(".remove-button").addEventListener("click", () => {
    removeFromCart(item.id);
    renderCartPage();
  });

  return div;
}

function updateSummary(subtotal) {
  const total = subtotal > 0 ? subtotal + DELIVERY_FEE : 0;

  subtotalElement.textContent = subtotal.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });

  deliveryFeeElement.textContent = subtotal > 0
    ? DELIVERY_FEE.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
      })
    : "R$ 0,00";

  totalElement.textContent = total.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}