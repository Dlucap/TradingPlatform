# Guia de Troubleshooting - CORS e HTTP 405

## ✅ Problemas Corrigidos

### 1. Erro de CORS
**Sintoma:** `Access to fetch at 'https://localhost:7225/api/orders' from origin 'http://localhost:5173' has been blocked by CORS policy`

**Solução Implementada:**
- ✅ Adicionado middleware CORS no `Program.cs`
- ✅ Configurado policy `AllowFrontend` com origens permitidas
- ✅ Middleware CORS posicionado ANTES de `UseHttpsRedirection()`

### 2. Erro HTTP 405 (Method Not Allowed)
**Sintoma:** `405 Method Not Allowed` ao fazer POST para `/api/orders`

**Causas Possíveis:**
- Rota incorreta no controller
- Método HTTP não configurado corretamente
- Middleware na ordem errada

**Solução Implementada:**
- ✅ Atributo `[HttpPost]` no método `Create`
- ✅ Rota configurada como `[Route("api/[controller]")]`
- ✅ Ordem correta dos middlewares no pipeline

## 🔧 Configuração Atual

### Program.cs
```csharp
// CORS configurado com origens do appsettings.json
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Ordem correta dos middlewares
app.UseCors("AllowFrontend");      // PRIMEIRO
app.UseHttpsRedirection();          // SEGUNDO
app.UseAuthorization();             // TERCEIRO
app.MapControllers();               // QUARTO
```

### Origens Permitidas (appsettings.Development.json)
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",  // Vite dev server (padrão)
      "http://localhost:3000",  // React dev server alternativo
      "http://localhost:4173"   // Vite preview mode
    ]
  }
}
```

## 🧪 Como Testar

### 1. Verificar se a API está rodando
```powershell
cd OrderAccumulator.Api
dotnet run
```
Aguarde a mensagem: `Now listening on: https://localhost:7225`

### 2. Verificar se o Worker está rodando
```powershell
cd OrderAccumulator.Worker
dotnet run
```
Aguarde a conexão FIX ser estabelecida.

### 3. Testar com curl (sem CORS)
```powershell
curl -X POST https://localhost:7225/api/orders `
  -H "Content-Type: application/json" `
  -k `
  -d '{
    "symbol": "PETR4",
    "side": "Buy",
    "quantity": 100,
    "price": 28.50,
    "orderType": "Limit"
  }'
```

### 4. Testar do Frontend
```powershell
cd FrontEnd\order-generator-ui
npm run dev
```
Acesse `http://localhost:5173` e envie uma ordem.

## 🐛 Troubleshooting Adicional

### Erro: "Failed to fetch"
**Causa:** API não está rodando ou certificado SSL rejeitado.

**Solução:**
1. Verifique se a API está rodando na porta 7225
2. No navegador, acesse diretamente `https://localhost:7225/api/orders` e aceite o certificado
3. Ou use HTTP: mude `.env` para `VITE_API_URL=http://localhost:5287`

### Erro: "net::ERR_CONNECTION_REFUSED"
**Causa:** API não está rodando.

**Solução:** Inicie a API com `dotnet run` na pasta `OrderAccumulator.Api`

### Erro: CORS com credenciais
**Causa:** Frontend enviando cookies mas CORS não permite.

**Solução Atual:** Já configurado `.AllowCredentials()` no CORS policy.

### Adicionar nova origem permitida
Edite `appsettings.Development.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000",
      "http://localhost:4173",
      "http://sua-nova-origem:porta"  // Adicione aqui
    ]
  }
}
```

## 📝 Ordem Correta dos Middlewares

A ordem é **crítica** no ASP.NET Core pipeline:

1. ✅ `app.UseCors()` - PRIMEIRO (deve vir antes de qualquer outro middleware)
2. ✅ `app.UseHttpsRedirection()` - Redirecionamento HTTP → HTTPS
3. ✅ `app.UseRouting()` - Roteamento (implícito quando usa MapControllers)
4. ✅ `app.UseAuthorization()` - Autorização
5. ✅ `app.MapControllers()` - Mapeamento de controllers

**❌ ERRADO:**
```csharp
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");  // Tarde demais!
```

**✅ CORRETO:**
```csharp
app.UseCors("AllowFrontend");  // PRIMEIRO!
app.UseHttpsRedirection();
```

## 🔍 Debug Headers CORS

Para debugar problemas de CORS, verifique os headers HTTP no navegador (F12 → Network):

**Request Headers (Frontend → API):**
```
Origin: http://localhost:5173
```

**Response Headers (API → Frontend):**
```
Access-Control-Allow-Origin: http://localhost:5173
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: *
Access-Control-Allow-Credentials: true
```

Se esses headers não aparecerem, o CORS não está configurado corretamente.

## 📚 Referências

- [ASP.NET Core CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [Middleware Order](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Vite CORS Issues](https://vitejs.dev/config/server-options.html#server-proxy)
