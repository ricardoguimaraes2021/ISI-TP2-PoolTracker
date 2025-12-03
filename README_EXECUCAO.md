# 🚀 Guia Rápido de Execução Local

## Execução Rápida (2 Terminais)

### Terminal 1: Backend API
```bash
cd PoolTracker.API
dotnet restore
dotnet run
```
A API estará em: **http://localhost:5011** e **https://localhost:7011**

### Terminal 2: Frontend React
```bash
cd pooltracker-web
npm install
npm run dev
```
O frontend estará em: **http://localhost:5173**

---

## 📋 Pré-requisitos

- ✅ .NET 8.0 SDK (`dotnet --version`)
- ✅ Node.js 18+ (`node --version`)
- ✅ SQL Server (LocalDB, Express ou Docker)

---

## ⚙️ Configuração Inicial

### 1. Base de Dados

A base de dados é criada automaticamente na primeira execução.

**Se precisar de criar manualmente:**
```bash
cd PoolTracker.API
dotnet ef database update
```

### 2. Configuração da API

O ficheiro `appsettings.json` já está configurado com:
- Connection String: `Server=localhost;Database=PoolTrackerDB;...`
- JWT Key: Configurado
- Admin PIN: `1234`

### 3. Configuração do Frontend

O ficheiro `.env` já está criado com:
- API URL: `http://localhost:5011`
- Admin PIN: `1234`

**Se a API usar outra porta**, edite `pooltracker-web/.env`:
```env
VITE_API_URL=http://localhost:PORTA_AQUI
```

---

## 🧪 Testar o Sistema

### 1. Testar API (Swagger)
1. Abra: **https://localhost:7011/swagger**
2. Teste `GET /api/pool/status` (público)
3. Teste `POST /api/auth/login` com `{ "pin": "1234" }`
4. Use o token para testar endpoints protegidos

### 2. Testar Frontend
1. Abra: **http://localhost:5173**
2. Veja a página pública
3. Aceda: **http://localhost:5173/admin/login**
4. Login com PIN: `1234`
5. Explore o dashboard admin

---

## 🔧 Problemas Comuns

### Erro de Conexão à BD
```bash
# Verificar se SQL Server está a correr
# Windows: services.msc → procurar "SQL Server"
```

### Porta já em uso
Edite `PoolTracker.API/Properties/launchSettings.json` e mude as portas.

### CORS Error
Verifique se `VITE_API_URL` no `.env` corresponde à porta da API.

---

## ✅ Checklist

- [ ] API inicia sem erros
- [ ] Swagger acessível
- [ ] Frontend inicia sem erros
- [ ] Página pública carrega
- [ ] Login admin funciona (PIN: 1234)
- [ ] Dashboard mostra dados
- [ ] Entrada/saída funciona

---

**Ver `GUIA_EXECUCAO_LOCAL.md` para instruções detalhadas.**

