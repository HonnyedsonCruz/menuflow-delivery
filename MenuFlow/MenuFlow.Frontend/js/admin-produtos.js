const productsLoading = document.getElementById("products-loading");
const adminProductsGrid = document.getElementById("admin-products-grid");
const productsEmpty = document.getElementById("products-empty");
const productsError = document.getElementById("products-error");

const openProductFormButton = document.getElementById("open-product-form-button");
const closeProductFormButton = document.getElementById("close-product-form-button");
const productFormCard = document.getElementById("product-form-card");
const productForm = document.getElementById("product-form");
const productFormTitle = document.getElementById("product-form-title");
const productFormError = document.getElementById("product-form-error");
const saveProductButton = document.getElementById("save-product-button");

const produtoIdInput = document.getElementById("produtoId");
const nomeInput = document.getElementById("nome");
const precoInput = document.getElementById("preco");
const descricaoInput = document.getElementById("descricao");
const imagemUrlInput = document.getElementById("imagemUrl");
const categoriaSelect = document.getElementById("categoriaId");
const disponivelSelect = document.getElementById("disponivel");
const imagePreview = document.getElementById("image-preview");

const productSearch = document.getElementById("product-search");
const logoutButtonProducts = document.getElementById("logout-button");
const toastProducts = document.getElementById("toast");

const adminUserNameProducts = document.getElementById("admin-user-name");
const adminUserEmailProducts = document.getElementById("admin-user-email");

let allProductsAdmin = [];
let allCategoriesAdmin = [];

document.addEventListener("DOMContentLoaded", async () => {
  requireAdminAuth();

  const user = getAdminUser();

  if (user) {
    adminUserNameProducts.textContent = user.nome;
    adminUserEmailProducts.textContent = user.email;
  }

  logoutButtonProducts.addEventListener("click", logoutAdmin);

  openProductFormButton.addEventListener("click", () => {
    openCreateProductForm();
  });

  closeProductFormButton.addEventListener("click", () => {
    closeProductForm();
  });

  productForm.addEventListener("submit", handleProductSubmit);

  productSearch.addEventListener("input", renderProductsAdmin);

  imagemUrlInput.addEventListener("input", updateImagePreview);

  await loadInitialData();
});

async function loadInitialData() {
  await Promise.all([
    loadCategoriesAdmin(),
    loadProductsAdmin()
  ]);
}

async function loadCategoriesAdmin() {
  try {
    const response = await apiGet("/categorias");

    if (!response.sucesso) {
      throw new Error(response.mensagem);
    }

    allCategoriesAdmin = response.dados || [];

    renderCategoryOptions();
  } catch (error) {
    showToast("Erro ao carregar categorias.");
  }
}

function renderCategoryOptions() {
  categoriaSelect.innerHTML = `<option value="">Selecione uma categoria</option>`;

  allCategoriesAdmin.forEach(category => {
    const option = document.createElement("option");
    option.value = category.id;
    option.textContent = category.nome;

    categoriaSelect.appendChild(option);
  });
}

async function loadProductsAdmin() {
  try {
    productsLoading.classList.remove("hidden");
    adminProductsGrid.classList.add("hidden");
    productsEmpty.classList.add("hidden");
    productsError.classList.add("hidden");

    const response = await apiGet("/produtos");

    if (!response.sucesso) {
      throw new Error(response.mensagem || "Erro ao carregar produtos.");
    }

    allProductsAdmin = response.dados || [];

    renderProductsAdmin();
  } catch (error) {
    productsError.textContent = "Erro ao carregar produtos. Verifique se a API está rodando.";
    productsError.classList.remove("hidden");
  } finally {
    productsLoading.classList.add("hidden");
  }
}

function renderProductsAdmin() {
  const searchTerm = productSearch.value.toLowerCase().trim();

  const filteredProducts = allProductsAdmin.filter(product => {
    return product.nome.toLowerCase().includes(searchTerm) ||
      product.descricao.toLowerCase().includes(searchTerm) ||
      product.categoriaNome.toLowerCase().includes(searchTerm);
  });

  adminProductsGrid.innerHTML = "";

  if (filteredProducts.length === 0) {
    adminProductsGrid.classList.add("hidden");
    productsEmpty.classList.remove("hidden");
    return;
  }

  productsEmpty.classList.add("hidden");
  adminProductsGrid.classList.remove("hidden");

  filteredProducts.forEach(product => {
    const card = createAdminProductCard(product);
    adminProductsGrid.appendChild(card);
  });
}

