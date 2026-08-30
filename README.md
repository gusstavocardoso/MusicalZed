# 🎸 Musical Zed — Loja Virtual de Instrumentos Musicais

> Loja virtual full-stack desenvolvida com **.NET 9**, **Blazor Server** e **SQLite**, com testes automatizados completos (unitários, integração, E2E e performance).

---

## 📋 Sumário

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Como Executar](#-como-executar)
  - [API](#executar-a-api)
  - [Frontend](#executar-o-frontend)
- [Documentação Swagger](#-documentação-swagger)
- [Testes](#-testes)
  - [Unitários](#testes-unitários)
  - [Integração](#testes-de-integração)
  - [E2E (Playwright)](#testes-e2e-playwright)
  - [Performance (k6)](#testes-de-performance-k6)
- [Regras de Negócio](#-regras-de-negócio)
- [Variáveis de Ambiente](#-variáveis-de-ambiente)
- [CI/CD](#-cicd)

---

## 🎵 Visão Geral

**Musical Zed** é uma loja virtual de instrumentos musicais com:

- 🏠 **Home** — hero section, categorias e produtos em destaque
- 🎸 **Catálogo** — filtro por categoria, busca por texto, cards de produtos
- 📄 **Detalhe do Produto** — imagem, avaliação, estoque, seleção de quantidade
- 🛒 **Carrinho** — adicionar, atualizar, remover itens, cálculo de frete
- 💳 **Checkout** — formulário completo de dados pessoais, endereço e pagamento
- ✅ **Confirmação de Pedido** — resumo do pedido após finalização

### Dados de exemplo

O banco é populado automaticamente com **20 produtos** em **6 categorias**:
Guitarras · Baixos · Baterias · Teclados · Amplificadores · Acessórios

Marcas incluídas: Fender, Gibson, Ibanez, Yamaha, Roland, Marshall, Boss, Ernie Ball e mais.

---

## 🏗️ Arquitetura

```
musical-zed/
├── src/
│   ├── MusicalZed.Domain/          # Entidades e interfaces
│   ├── MusicalZed.Application/     # Serviços, DTOs e contratos
│   ├── MusicalZed.Infrastructure/  # EF Core + SQLite + Repositories
│   ├── MusicalZed.API/             # ASP.NET Core Web API (porta 5000)
│   └── MusicalZed.Web/             # Blazor Server (porta 5002)
├── tests/
│   ├── MusicalZed.UnitTests/       # xUnit + Moq + FluentAssertions
│   ├── MusicalZed.IntegrationTests/ # WebApplicationFactory + SQLite isolado
│   ├── MusicalZed.E2ETests/        # Playwright + NUnit + Page Objects
│   └── MusicalZed.PerformanceTests/ # k6 scripts
├── .github/
│   └── workflows/ci.yml            # GitHub Actions CI
└── README.md
```

### Stack tecnológico

| Camada | Tecnologia |
|--------|-----------|
| Backend | ASP.NET Core 9 Web API |
| Frontend | Blazor Server (.NET 9) |
| ORM | Entity Framework Core 9 |
| Banco de dados | SQLite |
| Documentação API | Swagger (Swashbuckle 7) |
| Testes unitários | xUnit + Moq + FluentAssertions |
| Testes de integração | WebApplicationFactory + xUnit |
| Testes E2E | Playwright + NUnit (Page Object Model) |
| Testes de performance | k6 |

---

## ⚙️ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) (para k6, opcional)
- [k6](https://k6.io/docs/get-started/installation/) (apenas para testes de performance)
- [PowerShell](https://github.com/PowerShell/PowerShell) (para instalar browsers do Playwright)
- Git

---

## 📁 Estrutura do Projeto

```
src/MusicalZed.Domain/
  Entities/           → Product, Category, CartItem, Order, OrderItem
  Interfaces/         → IProductRepository, ICartRepository, etc.

src/MusicalZed.Application/
  DTOs/               → ProductDto, CartDto, OrderDto, etc.
  Interfaces/         → IProductService, ICartService, etc.
  Services/           → ProductService, CartService, OrderService, etc.

src/MusicalZed.Infrastructure/
  Data/               → MusicalZedDbContext, DataSeeder
  Repositories/       → ProductRepository, CartRepository, etc.

src/MusicalZed.API/
  Controllers/        → ProductsController, CartsController, OrdersController, etc.
  wwwroot/            → CSS customizado do Swagger
  Program.cs          → Configuração do app + Swagger + CORS + Seed

src/MusicalZed.Web/
  Components/
    Pages/            → Home, Products, ProductDetail, Cart, Checkout, OrderConfirmation
    Shared/           → ProductCard
    Layout/           → MainLayout, NavMenu
  Services/           → CartStateService
  Program.cs          → Configuração do app Blazor

tests/MusicalZed.E2ETests/
  PageObjects/        → BasePage, HomePage, ProductsPage, CartPage, etc.
  Tests/              → HomePageTests, ProductsPageTests, CartFlowTests, CheckoutFlowTests
  Helpers/            → PlaywrightSetup

tests/MusicalZed.PerformanceTests/k6/
  config/             → thresholds.js
  utils/              → helpers.js
  scenarios/          → homepage-load.js, products-load.js, cart-operations.js, checkout-flow.js
```

---

## 🚀 Como Executar

### 1. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd musical-zed
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### Executar a API

```bash
cd src/MusicalZed.API
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `http://localhost:5000/swagger`

O banco SQLite (`musicalzed.db`) é criado e populado automaticamente na primeira execução.

### Executar o Frontend

Em outro terminal:

```bash
cd src/MusicalZed.Web
dotnet run
```

O frontend estará disponível em:
- **HTTP**: `http://localhost:5002`

---

## 📖 Documentação Swagger

A API possui documentação completa via **Swagger UI**, acessível em:

```
http://localhost:5000/swagger
```

Recursos documentados:
- Todos os endpoints com descrições detalhadas
- Tipos de resposta com status codes
- Exemplos de request/response
- Regras de negócio documentadas (frete, etc.)
- Tema visual customizado Musical Zed
- Agrupamento por tags: **Produtos**, **Categorias**, **Carrinho**, **Pedidos**

---

## 🧪 Testes

### Testes Unitários

```bash
dotnet test tests/MusicalZed.UnitTests
```

Cobertura:
- `ProductService` — 7 testes
- `CartService` — 7 testes
- `OrderService` — 7 testes

### Testes de Integração

```bash
dotnet test tests/MusicalZed.IntegrationTests
```

Utiliza `WebApplicationFactory` com SQLite isolado por teste.

Cobertura:
- `ProductsApiTests` — 6 testes
- `CategoriesApiTests` — 3 testes
- `CartApiTests` — 4 testes
- `OrderApiTests` — 4 testes

### Testes E2E (Playwright)

#### 1. Buildar o projeto E2E

```bash
dotnet build tests/MusicalZed.E2ETests
```

#### 2. Instalar os browsers

```bash
# Windows (PowerShell)
pwsh tests/MusicalZed.E2ETests/bin/Debug/net9.0/playwright.ps1 install chromium

# Linux / macOS
./tests/MusicalZed.E2ETests/bin/Debug/net9.0/playwright.sh install chromium
```

#### 3. Iniciar a aplicação

```bash
# Terminal 1 — API
cd src/MusicalZed.API && dotnet run

# Terminal 2 — Frontend
cd src/MusicalZed.Web && dotnet run
```

#### 4. Executar os testes E2E

```bash
dotnet test tests/MusicalZed.E2ETests \
  --settings tests/MusicalZed.E2ETests/playwright.runsettings
```

Ou apontando para outro ambiente:

```bash
E2E_BASE_URL=http://localhost:5002 dotnet test tests/MusicalZed.E2ETests
```

**Page Objects implementados:**
- `HomePage` — hero, categorias, produtos em destaque
- `ProductsPage` — filtros, busca, grid de produtos
- `ProductDetailPage` — detalhe, quantidade, add to cart
- `CartPage` — itens, atualizar, remover, checkout
- `CheckoutPage` — formulário completo
- `OrderConfirmationPage` — sucesso do pedido

### Testes de Performance (k6)

#### Instalar k6

```bash
# Windows
winget install k6 --source winget

# macOS
brew install k6

# Linux (Debian/Ubuntu)
sudo apt-get install k6
```

#### Executar os cenários

Inicie a API antes de rodar os testes:

```bash
cd src/MusicalZed.API && dotnet run
```

```bash
# 1. Home page (smoke + load + stress)
k6 run tests/MusicalZed.PerformanceTests/k6/scenarios/homepage-load.js

# 2. Listagem e busca de produtos
k6 run tests/MusicalZed.PerformanceTests/k6/scenarios/products-load.js

# 3. Operações de carrinho
k6 run tests/MusicalZed.PerformanceTests/k6/scenarios/cart-operations.js

# 4. Fluxo completo de checkout
k6 run tests/MusicalZed.PerformanceTests/k6/scenarios/checkout-flow.js

# Com API em outro host
k6 run -e API_URL=https://minha-api.com tests/MusicalZed.PerformanceTests/k6/scenarios/products-load.js

# Exportar resultados
k6 run --out json=results.json tests/MusicalZed.PerformanceTests/k6/scenarios/products-load.js
```

**Thresholds (SLA):**

| Métrica | Limite |
|---------|--------|
| `http_req_duration p(95)` | < 2.000 ms |
| `http_req_duration p(99)` | < 5.000 ms |
| `http_req_failed` | < 1% |
| `cart_operation_time p(95)` | < 1.000 ms |
| `checkout_success_rate` | > 90% |

---

## 📜 Regras de Negócio

| Regra | Detalhe |
|-------|---------|
| **Frete grátis** | Subtotal ≥ R$ 500,00 |
| **Frete padrão** | Subtotal < R$ 500,00 → R$ 29,90 |
| **Carrinho por sessão** | Identificado por UUID (`sessionId`) |
| **Limpeza automática** | Carrinho limpo após finalização do pedido |
| **Soft delete** | Produtos desativados, não excluídos |
| **Incremento de quantidade** | Adicionar item já existente incrementa a quantidade |

---

## 🔧 Variáveis de Ambiente

### API (`src/MusicalZed.API/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=musicalzed.db"
  }
}
```

### Web (`src/MusicalZed.Web/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=musicalzed-web.db"
  }
}
```

### E2E Tests

| Variável | Padrão | Descrição |
|----------|--------|-----------|
| `E2E_BASE_URL` | `http://localhost:5002` | URL do frontend |

### k6

| Variável | Padrão | Descrição |
|----------|--------|-----------|
| `API_URL` | `http://localhost:5000` | URL da API |
| `WEB_URL` | `http://localhost:5002` | URL do frontend |

---

## 🔄 CI/CD

O projeto utiliza **GitHub Actions** com os seguintes jobs:

```
build → unit-tests ──┐
                     ├── e2e-tests
build → integration-tests ──┘
       └── performance-validation (apenas main)
```

| Job | Descrição |
|-----|-----------|
| `build` | Compila toda a solução em Release |
| `unit-tests` | Executa testes unitários |
| `integration-tests` | Executa testes de integração da API |
| `e2e-tests` | Inicia API + Web e executa Playwright |
| `performance-validation` | Executa scripts k6 (apenas em push para `main`) |

Arquivo de configuração: `.github/workflows/ci.yml`

---

## 📝 Licença

MIT © Musical Zed
