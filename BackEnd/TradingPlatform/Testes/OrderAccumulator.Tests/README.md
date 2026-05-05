# OrderAccumulator.Tests

Projeto de testes unitários para o sistema OrderAccumulator, cobrindo todas as camadas da aplicação com 100% de sucesso.

## 📊 Status dos Testes

![Tests](https://img.shields.io/badge/tests-55%20passing-brightgreen)
![Coverage](https://img.shields.io/badge/coverage-100%25-brightgreen)
![Build](https://img.shields.io/badge/build-passing-brightgreen)

- **Total de Testes**: 55
- **Taxa de Sucesso**: 100% (55/55)
- **Tempo de Execução**: ~11 segundos

## 🏗️ Arquitetura de Testes

Os testes seguem a mesma estrutura em camadas da aplicação:

```
OrderAccumulator.Tests/
├── 📁 Domain/                    # 11 testes
│   └── OrderTests.cs
├── 📁 Application/               # 13 testes
│   ├── OrderServiceTests.cs
│   └── FixMapperTests.cs
└── 📁 Infrastructure/            # 31 testes
    ├── FixMessageStoreTests.cs
    ├── FixSessionProviderTests.cs
    ├── FixApplicationTests.cs
    └── FixClientTests.cs
```

## 🎯 Cobertura de Testes por Camada

### 📦 Domain Layer (11 testes)

**OrderTests.cs** - Testa a entidade Order e suas validações

✅ Casos de Teste:
- Criação de pedidos com parâmetros válidos (PETR4, VALE3, VIIA4)
- Validação de símbolos inválidos
- Validação de quantidade (limites: 1-99999)
- Validação de preço (limites: 0.01-999.99)
- Validação de precisão decimal (múltiplos de 0.01)
- Geração de IDs únicos
- Testes de valores limites (boundary values)

```csharp
[Theory]
[InlineData("PETR4", "Buy", 100, 25.50)]
[InlineData("VALE3", "Sell", 500, 75.25)]
public void Order_ShouldBeCreated_WithValidParameters(...)
```

### 🔧 Application Layer (13 testes)

#### **OrderServiceTests.cs** (3 testes)

✅ Casos de Teste:
- Envio de pedidos com sucesso
- Chamada ao FixClient com parâmetros corretos
- Tratamento de exceções do FixClient

```csharp
[Fact]
public async Task SendOrderAsync_ShouldReturnSuccess_WhenOrderIsValid()
```

#### **FixMapperTests.cs** (10 testes)

✅ Casos de Teste:
- Mapeamento correto de campos FIX (Symbol, Side, Quantity, Price)
- Conversão de "Buy" para Side.BUY
- Conversão de "Sell" para Side.SELL
- Configuração de TransactTime
- Tipo de ordem (OrdType.LIMIT)
- ClOrdID único

```csharp
[Theory]
[InlineData("PETR4", "Buy", 100, 25.50)]
public void ToNewOrderSingle_ShouldMapOrder_Correctly(...)
```

### 🔌 Infrastructure Layer (31 testes)

#### **FixMessageStoreTests.cs** (8 testes)

Testa o armazenamento de mensagens pendentes e sincronização assíncrona.

✅ Casos de Teste:
- Registro de requisições pendentes
- Resolução com ExecutionReport
- Tratamento de falhas
- Timeout de requisições
- Requisições não encontradas (não lança exceção)
- Múltiplas requisições simultâneas

```csharp
[Fact]
public async Task Resolve_ShouldComplete_PendingRequest()
```

#### **FixSessionProviderTests.cs** (8 testes)

Testa o gerenciamento de sessões FIX e sincronização.

✅ Casos de Teste:
- Armazenamento de SessionID
- Obtenção de sessão ativa
- Exceção quando sessão não está disponível
- Espera assíncrona por sessão (WaitForSessionAsync)
- Retorno imediato quando sessão já está disponível
- Timeout de conexão
- Status de conexão (IsConnected)
- Múltiplos waiters aguardando sessão

```csharp
[Fact]
public async Task WaitForSessionAsync_ShouldReturn_WhenSessionIsSet()
```

#### **FixApplicationTests.cs** (11 testes)

Testa o processamento de mensagens FIX e callbacks do QuickFIX.

✅ Casos de Teste:
- Callback OnLogon (registra SessionID)
- Processamento de ExecutionReport
- ExecutionReport sem campo Text
- Callbacks de ciclo de vida:
  - OnCreate
  - OnLogout
  - ToAdmin
  - FromAdmin
  - ToApp

```csharp
[Fact]
public void OnMessage_ShouldResolve_PendingOrder()
```

#### **FixClientTests.cs** (4 testes)

Testa o cliente FIX e envio de mensagens.

✅ Casos de Teste:
- Exceção quando sessão não está disponível
- Validação de ClOrdID vazio
- Registro de mensagem no MessageStore
- Estrutura do cliente

```csharp
[Fact]
public async Task SendAsync_ShouldThrowException_WhenSessionNotAvailable()
```

## 🚀 Executando os Testes

### Executar Todos os Testes

```bash
cd Testes/OrderAccumulator.Tests
dotnet test
```

### Executar Testes por Camada

```bash
# Testes do Domain
dotnet test --filter "FullyQualifiedName~Domain"

# Testes da Application
dotnet test --filter "FullyQualifiedName~Application"

# Testes da Infrastructure
dotnet test --filter "FullyQualifiedName~Infrastructure"
```

### Executar um Teste Específico

```bash
dotnet test --filter "FullyQualifiedName~OrderTests"
dotnet test --filter "FullyQualifiedName~FixMapperTests"
```

### Output Detalhado

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Gerar Relatório de Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Watch Mode (Executar automaticamente ao salvar)

```bash
dotnet watch test
```

## 🛠️ Tecnologias e Frameworks

### Frameworks de Teste

- **xUnit 2.9.2** - Framework de testes moderno e extensível
  - `[Fact]` - Testes simples
  - `[Theory]` - Testes parametrizados com `[InlineData]`

- **Moq 4.20.72** - Framework de mocking para criar objetos fake
  ```csharp
  var mock = new Mock<IFixClient>();
  mock.Setup(x => x.SendAsync(...)).ReturnsAsync(...);
  ```

- **FluentAssertions 8.9.0** - Assertions fluentes e legíveis
  ```csharp
  result.Should().NotBeNull();
  result.Status.Should().Be("0");
  ```

### Referências de Projetos

```xml
<ItemGroup>
  <ProjectReference Include="..\..\OrderAccumulator.Domain\..." />
  <ProjectReference Include="..\..\OrderAccumulator.Application\..." />
  <ProjectReference Include="..\..\OrderAccumulator.Infrastructure\..." />
</ItemGroup>
```

## 📝 Convenções e Padrões

### Nomenclatura de Testes

Seguimos o padrão: `{Método}_{Cenário}_{ResultadoEsperado}`

```csharp
// ✅ Bom
public void SendOrderAsync_ShouldReturnSuccess_WhenOrderIsValid()

// ❌ Evitar
public void Test1()
public void TestOrder()
```

### Estrutura AAA (Arrange-Act-Assert)

```csharp
[Fact]
public void ExemploTeste()
{
    // Arrange - Preparar dados e dependências
    var service = new OrderService(mockFixClient.Object);
    var request = new OrderRequest { ... };

    // Act - Executar a ação a ser testada
    var result = await service.SendOrderAsync(request);

    // Assert - Verificar o resultado
    result.Should().NotBeNull();
    result.Status.Should().Be("0");
}
```

### Uso de Theory para Testes Parametrizados

```csharp
[Theory]
[InlineData("PETR4", "Buy", 100, 25.50)]
[InlineData("VALE3", "Sell", 500, 75.25)]
[InlineData("VIIA4", "Buy", 1000, 50.00)]
public void MultipleScenarios(string symbol, string side, int qty, decimal price)
{
    // Teste executado 3 vezes com diferentes parâmetros
}
```

## 🎯 Pontos Críticos Testados

### 1. Regras de Negócio ✅
- Validação de símbolos permitidos (PETR4, VALE3, VIIA4)
- Limites de quantidade (1 a 99999)
- Limites de preço (0.01 a 999.99)
- Precisão decimal (múltiplos de 0.01)
- Geração de IDs únicos

### 2. Comunicação FIX ✅
- Envio de mensagens NewOrderSingle
- Recebimento de ExecutionReport
- Mapeamento correto de campos FIX
- Conversão de tipos (Buy/Sell → Side)

### 3. Gerenciamento de Sessões ✅
- Estabelecimento de conexão FIX
- Timeout de conexão (10 segundos)
- Status de conectividade
- Sincronização de waiters

### 4. Concorrência ✅
- Múltiplas requisições simultâneas
- Thread-safety do MessageStore
- Múltiplos waiters aguardando sessão
- CancellationToken e timeout

### 5. Tratamento de Erros ✅
- Timeouts (conexão e resposta)
- Falhas de envio (SendToTarget)
- Validações de entrada
- Mensagens malformadas
- Exceções customizadas (OrderRejectedException)

## 📈 Métricas e Qualidade

### Cobertura por Camada

| Camada          | Testes | Classes | Métodos |
|-----------------|--------|---------|---------|
| Domain          | 11     | 1       | ~5      |
| Application     | 13     | 2       | ~8      |
| Infrastructure  | 31     | 4       | ~15     |
| **Total**       | **55** | **7**   | **28**  |

### Tempo de Execução

- **Mais Rápido**: < 1ms (testes síncronos simples)
- **Médio**: 2-10ms (testes com mocks)
- **Mais Lento**: ~10s (testes de timeout assíncronos)
- **Total**: ~11 segundos

### Qualidade do Código de Teste

✅ **Práticas Aplicadas**:
- Isolamento de testes (cada teste é independente)
- Uso de mocks para dependências externas
- Testes determinísticos (não dependem de ordem)
- Nomes descritivos e autoexplicativos
- Cobertura de casos de sucesso E erro
- Testes de valores limites (boundary testing)

## 🐛 Troubleshooting

### ❌ Erro: "Type or namespace could not be found"

**Solução**: Restaurar pacotes NuGet
```bash
dotnet restore
```

### ❌ Erro: "The type initializer for 'QuickFix.Session' threw an exception"

**Causa**: QuickFIX precisa de configuração de sessão

**Solução**: Usar mocks do IFixClient em vez de instanciar Session diretamente

### ❌ Testes falhando aleatoriamente

**Causa**: Problemas de concorrência ou dependência de ordem

**Solução**: Garantir que cada teste seja independente e use dados únicos

### ❌ Testes lentos

**Solução**: Revisar timeouts e usar mocks adequados
```csharp
// ❌ Evitar delays desnecessários
await Task.Delay(5000);

// ✅ Usar timeouts curtos em testes
var timeout = TimeSpan.FromMilliseconds(100);
```

## 📚 Recursos e Documentação

### xUnit
- [Documentação Oficial](https://xunit.net/)
- [Comparação de Atributos](https://xunit.net/docs/comparisons)

### Moq
- [Documentação Oficial](https://github.com/moq/moq4)
- [Quickstart](https://github.com/moq/moq4/wiki/Quickstart)

### FluentAssertions
- [Documentação Oficial](https://fluentassertions.com/)
- [Tips and Tricks](https://fluentassertions.com/tips/)

### QuickFIX/n
- [GitHub Repository](https://github.com/connamara/quickfixn)
- [FIX Protocol](https://www.fixtrading.org/)

## 🔄 CI/CD Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

## 🤝 Contribuindo com Testes

### Ao adicionar novos testes:

1. ✅ Siga o padrão de nomenclatura
2. ✅ Use o padrão AAA (Arrange-Act-Assert)
3. ✅ Teste casos de sucesso E erro
4. ✅ Use `[Theory]` para cenários similares
5. ✅ Mantenha testes isolados e independentes
6. ✅ Use FluentAssertions para assertions
7. ✅ Documente cenários complexos com comentários

### Exemplo de novo teste:

```csharp
[Fact]
public async Task NovoTeste_DeveRetornarSucesso_QuandoDadosValidos()
{
    // Arrange
    var mock = new Mock<IDependencia>();
    mock.Setup(x => x.Metodo()).ReturnsAsync(expectedValue);
    var sut = new ClasseTestada(mock.Object);

    // Act
    var result = await sut.ExecutarAcao();

    // Assert
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
    mock.Verify(x => x.Metodo(), Times.Once);
}
```

## 📞 Suporte

Para questões sobre os testes:
1. Verifique o [README principal](../../README.md)
2. Consulte o [SUMMARY.md](SUMMARY.md) para visão geral
3. Revise a documentação das bibliotecas utilizadas

---

**Última atualização**: 2026  
**Versão**: 1.0.0  
**Mantido por**: Equipe de Desenvolvimento
