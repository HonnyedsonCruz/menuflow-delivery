const dashboardLoading = document.getElementById("dashboard-loading");
const dashboardContent = document.getElementById("dashboard-content");
const dashboardError = document.getElementById("dashboard-error");

const adminUserName = document.getElementById("admin-user-name");
const adminUserEmail = document.getElementById("admin-user-email");
const logoutButton = document.getElementById("logout-button");

document.addEventListener("DOMContentLoaded", async () => {
  requireAdminAuth();

  const user = getAdminUser();

  if (user) {
    adminUserName.textContent = user.nome;
    adminUserEmail.textContent = user.email;
  }

  logoutButton.addEventListener("click", logoutAdmin);

  await loadDashboard();
});

async function loadDashboard() {
  try {
    const token = getAdminToken();

    dashboardLoading.classList.remove("hidden");
    dashboardContent.classList.add("hidden");
    dashboardError.classList.add("hidden");

    const resumoResponse = await apiGet("/dashboard/resumo", token);
    const topProductsResponse = await apiGet("/dashboard/produtos-mais-vendidos", token);
    const statusResponse = await apiGet("/dashboard/pedidos-por-status", token);

    if (!resumoResponse.sucesso) {
      throw new Error(resumoResponse.mensagem);
    }

    renderResumo(resumoResponse.dados);
    renderTopProducts(topProductsResponse.dados || []);
    renderStatus(statusResponse.dados || []);

    dashboardContent.classList.remove("hidden");
  } catch (error) {
    dashboardError.textContent = "Erro ao carregar dashboard. Faça login novamente ou verifique a API.";
    dashboardError.classList.remove("hidden");
  } finally {
    dashboardLoading.classList.add("hidden");
  }
}

function renderResumo(resumo) {
  document.getElementById("totalPedidos").textContent = resumo.totalPedidos;
  document.getElementById("pedidosHoje").textContent = resumo.pedidosHoje;
  document.getElementById("pedidosPendentes").textContent = resumo.pedidosPendentes;
  document.getElementById("faturamentoTotal").textContent = formatCurrency(resumo.faturamentoTotal);
  document.getElementById("faturamentoHoje").textContent = formatCurrency(resumo.faturamentoHoje);
}

function renderTopProducts(products) {
  const container = document.getElementById("top-products-list");
  container.innerHTML = "";

  if (products.length === 0) {
    container.innerHTML = `<p class="admin-empty">Nenhum produto vendido ainda.</p>`;
    return;
  }

  products.forEach((product, index) => {
    const div = document.createElement("div");
    div.className = "admin-list-item";

    div.innerHTML = `
      <div>
        <strong>${index + 1}. ${product.nomeProduto}</strong>
        <span>${product.quantidadeVendida} unidades vendidas</span>
      </div>
      <p>${formatCurrency(product.totalVendido)}</p>
    `;

    container.appendChild(div);
  });
}

function renderStatus(statusList) {
  const container = document.getElementById("status-list");
  container.innerHTML = "";

  if (statusList.length === 0) {
    container.innerHTML = `<p class="admin-empty">Nenhum pedido registrado.</p>`;
    return;
  }

  statusList.forEach(item => {
    const div = document.createElement("div");
    div.className = "admin-list-item";

    div.innerHTML = `
      <div>
        <strong>${formatStatusName(item.status)}</strong>
        <span>Status do pedido</span>
      </div>
      <p>${item.quantidade}</p>
    `;

    container.appendChild(div);
  });
}

function formatStatusName(status) {
  const map = {
    Recebido: "Recebido",
    EmPreparo: "Em preparo",
    SaiuParaEntrega: "Saiu para entrega",
    Entregue: "Entregue",
    Cancelado: "Cancelado"
  };

  return map[status] || status;
}

function formatCurrency(value) {
  return value.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}