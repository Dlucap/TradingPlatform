# OrderAccumulator - Trading Platform

Sistema de acumulação e processamento de ordens financeiras utilizando protocolo FIX (Financial Information eXchange).

## 📋 Arquitetura

O projeto é composto por três componentes principais:

- **Frontend (React + Vite)**: Interface web para geração e envio de ordens
- **API (ASP.NET Core)**: Backend REST que atua como **FIX Initiator** e processa as requisições
- **Worker (Console App)**: Serviço que atua como **FIX Acceptor** e acumula as ordens conforme regras de negócio

```
┌─────────────┐      HTTP/REST       ┌──────────────┐      FIX Protocol      ┌─────────────┐
│   React UI  │ ───────────────────> │  API (.NET)  │ <───────────────────>  │   Worker    │
│   (Port ??) │     JSON Orders      │ (Port 7225)  │   (Port 5001 FIX)      │   (.NET)    │
└─────────────┘                      └──────────────┘                        └─────────────┘
                                      FIX Initiator                          FIX Acceptor
```

## 🛠 Tecnologias Utilizadas

### Backend
- **.NET 10** - Framework principal
- **ASP.NET Core** - Web API
- **QuickFIX/n** - Implementação do protocolo FIX 4.4
- **C# 13** - Linguagem de programação

### Frontend
- **React 19.2.5** - Biblioteca UI
- **TypeScript 6.0.2** - Tipagem estática
- **Vite 8.0.10** - Build tool e dev server

### Testes
- **xUnit 2.9.2** - Framework de testes
- **Moq 4.20.72** - Mocking
- **FluentAssertions 8.9.0** - Assertions

### Arquitetura
- **Clean Architecture** - Separação em camadas (Domain, Application, Infrastructure)
- **DDD** - Domain-Driven Design
- **SOLID** - Princípios de design

## 📁 Estrutura do Projeto

```
TradingPlatform/
├── BackEnd/TradingPlatform/
│   ├── OrderAccumulator.Api/          # API REST (FIX Initiator)
│   ├── OrderAccumulator.Application/  # Regras de negócio
│   ├── OrderAccumulator.Domain/       # Entidades e interfaces
│   ├── OrderAccumulator.Infrastructure/# FIX, persistência, serviços
│   ├── OrderAccumulator.Worker/       # Worker FIX Acceptor
│   └── Testes/OrderAccumulator.Tests/ # Testes unitários (55 testes)
└── FrontEnd/order-generator-ui/       # Interface React
```

## 🚀 Instalação e Execução

### Pré-requisitos

- **.NET SDK 10** ou superior
- **Node.js 18+** e npm
- **Visual Studio 2026** (recomendado) ou VS Code

### 1. Configurar e Executar o Worker (FIX Acceptor)

O Worker deve ser iniciado **primeiro** pois atua como servidor FIX:

```bash
cd BackEnd\TradingPlatform\OrderAccumulator.Worker
dotnet run
```

O Worker irá:
- Iniciar o FIX Acceptor na porta **5001**
- Aguardar conexões de iniciadores FIX
- Processar e acumular ordens recebidas

### 2. Configurar e Executar a API (FIX Initiator)

```bash
cd BackEnd\TradingPlatform\OrderAccumulator.Api
dotnet run
```

A API irá:
- Iniciar na porta **HTTPS: 7225** / **HTTP: 5287**
- Conectar-se automaticamente ao Worker via FIX
- Expor endpoints REST para o frontend

**Endpoints disponíveis:**
- `POST /api/orders` - Enviar nova ordem

### 3. Configurar e Executar o Frontend

```bash
cd FrontEnd\order-generator-ui

# Instalar dependências (primeira vez)
npm install

# Configurar URL da API
cp .env.example .env
# Edite o arquivo .env se necessário (padrão: https://localhost:7225)

# Executar em modo desenvolvimento
npm run dev
```

O frontend estará disponível em: `http://localhost:5173` (porta padrão do Vite)

## 🧪 Executar Testes Unitários

O projeto possui **55 testes unitários** cobrindo as três camadas principais:

```bash
# Executar todos os testes
cd BackEnd\TradingPlatform
dotnet test Testes\OrderAccumulator.Tests\OrderAccumulator.Tests.csproj

# Executar com output detalhado
dotnet test Testes\OrderAccumulator.Tests\OrderAccumulator.Tests.csproj --logger "console;verbosity=detailed"

# Executar apenas testes de uma camada específica
dotnet test --filter "FullyQualifiedName~Domain"
dotnet test --filter "FullyQualifiedName~Application"
dotnet test --filter "FullyQualifiedName~Infrastructure"
```

**Cobertura de Testes:**
- Domain: 11 testes
- Application: 13 testes
- Infrastructure: 31 testes
- **Total: 55 testes** ✅

Para mais detalhes sobre os testes, consulte: [Testes/OrderAccumulator.Tests/README.md](Testes/OrderAccumulator.Tests/README.md)

## 🔧 Configuração

### Configuração FIX

As configurações do protocolo FIX estão em:

- **API (Initiator)**: `OrderAccumulator.Api/initiator.cfg`
- **Worker (Acceptor)**: `OrderAccumulator.Worker/acceptor.cfg`

**Porta FIX padrão:** 5001

### Configuração da API URL no Frontend

O frontend utiliza variáveis de ambiente do Vite para configurar a URL da API:

```env
# .env
VITE_API_URL=https://localhost:7225
```

Arquivos de configuração disponíveis:
- `.env.development` - Desenvolvimento local
- `.env.production` - Produção
- `.env.example` - Template de configuração

## 🐛 Troubleshooting

### Erro: "Não foi possível conectar ao OrderAccumulator"

**Causa:** O Worker não está rodando ou a conexão FIX não foi estabelecida.

**Solução:**
1. Certifique-se de que o Worker está rodando **antes** da API
2. Verifique se a porta 5001 não está em uso
3. Aguarde alguns segundos para a conexão FIX ser estabelecida

### Erro: "Unable to resolve service for type 'IOrderService'"

**Causa:** Serviços não registrados no container DI.

**Solução:** Já corrigido na versão atual. Se persistir, verifique `Program.cs` da API.

### Erro de CORS no Frontend

**Causa:** Frontend tentando acessar a API com URL incorreta.

**Solução:** Verifique o arquivo `.env` e certifique-se de que `VITE_API_URL` está correto.

## 📝 Como Usar

1. **Inicie o Worker** (terminal 1)
2. **Inicie a API** (terminal 2) - aguarde conexão FIX ser estabelecida
3. **Inicie o Frontend** (terminal 3)
4. Acesse `http://localhost:5173` no navegador
5. Preencha os campos da ordem e clique em "Enviar Ordem"
6. O sistema processará via FIX e acumulará conforme as regras de negócio

## 📚 Documentação Adicional

- [README do Frontend](FrontEnd/order-generator-ui/README.md)
- [README dos Testes](Testes/OrderAccumulator.Tests/README.md)
- [Documentação QuickFIX/n](https://github.com/connamara/quickfixn)

## 👨‍💻 Desenvolvimento

### Build de Produção

```bash
# Backend
cd BackEnd\TradingPlatform
dotnet build -c Release

# Frontend
cd FrontEnd\order-generator-ui
npm run build
```

### Estrutura de Logs

- API: Console output e logs estruturados
- Worker: Console output com mensagens FIX
- Frontend: Console do navegador

---

> This is a challenge by [Coodesh](https://coodesh.com/)
