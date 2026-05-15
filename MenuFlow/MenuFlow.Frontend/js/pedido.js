const telefoneInput = document.getElementById("telefoneInput");
const buscarPedidosButton = document.getElementById("buscarPedidosButton");

const pedidoError = document.getElementById("pedido-error");
const pedidoLoading = document.getElementById("pedido-loading");
const pedidosContent = document.getElementById("pedidos-content");
const pedidosEmpty = document.getElementById("pedidos-empty");

let currentTelefone = null;

document.addEventListener("DOMContentLoaded", () => {
  const params = new URLSearchParams(window.location.search);
  const telefone = params.get("telefone");

  if (telefone) {
    telefoneInput.value = telefone;
    buscarPedidosPorTelefone(telefone);
  }

  buscarPedidosButton.addEventListener("click", () => {
    const telefoneDigitado = telefoneInput.value.trim();

    if (!telefoneDigitado) {
      showError("Informe o telefone usado no pedido.");
      return;
    }

    buscarPedidosPorTelefone(telefoneDigitado);
  });
});

async function buscarPedidosPorTelefone(telefone) {
  try {
    hideError();
    setLoading(true);

    currentTelefone = telefone;

    const response = await apiGet(`/pedidos/acompanhar?telefone=${encodeURIComponent(telefone)}`);

    if (!response.sucesso) {
      showError(response.mensagem || "Não foi possível buscar seus pedidos.");
      pedidosContent.classList.add("hidden");
      pedidosEmpty.classList.remove("hidden");
      return;
    }

    const pedidos = response.dados || [];

    renderPedidos(pedidos);
  } catch (error) {
    showError("Erro ao buscar pedidos. Verifique se a API está rodando.");
    pedidosContent.classList.add("hidden");
    pedidosEmpty.classList.remove("hidden");
  } finally {
    setLoading(false);
  }
}

function renderPedidos(pedidos) {
  pedidosContent.innerHTML = "";

  if (pedidos.length === 0) {
    pedidosContent.classList.add("hidden");
    pedidosEmpty.classList.remove("hidden");
    return;
  }

  pedidosEmpty.classList.add("hidden");
  pedidosContent.classList.remove("hidden");

  pedidos.forEach(pedido => {
    const card = createPedidoCard(pedido);
    pedidosContent.appendChild(card);
  });
}

function createPedidoCard(pedido) {
  const article = document.createElement("article");
  article.className = "order-details order-track-card";

  article.innerHTML = `
    <div class="order-main-card">
      <div class="order-header">
        <div>
          <span class="section-kicker">Pedido ativo</span>
          <h3>Pedido #${pedido.id}</h3>
          <p>Criado em ${formatDate(pedido.criadoEm)}</p>
        </div>

        <span class="status-badge status-${pedido.status}">
          ${formatStatus(pedido.status)}
        </span>
      </div>

      ${createTimelineHtml(pedido.status)}

      <div class="order-info-grid">
        <div class="order-info-box">
          <span>Cliente</span>
          <strong>${pedido.nomeCliente}</strong>
        </div>

        <div class="order-info-box">
          <span>Pagamento</span>
          <strong>${formatPayment(pedido.formaPagamento)}</strong>
        </div>

        <div class="order-info-box full">
          <span>Endereço</span>
          <strong>${pedido.enderecoResumo || "Endereço protegido por segurança."}</strong>
        </div>

        <div class="order-info-box full">
          <span>Observação</span>
          <strong>${pedido.observacao || "Nenhuma observação."}</strong>
        </div>
      </div>
    </div>

    <aside class="summary-card">
      <h3>Itens do pedido</h3>

      <div class="checkout-items">
        ${pedido.itens.map(item => `
          <div class="checkout-item">
            <div>
              <strong>${item.nomeProduto}</strong>
              <span>${item.quantidade}x ${formatCurrency(item.precoUnitario)}</span>
            </div>
            <p>${formatCurrency(item.subtotal)}</p>
          </div>
        `).join("")}
      </div>

      <div class="summary-divider"></div>

      <div class="summary-row">
        <span>Subtotal</span>
        <strong>${formatCurrency(pedido.subtotal)}</strong>
      </div>

      <div class="summary-row">
        <span>Taxa de entrega</span>
        <strong>${formatCurrency(pedido.taxaEntrega)}</strong>
      </div>

      <div class="summary-divider"></div>

      <div class="summary-row total">
        <span>Total</span>
        <strong>${formatCurrency(pedido.total)}</strong>
      </div>

      <button class="clear-button refresh-button refresh-single-button">
        Atualizar status
      </button>
    </aside>
  `;

  const refreshButton = article.querySelector(".refresh-single-button");

  refreshButton.addEventListener("click", () => {
    if (currentTelefone) {
      buscarPedidosPorTelefone(currentTelefone);
    }
  });

  return article;
}

function createTimelineHtml(status) {
  const steps = [
    { id: 1, label: "Recebido" },
    { id: 2, label: "Em preparo" },
    { id: 3, label: "Saiu para entrega" },
    { id: 4, label: "Entregue" }
  ];

  if (status === 5) {
    return `
      <div class="status-timeline canceled-timeline">
        <div class="timeline-step active">
          <span>!</span>
          <p>Pedido cancelado</p>
        </div>
      </div>
    `;
  }

  const currentIndex = steps.findIndex(step => step.id === status);

  let html = `<div class="status-timeline">`;

  steps.forEach((step, index) => {
    const completed = index < currentIndex ? "completed" : "";
    const active = index === currentIndex ? "active" : "";

    html += `
      <div class="timeline-step ${completed} ${active}">
        <span>${step.id}</span>
        <p>${step.label}</p>
      </div>
    `;

    if (index < steps.length - 1) {
      const lineActive = index < currentIndex ? "active" : "";
      html += `<div class="timeline-line ${lineActive}"></div>`;
    }
  });

  html += `</div>`;

  return html;
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

function formatPayment(payment) {
  const paymentMap = {
    1: "Pix",
    2: "Dinheiro",
    3: "Cartão na entrega",
    4: "Cartão online"
  };

  return paymentMap[payment] || "Não informado";
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

function showError(message) {
  pedidoError.textContent = message;
  pedidoError.classList.remove("hidden");
}

function hideError() {
  pedidoError.textContent = "";
  pedidoError.classList.add("hidden");
}

function setLoading(isLoading) {
  pedidoLoading.classList.toggle("hidden", !isLoading);
}