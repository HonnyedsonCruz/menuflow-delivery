const categoriesLoading = document.getElementById("categories-loading");
const adminCategoriesGrid = document.getElementById("admin-categories-grid");
const categoriesEmpty = document.getElementById("categories-empty");
const categoriesError = document.getElementById("categories-error");

const openCategoryFormButton = document.getElementById("open-category-form-button");
const closeCategoryFormButton = document.getElementById("close-category-form-button");
const categoryFormCard = document.getElementById("category-form-card");
const categoryForm = document.getElementById("category-form");
const categoryFormTitle = document.getElementById("category-form-title");
const categoryFormError = document.getElementById("category-form-error");
const saveCategoryButton = document.getElementById("save-category-button");

const categoriaIdInput = document.getElementById("categoriaId");
const nomeCategoryInput = document.getElementById("nome");
const descricaoCategoryInput = document.getElementById("descricao");
const ativaCategorySelect = document.getElementById("ativa");

const categorySearch = document.getElementById("category-search");
const logoutButtonCategories = document.getElementById("logout-button");
const toastCategories = document.getElementById("toast");

const adminUserNameCategories = document.getElementById("admin-user-name");
const adminUserEmailCategories = document.getElementById("admin-user-email");

let allCategoriesPanel = [];

document.addEventListener("DOMContentLoaded", async () => {
  requireAdminAuth();

  const user = getAdminUser();

  if (user) {
    adminUserNameCategories.textContent = user.nome;
    adminUserEmailCategories.textContent = user.email;
  }

  logoutButtonCategories.addEventListener("click", logoutAdmin);

  openCategoryFormButton.addEventListener("click", openCreateCategoryForm);
  closeCategoryFormButton.addEventListener("click", closeCategoryForm);

  categoryForm.addEventListener("submit", handleCategorySubmit);
  categorySearch.addEventListener("input", renderCategoriesPanel);

  await loadCategoriesPanel();
});

async function loadCategoriesPanel() {
  try {
    categoriesLoading.classList.remove("hidden");
    adminCategoriesGrid.classList.add("hidden");
    categoriesEmpty.classList.add("hidden");
    categoriesError.classList.add("hidden");

    const response = await apiGet("/categorias");

    if (!response.sucesso) {
      throw new Error(response.mensagem || "Erro ao carregar categorias.");
    }

    allCategoriesPanel = response.dados || [];

    renderCategoriesPanel();
  } catch (error) {
    categoriesError.textContent = "Erro ao carregar categorias. Verifique se a API está rodando.";
    categoriesError.classList.remove("hidden");
  } finally {
    categoriesLoading.classList.add("hidden");
  }
}

function renderCategoriesPanel() {
  const searchTerm = categorySearch.value.toLowerCase().trim();

  const filteredCategories = allCategoriesPanel.filter(category => {
    return category.nome.toLowerCase().includes(searchTerm) ||
      category.descricao.toLowerCase().includes(searchTerm);
  });

  adminCategoriesGrid.innerHTML = "";

  if (filteredCategories.length === 0) {
    adminCategoriesGrid.classList.add("hidden");
    categoriesEmpty.classList.remove("hidden");
    return;
  }

  categoriesEmpty.classList.add("hidden");
  adminCategoriesGrid.classList.remove("hidden");

  filteredCategories.forEach(category => {
    const card = createCategoryCard(category);
    adminCategoriesGrid.appendChild(card);
  });
}

function createCategoryCard(category) {
  const article = document.createElement("article");
  article.className = "admin-category-card";

  article.innerHTML = `
    <div class="admin-category-icon">
      ${getCategoryEmoji(category.nome)}
    </div>

    <div class="admin-category-body">
      <div class="admin-product-top">
        <span class="section-kicker">Categoria #${category.id}</span>
        <span class="product-status ${category.ativa ? "available" : "unavailable"}">
          ${category.ativa ? "Ativa" : "Inativa"}
        </span>
      </div>

      <h3>${category.nome}</h3>
      <p>${category.descricao || "Sem descrição."}</p>

      <div class="admin-product-actions">
        <button class="admin-small-button edit-button">
          Editar
        </button>

        <button class="admin-small-button toggle-button">
          ${category.ativa ? "Desativar" : "Ativar"}
        </button>

        <button class="admin-small-button danger delete-button">
          Remover
        </button>
      </div>
    </div>
  `;

  article.querySelector(".edit-button").addEventListener("click", () => {
    openEditCategoryForm(category);
  });

  article.querySelector(".toggle-button").addEventListener("click", async () => {
    await toggleCategoryStatus(category);
  });

  article.querySelector(".delete-button").addEventListener("click", async () => {
    await deleteCategory(category.id, category.nome);
  });

  return article;
}

