let allProducts = [];
let selectedCategory = "all";

const productsGrid = document.getElementById("products-grid");
const loading = document.getElementById("loading");
const emptyState = document.getElementById("empty-state");
const searchInput = document.getElementById("search-input");
const categoryList = document.getElementById("category-list");
const toast = document.getElementById("toast");

document.addEventListener("DOMContentLoaded", async () => {
  await loadProducts();

  searchInput.addEventListener("input", renderProducts);
});

async function loadProducts() {
  try {
    loading.classList.remove("hidden");

    const response = await apiGet("/produtos");

    allProducts = response.dados || [];

    createCategoryButtons();
    renderProducts();
  } catch (error) {
    productsGrid.innerHTML = `
      <div class="empty-state">
        <h3>Erro ao carregar produtos</h3>
        <p>Verifique se a API está rodando em http://localhost:5094.</p>
      </div>
    `;
  } finally {
    loading.classList.add("hidden");
  }
}

function createCategoryButtons() {
  const categories = [...new Set(allProducts.map(product => product.categoriaNome))];

  categories.forEach(category => {
    const button = document.createElement("button");
    button.className = "category-button";
    button.textContent = category;
    button.dataset.category = category;

    button.addEventListener("click", () => {
      document.querySelectorAll(".category-button").forEach(btn => {
        btn.classList.remove("active");
      });

      button.classList.add("active");
      selectedCategory = category;
      renderProducts();
    });

    categoryList.appendChild(button);
  });

  const allButton = document.querySelector('[data-category="all"]');

  allButton.addEventListener("click", () => {
    document.querySelectorAll(".category-button").forEach(btn => {
      btn.classList.remove("active");
    });

    allButton.classList.add("active");
    selectedCategory = "all";
    renderProducts();
  });
}

function renderProducts() {
  const searchTerm = searchInput.value.toLowerCase().trim();

  const filteredProducts = allProducts.filter(product => {
    const matchesSearch = product.nome.toLowerCase().includes(searchTerm) ||
      product.descricao.toLowerCase().includes(searchTerm);

    const matchesCategory = selectedCategory === "all" ||
      product.categoriaNome === selectedCategory;

    return matchesSearch && matchesCategory;
  });

  productsGrid.innerHTML = "";

  if (filteredProducts.length === 0) {
    emptyState.classList.remove("hidden");
    return;
  }

  emptyState.classList.add("hidden");

  filteredProducts.forEach(product => {
    const card = createProductCard(product);
    productsGrid.appendChild(card);
  });
}

function createProductCard(product) {
  const card = document.createElement("article");
  card.className = "product-card";

  const formattedPrice = product.preco.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });

  const imageContent = product.imagemUrl
    ? `<img src="${product.imagemUrl}" alt="${product.nome}" />`
    : `<div class="placeholder">🍽️</div>`;

  card.innerHTML = `
    <div class="product-image">
      ${imageContent}
    </div>

    <div class="product-info">
      <div class="product-category">${product.categoriaNome}</div>
      <h3>${product.nome}</h3>
      <p>${product.descricao}</p>

      <div class="product-footer">
        <span class="price">${formattedPrice}</span>
        <button class="add-button ${!product.disponivel ? "disabled" : ""}">
          ${product.disponivel ? "Adicionar" : "Indisponível"}
        </button>
      </div>
    </div>
  `;

  const button = card.querySelector(".add-button");

  if (product.disponivel) {
    button.addEventListener("click", () => {
      addToCart(product);
      animateButton(button);
      showToast(`${product.nome} adicionado ao carrinho!`);
    });
  }

  return card;
}

function animateButton(button) {
  button.textContent = "Adicionado!";
  button.style.background = "#16a34a";

  setTimeout(() => {
    button.textContent = "Adicionar";
    button.style.background = "";
  }, 900);
}

function showToast(message) {
  toast.textContent = message;
  toast.classList.remove("hidden");

  setTimeout(() => {
    toast.classList.add("hidden");
  }, 2200);
}