function createAdminProductCard(product) {
  const article = document.createElement("article");
  article.className = "admin-product-card";

  const imageContent = product.imagemUrl
    ? `<img src="${product.imagemUrl}" alt="${product.nome}" />`
    : `<span>🍽️</span>`;

  article.innerHTML = `
    <div class="admin-product-image">
      ${imageContent}
    </div>

    <div class="admin-product-body">
      <div class="admin-product-top">
        <span class="section-kicker">${product.categoriaNome}</span>
        <span class="product-status ${product.disponivel ? "available" : "unavailable"}">
          ${product.disponivel ? "Disponível" : "Indisponível"}
        </span>
      </div>

      <h3>${product.nome}</h3>
      <p>${product.descricao}</p>

      <strong class="admin-product-price">
        ${formatCurrency(product.preco)}
      </strong>

      <div class="admin-product-actions">
        <button class="admin-small-button edit-button">
          Editar
        </button>

        <button class="admin-small-button toggle-button">
          ${product.disponivel ? "Desativar" : "Ativar"}
        </button>

        <button class="admin-small-button danger delete-button">
          Remover
        </button>
      </div>
    </div>
  `;

  article.querySelector(".edit-button").addEventListener("click", () => {
    openEditProductForm(product);
  });

  article.querySelector(".toggle-button").addEventListener("click", async () => {
    await toggleProductAvailability(product.id);
  });

  article.querySelector(".delete-button").addEventListener("click", async () => {
    await deleteProduct(product.id, product.nome);
  });

  return article;
}

function openCreateProductForm() {
  productFormTitle.textContent = "Novo produto";
  saveProductButton.textContent = "Salvar produto";

  produtoIdInput.value = "";
  productForm.reset();
  disponivelSelect.value = "true";
  imagePreview.innerHTML = "🍽️";
  hideProductFormError();

  productFormCard.classList.remove("hidden");
  productFormCard.scrollIntoView({ behavior: "smooth", block: "start" });
}

function openEditProductForm(product) {
  productFormTitle.textContent = `Editar produto #${product.id}`;
  saveProductButton.textContent = "Atualizar produto";

  produtoIdInput.value = product.id;
  nomeInput.value = product.nome;
  precoInput.value = product.preco;
  descricaoInput.value = product.descricao;
  imagemUrlInput.value = product.imagemUrl || "";
  categoriaSelect.value = product.categoriaId;
  disponivelSelect.value = String(product.disponivel);

  updateImagePreview();
  hideProductFormError();

  productFormCard.classList.remove("hidden");
  productFormCard.scrollIntoView({ behavior: "smooth", block: "start" });
}

function closeProductForm() {
  productFormCard.classList.add("hidden");
  productForm.reset();
  produtoIdInput.value = "";
  hideProductFormError();
}

async function handleProductSubmit(event) {
  event.preventDefault();

  hideProductFormError();

  const token = getAdminToken();

  const productData = {
    nome: nomeInput.value.trim(),
    descricao: descricaoInput.value.trim(),
    preco: Number(precoInput.value),
    imagemUrl: imagemUrlInput.value.trim(),
    disponivel: disponivelSelect.value === "true",
    categoriaId: Number(categoriaSelect.value)
  };

  try {
    setProductFormLoading(true);

    const productId = produtoIdInput.value;

    let response;

    if (productId) {
      response = await apiPut(`/produtos/${productId}`, productData, token);
    } else {
      response = await apiPost("/produtos", productData, token);
    }

    if (!response.sucesso) {
      showProductFormError(response.mensagem || "Erro ao salvar produto.");
      return;
    }

    showToast(productId ? "Produto atualizado com sucesso!" : "Produto criado com sucesso!");

    closeProductForm();
    await loadProductsAdmin();
  } catch (error) {
    showProductFormError("Erro ao salvar produto. Verifique os dados e tente novamente.");
  } finally {
    setProductFormLoading(false);
  }
}

async function toggleProductAvailability(productId) {
  try {
    const token = getAdminToken();

    const response = await apiPatch(`/produtos/${productId}/disponibilidade`, token);

    if (!response.sucesso) {
      showToast(response.mensagem || "Erro ao alterar disponibilidade.");
      return;
    }

    showToast("Disponibilidade alterada com sucesso!");
    await loadProductsAdmin();
  } catch (error) {
    showToast("Erro ao alterar disponibilidade.");
  }
}

async function deleteProduct(productId, productName) {
  const confirmDelete = confirm(`Tem certeza que deseja remover o produto "${productName}"?`);

  if (!confirmDelete) return;

  try {
    const token = getAdminToken();

    const response = await apiDelete(`/produtos/${productId}`, token);

    if (!response.sucesso) {
      showToast(response.mensagem || "Erro ao remover produto.");
      return;
    }

    showToast("Produto removido com sucesso!");
    await loadProductsAdmin();
  } catch (error) {
    showToast("Erro ao remover produto.");
  }
}

function updateImagePreview() {
  const imageUrl = imagemUrlInput.value.trim();

  if (!imageUrl) {
    imagePreview.innerHTML = "🍽️";
    return;
  }

  imagePreview.innerHTML = `<img src="${imageUrl}" alt="Prévia da imagem" />`;
}

function setProductFormLoading(isLoading) {
  saveProductButton.disabled = isLoading;
  saveProductButton.textContent = isLoading ? "Salvando..." : produtoIdInput.value ? "Atualizar produto" : "Salvar produto";
}

function showProductFormError(message) {
  productFormError.textContent = message;
  productFormError.classList.remove("hidden");
}

function hideProductFormError() {
  productFormError.textContent = "";
  productFormError.classList.add("hidden");
}

function showToast(message) {
  toastProducts.textContent = message;
  toastProducts.classList.remove("hidden");

  setTimeout(() => {
    toastProducts.classList.add("hidden");
  }, 2200);
}

function formatCurrency(value) {
  return value.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}