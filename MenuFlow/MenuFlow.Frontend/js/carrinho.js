const CART_KEY = "menuflow_cart";

function getCart() {
  const cart = localStorage.getItem(CART_KEY);
  return cart ? JSON.parse(cart) : [];
}

function saveCart(cart) {
  localStorage.setItem(CART_KEY, JSON.stringify(cart));
  updateCartCount();
}

function addToCart(product) {
  const cart = getCart();

  const existingItem = cart.find(item => item.id === product.id);

  if (existingItem) {
    existingItem.quantidade += 1;
  } else {
    cart.push({
      id: product.id,
      nome: product.nome,
      preco: product.preco,
      imagemUrl: product.imagemUrl,
      quantidade: 1
    });
  }

  saveCart(cart);
}

function removeFromCart(productId) {
  const cart = getCart().filter(item => item.id !== productId);
  saveCart(cart);
}

function updateCartItemQuantity(productId, quantity) {
  const cart = getCart();

  const item = cart.find(item => item.id === productId);

  if (!item) return;

  item.quantidade = quantity;

  if (item.quantidade <= 0) {
    removeFromCart(productId);
    return;
  }

  saveCart(cart);
}

function clearCart() {
  localStorage.removeItem(CART_KEY);
  updateCartCount();
}

function getCartTotal() {
  return getCart().reduce((total, item) => {
    return total + item.preco * item.quantidade;
  }, 0);
}

function updateCartCount() {
  const cartCount = document.getElementById("cart-count");

  if (!cartCount) return;

  const totalItems = getCart().reduce((total, item) => {
    return total + item.quantidade;
  }, 0);

  cartCount.textContent = totalItems;
}

document.addEventListener("DOMContentLoaded", updateCartCount);