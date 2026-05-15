const ordersLoading = document.getElementById("orders-loading");
const ordersList = document.getElementById("orders-list");
const ordersEmpty = document.getElementById("orders-empty");
const ordersError = document.getElementById("orders-error");
const statusFilter = document.getElementById("status-filter");
const refreshOrdersButton = document.getElementById("refresh-orders-button");
const logoutButtonOrders = document.getElementById("logout-button");
const toastOrders = document.getElementById("toast");

const adminUserNameOrders = document.getElementById("admin-user-name");
const adminUserEmailOrders = document.getElementById("admin-user-email");

let allOrders = [];

document.addEventListener("DOMContentLoaded", async () => {
  requireAdminAuth();

  const user = getAdminUser();

  if (user) {
    adminUserNameOrders.textContent = user.nome;
    adminUserEmailOrders.textContent = user.email;
  }

  logoutButtonOrders.addEventListener("click", logoutAdmin);
  refreshOrdersButton.addEventListener("click", loadOrders);
  statusFilter.addEventListener("change", renderOrders);

  await loadOrders();
});

async function loadOrders() {
  try {
    const token = getAdminToken();

    ordersLoading.classList.remove("hidden");
    ordersList.classList.add("hidden");
    ordersEmpty.classList.add("hidden");
    ordersError.classList.add("hidden");

    const response = await apiGet("/pedidos", token);

    if (!response.sucesso) {
      throw new Error(response.mensagem || "Erro ao carregar pedidos.");
    }

    allOrders = response.dados || [];

    renderOrders();
  } catch (error) {
    ordersError.textContent = "Erro ao carregar pedidos. Faça login novamente ou verifique a API.";
    ordersError.classList.remove("hidden");
  } finally {
    ordersLoading.classList.add("hidden");
  }
}

function renderOrders() {
  const selectedStatus = statusFilter.value;

  let filteredOrders = [...allOrders];

  if (selectedStatus !== "all") {
    filteredOrders = filteredOrders.filter(order => String(order.status) === selectedStatus);
  }

  ordersList.innerHTML = "";

  if (filteredOrders.length === 0) {
    ordersList.classList.add("hidden");
    ordersEmpty.classList.remove("hidden");
    return;
  }

  ordersEmpty.classList.add("hidden");
  ordersList.classList.remove("hidden");

  filteredOrders.forEach(order => {
    const card = createOrderCard(order);
    ordersList.appendChild(card);
  });
}

function createOrderCard(order) {
  const article = document.createElement("article");
  article.className = "admin-order-card";

  const statusText = formatStatus(order.status);

  article.innerHTML = `
    <div class="admin-order-header">
      <div>
        <span class="section-kicker">Pedido #${order.id}</span>
        <h3>${order.nomeCliente}</h3>
        <p>${formatDate(order.criadoEm)}</p>
      </div>

      <span class="status-badge status-${order.status}">
        ${statusText}
      </span>
    </div>

    <div class="admin-order-grid">
      <div class="order-info-box">
        <span>Telefone</span>
        <strong>${order.telefoneCliente}</strong>
      </div>

      <div class="order-info-box">
        <span>Total</span>
        <strong>${formatCurrency(order.total)}</strong>
      </div>

      <div class="order-info-box full">
        <span>Endereço</span>
        <strong>${order.enderecoEntrega}</strong>
      </div>

      <div class="order-info-box full">
        <span>Observação</span>
        <strong>${order.observacao || "Nenhuma observação."}</strong>
      </div>
    </div>

    <div class="admin-order-items">
      <h4>Itens</h4>
      ${order.itens.map(item => `
        <div class="admin-order-item">
          <span>${item.quantidade}x ${item.nomeProduto}</span>
          <strong>${formatCurrency(item.subtotal)}</strong>
        </div>
      `).join("")}
    </div>

    <div class="admin-order-footer">
      <div class="admin-status-control">
        <label>Alterar status</label>
        <select class="admin-select order-status-select">
          <option value="1" ${order.status === 1 ? "selected" : ""}>Recebido</option>
          <option value="2" ${order.status === 2 ? "selected" : ""}>Em preparo</option>
          <option value="3" ${order.status === 3 ? "selected" : ""}>Saiu para entrega</option>
          <option value="4" ${order.status === 4 ? "selected" : ""}>Entregue</option>
          <option value="5" ${order.status === 5 ? "selected" : ""}>Cancelado</option>
        </select>
      </div>

      <button class="admin-action-button save-status-button">
        Salvar status
      </button>
    </div>
  `;

  const select = article.querySelector(".order-status-select");
  const saveButton = article.querySelector(".save-status-button");

  saveButton.addEventListener("click", async () => {
    const newStatus = Number(select.value);
    await updateOrderStatus(order.id, newStatus);
  });

  return article;
}

async function updateOrderStatus(orderId, status) {
  try {
    const token = getAdminToken();

    const response = await apiPatch(`/pedidos/${orderId}/status?status=${status}`, token);

    if (!response.sucesso) {
      showToast(response.mensagem || "Erro ao atualizar status.");
      return;
    }

    showToast("Status atualizado com sucesso!");

    await loadOrders();
  } catch (error) {
    showToast("Erro ao atualizar status. Verifique a API.");
  }
}

function formatStatus(status) {
  const statusMap = {
    1: "Recebido",
    2: "Em preparo",
    3: "Saiu para entrega",
    4: "Entregue",
    5: "Cancelado"
  };

  return statusMap[status] || "Desconhecido";
}

function formatDate(dateString) {
  const date = new Date(dateString);

  return date.toLocaleString("pt-BR", {
    dateStyle: "short",
    timeStyle: "short"
  });
}

function formatCurrency(value) {
  return value.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL"
  });
}

function showToast(message) {
  toastOrders.textContent = message;
  toastOrders.classList.remove("hidden");

  setTimeout(() => {
    toastOrders.classList.add("hidden");
  }, 2200);
}