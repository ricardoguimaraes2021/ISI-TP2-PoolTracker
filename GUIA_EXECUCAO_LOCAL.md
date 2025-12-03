# Guia de Execução Local - PoolTracker

Este guia explica como executar o projeto PoolTracker localmente para testes antes do deploy em produção.

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **.NET 8.0 SDK** ou superior
  - Verificar: `dotnet --version`
  - Download: https://dotnet.microsoft.com/download
- **Node.js 18+** e npm
  - Verificar: `node --version` e `npm --version`
  - Download: https://nodejs.org/
- **SQL Server** (LocalDB, Express ou Developer Edition)
  - Ou **SQL Server Express** (gratuito)
  - Download: https://www.microsoft.com/sql-server/sql-server-downloads
- **Git** (opcional, para clonar o repositório)

---

## 🗄️ Passo 1: Configurar a Base de Dados

### Opção A: SQL Server LocalDB (Recomendado para desenvolvimento)

O LocalDB é incluído com o Visual Studio ou pode ser instalado separadamente.

1. Verificar se o LocalDB está instalado:
```bash
sqllocaldb info
```

2. Criar uma instância (se necessário):
```bash
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Opção B: SQL Server Express

1. Instalar SQL Server Express
2. Durante a instalação, escolha "Mixed Mode Authentication"
3. Anote o nome do servidor (geralmente `localhost` ou `localhost\SQLEXPRESS`)

### Opção C: Docker (Alternativa)

Se preferir usar Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## ⚙️ Passo 2: Configurar o Backend (API .NET)

### 2.1. Configurar Connection String

Edite o ficheiro `PoolTracker.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PoolTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity!",
    "Issuer": "PoolTrackerAPI",
    "Audience": "PoolTrackerClients",
    "ExpiryMinutes": 60
  },
  "AdminPin": "1234"
}
```

**Para SQL Server Express:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PoolTrackerDB;Trusted_Connection=true;TrustServerCertificate=true;"
```

**Para Docker:**
```json
"DefaultConnection": "Server=localhost,1433;Database=PoolTrackerDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
```

### 2.2. Restaurar Dependências

```bash
cd PoolTracker.API
dotnet restore
```

### 2.3. Criar a Base de Dados

A base de dados será criada automaticamente na primeira execução (devido ao `EnsureCreated()` no `Program.cs`).

**OU** pode criar manualmente usando Entity Framework:

```bash
# Instalar ferramentas EF Core (se ainda não tiver)
dotnet tool install --global dotnet-ef

# Criar migration (se necessário)
dotnet ef migrations add InitialCreate --project ../PoolTracker.Infrastructure --startup-project .

# Aplicar migrations
dotnet ef database update
```

### 2.4. Executar a API

```bash
cd PoolTracker.API
dotnet run
```

A API estará disponível em:
- **HTTP**: `http://localhost:5292`
- **HTTPS**: `https://localhost:7292`
- **Swagger**: `https://localhost:7292/swagger`

**Nota**: Se as portas estiverem ocupadas, o .NET escolherá automaticamente outras portas. Verifique a consola para ver as URLs exatas.

---

## 🎨 Passo 3: Configurar o Frontend (React)

### 3.1. Criar Ficheiro .env

Crie um ficheiro `.env` na pasta `pooltracker-web/`:

```bash
cd pooltracker-web
cp .env.example .env
```

Edite o `.env`:

```env
VITE_API_URL=http://localhost:5292
VITE_ADMIN_PIN=1234
```

**Importante**: Se a API estiver a correr numa porta diferente, atualize `VITE_API_URL` com a porta correta.

### 3.2. Instalar Dependências

```bash
cd pooltracker-web
npm install
```

### 3.3. Executar o Frontend

```bash
npm run dev
```

O frontend estará disponível em:
- **URL**: `http://localhost:5173`

---

## 🧪 Passo 4: Testar o Sistema

### 4.1. Testar a API

1. Abra o Swagger: `https://localhost:7292/swagger`
2. Teste o endpoint público:
   - `GET /api/pool/status` - Deve retornar o estado da piscina
3. Teste o login:
   - `POST /api/auth/login` com body: `{ "pin": "1234" }`
   - Copie o token retornado
4. Use o token para testar endpoints protegidos:
   - Clique em "Authorize" no Swagger
   - Cole o token no formato: `Bearer {seu-token}`
   - Teste `POST /api/pool/enter`

