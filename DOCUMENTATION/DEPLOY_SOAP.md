# 🚀 Deploy dos Serviços SOAP - PoolTracker

Este guia explica como fazer o deployment dos serviços SOAP na Azure.

---

## 📋 Opções de Deploy

Existem **duas opções** para fazer deploy dos serviços SOAP:

### Opção 1: Mesmo App Service (Recomendado)
- ✅ Mais simples
- ✅ Menor custo (um único App Service)
- ✅ Partilha a mesma base de dados
- ✅ Mesma connection string

### Opção 2: App Service Separado
- ✅ Isolamento completo
- ✅ Escalabilidade independente
- ❌ Custo adicional (dois App Services)
- ❌ Mais complexo de gerir

**Recomendação**: Usar a **Opção 1** (mesmo App Service) para o plano gratuito.

---

## 🔧 Opção 1: Deploy no Mesmo App Service

### Passo 1: Integrar SOAP na API REST

Os serviços SOAP podem ser adicionados ao mesmo projeto `PoolTracker.API` ou manter-se separados. Vamos adicionar ao mesmo App Service:

#### 1.1 Adicionar Referência ao Projeto SOAP

```bash
cd PoolTracker.API
dotnet add reference ../PoolTracker.SOAP/PoolTracker.SOAP.csproj
```

#### 1.2 Atualizar Program.cs da API

Adicionar os endpoints SOAP ao `PoolTracker.API/Program.cs`:

```csharp
using SoapCore;
using PoolTracker.SOAP.Contracts;
using PoolTracker.SOAP.Services;

// ... código existente ...

// SOAP Services
builder.Services.AddScoped<IPoolDataService, PoolDataService>();
builder.Services.AddScoped<IWorkerDataService, WorkerDataService>();
builder.Services.AddScoped<IWaterQualityDataService, WaterQualityDataService>();
builder.Services.AddScoped<IReportDataService, ReportDataService>();

var app = builder.Build();

// ... middleware existente ...

// Configure SOAP endpoints (depois do MapControllers)
app.UseSoapEndpoint<IPoolDataService>("/soap/PoolDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
app.UseSoapEndpoint<IWorkerDataService>("/soap/WorkerDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
app.UseSoapEndpoint<IWaterQualityDataService>("/soap/WaterQualityDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
app.UseSoapEndpoint<IReportDataService>("/soap/ReportDataService", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

app.Run();
```

#### 1.3 Adicionar Package SoapCore

```bash
cd PoolTracker.API
dotnet add package SoapCore
```

#### 1.4 Build e Teste Local

```bash
dotnet build
dotnet run
```

Testar:
- REST: http://localhost:5011/api/pool/status
- SOAP: http://localhost:5011/soap/PoolDataService?wsdl

### Passo 2: Deploy para Azure

```bash
# Build
cd PoolTracker.API
dotnet publish -c Release -o ./publish

# Criar ZIP
cd publish
zip -r ../api-with-soap.zip .
cd ..

# Deploy
az webapp deploy \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --src-path api-with-soap.zip \
  --type zip
```

### Passo 3: Verificar

```bash
# Testar WSDL
curl "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl"

# Testar método SOAP
curl -X POST "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService" \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: \"http://tempuri.org/IPoolDataService/GetPoolStatus\"" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetPoolStatus xmlns="http://tempuri.org/">
    </GetPoolStatus>
  </soap:Body>
</soap:Envelope>'
```

---

## 🔧 Opção 2: App Service Separado

### Passo 1: Criar Novo App Service

```bash
# Criar App Service para SOAP
az webapp create \
  --resource-group pooltracker-rg \
  --plan pooltracker-plan \
  --name pooltracker-soap-64853 \
  --runtime "DOTNETCORE:8.0"
```

### Passo 2: Configurar Connection String

```bash
CONNECTION_STRING="Server=tcp:pooltracker-sql-65033.database.windows.net,1433;Initial Catalog=pooltracker;Persist Security Info=False;User ID=pooltracker_admin;Password=P00lTr@ck3r2025!Az#XyZ;MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config appsettings set \
  --resource-group pooltracker-rg \
  --name pooltracker-soap-64853 \
  --settings "ConnectionStrings__DefaultConnection=$CONNECTION_STRING"
```

### Passo 3: Build e Deploy

```bash
# Build do projeto SOAP
cd PoolTracker.SOAP
dotnet publish -c Release -o ./publish

# Criar ZIP
cd publish
zip -r ../soap-deploy.zip .
cd ..

# Deploy
az webapp deploy \
  --resource-group pooltracker-rg \
  --name pooltracker-soap-64853 \
  --src-path soap-deploy.zip \
  --type zip
```