function openCreateCategoryForm() {
  categoryFormTitle.textContent = "Nova categoria";
  saveCategoryButton.textContent = "Salvar categoria";

  categoriaIdInput.value = "";
  categoryForm.reset();
  ativaCategorySelect.value = "true";
  hideCategoryFormError();

  categoryFormCard.classList.remove("hidden");
  categoryFormCard.scrollIntoView({ behavior: "smooth", block: "start" });
}

function openEditCategoryForm(category) {
  categoryFormTitle.textContent = `Editar categoria #${category.id}`;
  saveCategoryButton.textContent = "Atualizar categoria";

  categoriaIdInput.value = category.id;
  nomeCategoryInput.value = category.nome;
  descricaoCategoryInput.value = category.descricao || "";
  ativaCategorySelect.value = String(category.ativa);

  hideCategoryFormError();

  categoryFormCard.classList.remove("hidden");
  categoryFormCard.scrollIntoView({ behavior: "smooth", block: "start" });
}

function closeCategoryForm() {
  categoryFormCard.classList.add("hidden");
  categoryForm.reset();
  categoriaIdInput.value = "";
  hideCategoryFormError();
}

async function handleCategorySubmit(event) {
  event.preventDefault();

  hideCategoryFormError();

  const token = getAdminToken();

  const categoryData = {
    nome: nomeCategoryInput.value.trim(),
    descricao: descricaoCategoryInput.value.trim(),
    ativa: ativaCategorySelect.value === "true"
  };

  try {
    setCategoryFormLoading(true);

    const categoryId = categoriaIdInput.value;

    let response;

    if (categoryId) {
      response = await apiPut(`/categorias/${categoryId}`, categoryData, token);
    } else {
      response = await apiPost("/categorias", categoryData, token);
    }

    if (!response.sucesso) {
      showCategoryFormError(response.mensagem || "Erro ao salvar categoria.");
      return;
    }

    showToast(categoryId ? "Categoria atualizada com sucesso!" : "Categoria criada com sucesso!");

    closeCategoryForm();
    await loadCategoriesPanel();
  } catch (error) {
    showCategoryFormError("Erro ao salvar categoria. Verifique os dados e tente novamente.");
  } finally {
    setCategoryFormLoading(false);
  }
}

async function toggleCategoryStatus(category) {
  const token = getAdminToken();

  const categoryData = {
    nome: category.nome,
    descricao: category.descricao,
    ativa: !category.ativa
  };

  try {
    const response = await apiPut(`/categorias/${category.id}`, categoryData, token);

    if (!response.sucesso) {
      showToast(response.mensagem || "Erro ao alterar status da categoria.");
      return;
    }

    showToast("Status da categoria alterado com sucesso!");
    await loadCategoriesPanel();
  } catch (error) {
    showToast("Erro ao alterar status da categoria.");
  }
}

async function deleteCategory(categoryId, categoryName) {
  const confirmDelete = confirm(`Tem certeza que deseja remover a categoria "${categoryName}"?`);

  if (!confirmDelete) return;

  try {
    const token = getAdminToken();

    const response = await apiDelete(`/categorias/${categoryId}`, token);

    if (!response.sucesso) {
      showToast(response.mensagem || "Erro ao remover categoria.");
      return;
    }

    showToast("Categoria removida com sucesso!");
    await loadCategoriesPanel();
  } catch (error) {
    showToast("Erro ao remover categoria.");
  }
}

function setCategoryFormLoading(isLoading) {
  saveCategoryButton.disabled = isLoading;
  saveCategoryButton.textContent = isLoading
    ? "Salvando..."
    : categoriaIdInput.value
      ? "Atualizar categoria"
      : "Salvar categoria";
}

function showCategoryFormError(message) {
  categoryFormError.textContent = message;
  categoryFormError.classList.remove("hidden");
}

function hideCategoryFormError() {
  categoryFormError.textContent = "";
  categoryFormError.classList.add("hidden");
}

function showToast(message) {
  toastCategories.textContent = message;
  toastCategories.classList.remove("hidden");

  setTimeout(() => {
    toastCategories.classList.add("hidden");
  }, 2200);
}

function getCategoryEmoji(categoryName) {
  const name = categoryName.toLowerCase();

  if (name.includes("hamb")) return "🍔";
  if (name.includes("pizza")) return "🍕";
  if (name.includes("beb")) return "🥤";
  if (name.includes("suco")) return "🧃";
  if (name.includes("sobrem")) return "🍰";
  if (name.includes("combo")) return "🍟";
  if (name.includes("lanche")) return "🌭";

  return "🍽️";
}