### 4.2. Testar o Frontend

1. Abra `http://localhost:5173` no navegador
2. **Página Pública**: Deve mostrar o estado da piscina, meteorologia e qualidade da água
3. **Login Admin**: Aceda a `http://localhost:5173/admin/login`
   - PIN: `1234`
4. **Dashboard Admin**: Após login, deve ver o dashboard com controlo de lotação
5. **Navegar pelas páginas**:
   - Trabalhadores
   - Qualidade da Água
   - Relatórios

### 4.3. Testar Funcionalidades Completas

1. **Registar Entrada/Saída**:
   - No dashboard admin, clique em "Entrou" ou "Saiu"
   - Verifique se o contador atualiza

2. **Gerir Trabalhadores**:
   - Vá para "Trabalhadores"
   - Crie um novo trabalhador
   - Ative um turno

3. **Registar Qualidade da Água**:
   - Vá para "Qualidade da Água"
   - Adicione uma nova medição
   - Verifique se aparece na lista

4. **Ver Relatórios**:
   - Vá para "Relatórios"
   - Verifique se os gráficos aparecem

---

## 🔧 Resolução de Problemas

### Problema: Erro de conexão à base de dados

**Solução**:
1. Verifique se o SQL Server está a correr:
   ```bash
   # Windows
   services.msc  # Procurar por "SQL Server"
   
   # Ou verificar via PowerShell
   Get-Service | Where-Object {$_.Name -like "*SQL*"}
   ```

2. Verifique a connection string no `appsettings.json`
3. Teste a conexão:
   ```bash
   sqlcmd -S localhost -E
   # Ou para SQL Express:
   sqlcmd -S localhost\SQLEXPRESS -E
   ```

### Problema: Porta já em uso

**Solução**:
1. Altere a porta no `launchSettings.json`:
   ```json
   "applicationUrl": "http://localhost:5000;https://localhost:5001"
   ```

2. Ou pare o processo que está a usar a porta:
   ```bash
   # Windows
   netstat -ano | findstr :5292
   taskkill /PID <PID> /F
   ```

### Problema: CORS Error no Frontend

**Solução**:
1. Verifique se `VITE_API_URL` no `.env` está correto
2. Verifique se a API está a correr
3. O CORS já está configurado para permitir todas as origens em desenvolvimento

### Problema: Token JWT Inválido

**Solução**:
1. Verifique se o PIN está correto (padrão: `1234`)
2. Verifique se o token não expirou (padrão: 60 minutos)
3. Faça logout e login novamente

### Problema: Base de Dados não criada

**Solução**:
1. Execute manualmente:
   ```bash
   cd PoolTracker.API
   dotnet ef database update
   ```

2. Ou delete a base de dados e deixe o sistema criar novamente:
   ```sql
   DROP DATABASE PoolTrackerDB;
   ```

---

## 📝 Checklist de Verificação

Antes de considerar que tudo está a funcionar:

- [ ] API inicia sem erros
- [ ] Swagger está acessível
- [ ] Base de dados foi criada
- [ ] Frontend inicia sem erros
- [ ] Página pública carrega dados
- [ ] Login admin funciona
- [ ] Dashboard admin mostra dados
- [ ] Entrada/saída funciona
- [ ] CRUD de trabalhadores funciona
- [ ] Registo de qualidade da água funciona
- [ ] Relatórios mostram dados e gráficos

---

## 🚀 Execução Rápida (Resumo)

```bash
# Terminal 1: Backend
cd PoolTracker.API
dotnet restore
dotnet run

# Terminal 2: Frontend
cd pooltracker-web
npm install
npm run dev
```

Aceda:
- **Frontend**: http://localhost:5173
- **API Swagger**: https://localhost:7292/swagger
- **Login Admin**: http://localhost:5173/admin/login (PIN: 1234)

---

## 📚 Próximos Passos

Após testar localmente:

1. **Executar Testes Automatizados**:
   ```bash
   cd PoolTracker.Tests
   dotnet test
   ```

2. **Verificar Code Coverage**:
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

3. **Preparar para Deploy**:
   - Ver guia de deploy na documentação
   - Configurar variáveis de ambiente de produção
   - Testar em ambiente de staging

---

**Boa sorte com os testes! 🎉**

