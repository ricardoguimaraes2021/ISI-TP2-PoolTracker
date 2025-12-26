# 🚀 Guia de Deploy - PoolTracker na Azure

Este documento descreve o processo completo de deploy do PoolTracker na Microsoft Azure usando o plano gratuito para estudantes.

---

## 📋 Pré-requisitos

1. **Conta Azure for Students** ativa
   - Aceder a: https://azure.microsoft.com/free/students/
   - Ativar com email académico (@alunos.ipca.pt)

2. **Azure CLI instalado**
   ```bash
   curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
   ```

3. **Login no Azure**
   ```bash
   az login
   az account set --subscription "Azure for Students"
   ```

---

## 🏗️ Recursos Criados

### 1. Resource Group

```bash
az group create \
  --name pooltracker-rg \
  --location spaincentral
```

**Nota**: A região `spaincentral` foi escolhida por:
- Proximidade geográfica a Portugal
- Suportada pelo plano Azure for Students
- Baixa latência

### 2. Azure SQL Server

```bash
az sql server create \
  --resource-group pooltracker-rg \
  --name pooltracker-sql-65033 \
  --location spaincentral \
  --admin-user pooltracker_admin \
  --admin-password "P00lTr@ck3r2025!Az#XyZ"
```

**Características**:
- **Tier**: Free (gratuito para estudantes)
- **Região**: Spain Central
- **Admin User**: `pooltracker_admin`

### 3. Azure SQL Database

```bash
az sql db create \
  --resource-group pooltracker-rg \
  --server pooltracker-sql-65033 \
  --name pooltracker \
  --service-objective Free
```

**Limitações do Free Tier**:
- 32 MB de espaço
- 5 DTU (Database Transaction Units)
- Adequado para desenvolvimento e testes

### 4. Firewall Rules

```bash
# Permitir serviços Azure
az sql server firewall-rule create \
  --resource-group pooltracker-rg \
  --server pooltracker-sql-65033 \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### 5. App Service Plan

```bash
az appservice plan create \
  --name pooltracker-plan \
  --resource-group pooltracker-rg \
  --location spaincentral \
  --sku FREE \
  --is-linux
```

**Características**:
- **Tier**: FREE (F1)
- **OS**: Linux
- **Região**: Spain Central

### 6. App Service (Web App)

```bash
az webapp create \
  --resource-group pooltracker-rg \
  --plan pooltracker-plan \
  --name pooltracker-api-64853 \
  --runtime "DOTNETCORE:8.0"
```

**Configuração**:
- **Runtime**: .NET Core 8.0
- **OS**: Linux
- **URL**: `https://pooltracker-api-64853.azurewebsites.net`

---

## ⚙️ Configuração

### 1. Connection String

```bash
CONNECTION_STRING="Server=tcp:pooltracker-sql-65033.database.windows.net,1433;Initial Catalog=pooltracker;Persist Security Info=False;User ID=pooltracker_admin;Password=P00lTr@ck3r2025!Az#XyZ;MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config appsettings set \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --settings "ConnectionStrings__DefaultConnection=$CONNECTION_STRING"
```

### 2. Variáveis de Ambiente

```bash
az webapp config appsettings set \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --settings \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "Jwt__Key=YourSuperSecretKeyThatIsAtLeast32CharactersLong!" \
    "Jwt__Issuer=PoolTrackerAPI" \
    "Jwt__Audience=PoolTrackerClients" \
    "Jwt__ExpiryMinutes=60"
```

---

## 📦 Deploy da Aplicação

### 1. Build da Aplicação

```bash
cd PoolTracker.API
dotnet publish -c Release -o ./publish
```

### 2. Criar ZIP

```bash
cd publish
zip -r ../api-deploy.zip .
cd ..
```

### 3. Deploy para Azure

```bash
az webapp deploy \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --src-path api-deploy.zip \
  --type zip
```

**Alternativa (método antigo)**:
```bash
az webapp deployment source config-zip \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --src api-deploy.zip
```

---

## ✅ Verificação

### 1. Verificar Status

```bash
az webapp show \
  --name pooltracker-api-64853 \
  --resource-group pooltracker-rg \
  --query "{state:state, defaultHostName:defaultHostName}" \
  --output table
```

### 2. Testar API

```bash
# Status da piscina
curl https://pooltracker-api-64853.azurewebsites.net/api/pool/status

# Meteorologia
curl https://pooltracker-api-64853.azurewebsites.net/api/weather/current

# Swagger
curl https://pooltracker-api-64853.azurewebsites.net/swagger
```

### 3. Verificar Logs

```bash
# Download logs
az webapp log download \
  --name pooltracker-api-64853 \
  --resource-group pooltracker-rg \
  --log-file app_logs.zip

# Stream logs (tempo real)
az webapp log tail \
  --name pooltracker-api-64853 \
  --resource-group pooltracker-rg
```

---

## 🔧 Troubleshooting

### Problema: "Login failed for user"

**Causa**: Password incorreta na connection string.

