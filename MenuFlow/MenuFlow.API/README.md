# 🍔 MenuFlow — Sistema de Delivery para Restaurantes

MenuFlow é um sistema web de delivery para restaurantes, desenvolvido com **C#/.NET**, **PostgreSQL** e **JavaScript**, com foco em boas práticas de backend, autenticação, painel administrativo e frontend responsivo.

O projeto simula um sistema real onde clientes podem visualizar o cardápio, adicionar produtos ao carrinho, finalizar pedidos e acompanhar o status. O administrador pode gerenciar produtos, categorias, pedidos e visualizar métricas no dashboard.

---

## 🚀 Objetivo do projeto

Este projeto foi desenvolvido como parte do meu portfólio para demonstrar conhecimentos em:

- Desenvolvimento de APIs REST com C# e ASP.NET Core
- Modelagem de banco de dados relacional
- Entity Framework Core
- Autenticação com JWT
- Criptografia de senhas com BCrypt
- Validações com FluentValidation
- Organização em camadas
- Consumo de API com JavaScript
- Frontend responsivo
- Boas práticas de segurança e manutenção

---

## 🧩 Funcionalidades

### 👤 Área do Cliente

- Visualização do cardápio
- Busca de produtos
- Filtro por categorias
- Carrinho de compras
- Controle de quantidade dos itens
- Checkout com dados do cliente
- Escolha da forma de pagamento
- Criação de pedido
- Acompanhamento de pedidos ativos por telefone
- Exibição de pedidos das últimas 24 horas
- Interface responsiva para celular e computador

---

### 🔐 Área Administrativa

- Login administrativo com JWT
- Dashboard com métricas do restaurante
- Total de pedidos
- Pedidos do dia
- Pedidos pendentes
- Faturamento total
- Faturamento do dia
- Produtos mais vendidos
- Pedidos por status
- Listagem de pedidos
- Alteração de status dos pedidos
- Cadastro, edição e remoção de produtos
- Cadastro, edição e remoção de categorias
- Controle de disponibilidade dos produtos

---

## 🛡️ Segurança

O projeto possui algumas melhorias de segurança importantes:

- Rotas administrativas protegidas com JWT
- Senhas criptografadas com BCrypt
- Middleware global para tratamento de erros
- Respostas padronizadas com `ApiResponse`
- Validação de entrada com FluentValidation
- Acompanhamento de pedidos por telefone limitado às últimas 24 horas
- Endpoint de busca por ID protegido para administradores
- Dados sensíveis do pedido parcialmente protegidos no acompanhamento público

---

## 🛠️ Tecnologias utilizadas

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- BCrypt.Net
- FluentValidation
- Swagger
- LINQ
- Middleware personalizado

### Frontend

- HTML5
- CSS3
- JavaScript
- Fetch API
- LocalStorage
- Design responsivo
- Animações e interações com CSS/JS

### Ferramentas

- Visual Studio Code
- Postman
- Git
- GitHub
- PostgreSQL

---

## 🏗️ Arquitetura do Backend

O backend foi organizado em camadas para melhorar manutenção e escalabilidade:

```txt
MenuFlow.API
├── Controllers
├── Data
├── DTOs
├── Enums
├── Exceptions
├── Middlewares
├── Migrations
├── Models
├── Repositories
├── Responses
├── Services
├── Validators
└── Program.cs