### Passo 4: Verificar

```bash
# Testar WSDL
curl "https://pooltracker-soap-64853.azurewebsites.net/soap/PoolDataService?wsdl"
```

---

## 🧪 Testar Serviços SOAP em Produção

### Método 1: SoapUI

1. Abrir SoapUI
2. New SOAP Project
3. WSDL: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
4. Testar métodos

### Método 2: Postman

1. Import → URL
2. WSDL: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
3. Criar request SOAP

### Método 3: cURL

```bash
# Obter WSDL
curl "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl" > wsdl.xml

# Chamar GetPoolStatus
curl -X POST "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService" \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: \"http://tempuri.org/IPoolDataService/GetPoolStatus\"" \
  -d @soap_request.xml
```

Onde `soap_request.xml` contém:
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetPoolStatus xmlns="http://tempuri.org/">
    </GetPoolStatus>
  </soap:Body>
</soap:Envelope>
```

---

## 📊 URLs de Produção (Após Deploy)

### Opção 1 (Mesmo App Service)

| Serviço | URL |
|---------|-----|
| **PoolDataService** | https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService |
| **WorkerDataService** | https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService |
| **WaterQualityDataService** | https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService |
| **ReportDataService** | https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService |

**WSDL**: Adicionar `?wsdl` ao final de cada URL

### Opção 2 (App Service Separado)

| Serviço | URL |
|---------|-----|
| **PoolDataService** | https://pooltracker-soap-64853.azurewebsites.net/soap/PoolDataService |
| **WorkerDataService** | https://pooltracker-soap-64853.azurewebsites.net/soap/WorkerDataService |
| **WaterQualityDataService** | https://pooltracker-soap-64853.azurewebsites.net/soap/WaterQualityDataService |
| **ReportDataService** | https://pooltracker-soap-64853.azurewebsites.net/soap/ReportDataService |

---

## 🔧 Troubleshooting

### Problema: "404 Not Found" ao aceder ao WSDL

**Causa**: Endpoints SOAP não foram configurados corretamente.

**Solução**:
1. Verificar se `SoapCore` está instalado
2. Verificar se `UseSoapEndpoint` está no `Program.cs`
3. Verificar se o deploy incluiu o projeto SOAP

### Problema: "500 Internal Server Error"

**Causa**: Erro na inicialização ou connection string incorreta.

**Solução**:
1. Verificar logs:
   ```bash
   az webapp log download --name pooltracker-api-64853 --resource-group pooltracker-rg --log-file logs.zip
   unzip -p logs.zip LogFiles/Application/*.log | tail -50
   ```

2. Verificar connection string:
   ```bash
   az webapp config appsettings list \
     --name pooltracker-api-64853 \
     --resource-group pooltracker-rg \
     --query "[?name=='ConnectionStrings__DefaultConnection']"
   ```

### Problema: WSDL não é gerado

**Causa**: SoapCore pode não estar a gerar WSDL automaticamente.

**Solução**: Adicionar configuração explícita no `Program.cs`:
```csharp
app.UseSoapEndpoint<IPoolDataService>("/soap/PoolDataService", 
    new SoapEncoderOptions(), 
    SoapSerializer.XmlSerializer,
    false,  // caseInsensitive
    null,   // pathMatch
    true);  // generateWsdl
```

---

## ✅ Checklist de Deploy

- [ ] SoapCore package instalado
- [ ] Serviços SOAP registados no DI container
- [ ] Endpoints SOAP configurados no `Program.cs`
- [ ] Connection string configurada no Azure
- [ ] Build bem-sucedido
- [ ] Deploy concluído
- [ ] WSDL acessível (`?wsdl`)
- [ ] Métodos SOAP testados (SoapUI/Postman)

---

## 📚 Referências

- [SoapCore GitHub](https://github.com/DigDes/SoapCore)
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [SOAP Services Guide](./SOAP_SERVICES.md)

---

**Última Atualização**: 26 de Dezembro de 2025  
**Status**: ✅ Deployado e funcional

## ✅ Status Final

**Serviços SOAP integrados e funcionais no Azure:**

- ✅ Integração concluída no mesmo App Service
- ✅ SoapCore package instalado
- ✅ 4 serviços SOAP registados e configurados
- ✅ WSDL acessível para todos os serviços (HTTP 200)
- ✅ Chamadas SOAP funcionais (testado GetPoolStatus)

**URLs de Produção:**
- PoolDataService: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
- WorkerDataService: `https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService?wsdl`
- WaterQualityDataService: `https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService?wsdl`
- ReportDataService: `https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService?wsdl`