**Solução**:
1. Verificar password do SQL Server:
   ```bash
   az sql server show --name pooltracker-sql-65033 --resource-group pooltracker-rg
   ```

2. Redefinir password se necessário:
   ```bash
   az sql server update \
     --name pooltracker-sql-65033 \
     --resource-group pooltracker-rg \
     --admin-password "NovaPassword123!"
   ```

3. Atualizar connection string no App Service

### Problema: "Application Error" (503)

**Causas possíveis**:
1. Erro na inicialização da aplicação
2. Connection string incorreta
3. Firewall bloqueando conexão

**Solução**:
1. Verificar logs:
   ```bash
   az webapp log download --name pooltracker-api-64853 --resource-group pooltracker-rg --log-file logs.zip
   unzip -p logs.zip LogFiles/Application/*.log | tail -50
   ```

2. Verificar firewall rules:
   ```bash
   az sql server firewall-rule list \
     --resource-group pooltracker-rg \
     --server pooltracker-sql-65033
   ```

3. Reiniciar App Service:
   ```bash
   az webapp restart --name pooltracker-api-64853 --resource-group pooltracker-rg
   ```

### Problema: "Runtime status: Issues Detected"

**Causa**: Aplicação não está a iniciar corretamente.

**Solução**:
1. Verificar logs do Docker:
   ```bash
   az webapp log download --name pooltracker-api-64853 --resource-group pooltracker-rg --log-file logs.zip
   unzip -p logs.zip LogFiles/2025_12_26_*_docker.log | grep -i error
   ```

2. Verificar se o código foi publicado corretamente
3. Verificar variáveis de ambiente

---

## 📊 URLs de Produção

| Serviço | URL |
|---------|-----|
| **API Base** | https://pooltracker-api-64853.azurewebsites.net |
| **Swagger UI** | https://pooltracker-api-64853.azurewebsites.net/swagger |
| **Status** | https://pooltracker-api-64853.azurewebsites.net/api/pool/status |
| **Workers** | https://pooltracker-api-64853.azurewebsites.net/api/workers |
| **Weather** | https://pooltracker-api-64853.azurewebsites.net/api/weather/current |

---

## 💰 Custos

Com o plano **Azure for Students**:
- ✅ **App Service Plan (F1)**: Gratuito
- ✅ **Azure SQL Database (Free)**: Gratuito
- ✅ **SQL Server**: Gratuito (com Free tier)
- ✅ **Data Transfer**: 5 GB/mês gratuitos

**Total**: €0.00/mês (dentro dos limites do plano estudante)

---

## 🔄 Atualizações Futuras

### Deploy Contínuo (CI/CD)

1. **GitHub Actions**:
   - Automatizar build e deploy
   - Deploy automático em push para `main`

2. **Azure DevOps**:
   - Pipeline completo
   - Testes automatizados antes do deploy

### Melhorias

1. **Health Checks**: Configurar health check path no App Service
2. **Application Insights**: Habilitar monitorização
3. **Custom Domain**: Adicionar domínio personalizado
4. **SSL Certificate**: Configurar HTTPS obrigatório
5. **Scaling**: Upgrade para tier pago se necessário

---

## 📚 Referências

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/azure/sql-database/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/)
- [Azure for Students](https://azure.microsoft.com/free/students/)

---

**Última Atualização**: 26 de Dezembro de 2025  
**Status**: ✅ Todos os serviços deployados e funcionais em produção

## ✅ Status Final do Deploy

**Todos os serviços estão funcionais em produção:**

- ✅ **API RESTful**: HTTP 200 em todos os endpoints testados
- ✅ **Serviços SOAP**: 4 serviços com WSDL acessível (HTTP 200)
- ✅ **Swagger UI**: Acessível e funcional
- ✅ **Base de Dados**: Azure SQL Database conectada e funcional
- ✅ **Frontend**: Deployado na Vercel - https://pooltracker-web.vercel.app
- ✅ **Startup Command**: Configurado para `dotnet PoolTracker.API.dll`
- ✅ **CORS**: Configurado com suporte a credentials para frontend Vercel
- ✅ **Correções Aplicadas**: 
  - Erro LINQ (GroupBy com ToString) corrigido
  - Race condition em daily_visitors corrigida
  - Parsing de enums (PoolType, CleaningType) corrigido

**URLs de Produção:**
- **Frontend**: https://pooltracker-web.vercel.app
- **API Base**: https://pooltracker-api-64853.azurewebsites.net
- **Swagger**: https://pooltracker-api-64853.azurewebsites.net/swagger
- **SOAP Services**: https://pooltracker-api-64853.azurewebsites.net/soap/*

**Problemas Resolvidos**:
1. Conflito de múltiplos `.runtimeconfig.json` → Resolvido com startup command explícito
2. Erro LINQ em GetActiveWorkersCountAsync → Resolvido fazendo GroupBy em memória
3. Race condition em IncrementDailyVisitorsAsync → Resolvido com tratamento de exceção

