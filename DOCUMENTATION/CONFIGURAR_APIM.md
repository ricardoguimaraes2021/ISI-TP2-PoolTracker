# 🔧 Configurar Azure API Management (APIM) - PoolTracker

Este guia explica como configurar o Azure API Management para gerir a API PoolTracker.

---

## 📋 Pré-requisitos

1. ✅ Azure CLI instalado e configurado
2. ✅ Login no Azure (`az login`)
3. ✅ Resource Group `pooltracker-rg` criado
4. ✅ App Service `pooltracker-api-64853` deployado e funcional
5. ✅ Swagger/OpenAPI disponível em: `https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json`

---

## 🚀 Passo 1: Verificar/Criar Azure API Management

### 1.1 Verificar se já existe um APIM

```bash
az apim list --resource-group pooltracker-rg --output table
```

### 1.2 Se não existir, criar um APIM (Developer tier - gratuito)

```bash
# Variáveis
RESOURCE_GROUP="pooltracker-rg"
APIM_NAME="pooltracker-apim-$(date +%s | tail -c 6)"
LOCATION="spaincentral"

# Criar APIM
az apim create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APIM_NAME" \
  --location "$LOCATION" \
  --publisher-name "Ricardo Guimarães" \
  --publisher-email "a20469@alunos.ipca.pt" \
  --sku-name Developer \
  --sku-capacity 1 \
  --enable-managed-identity
```

**Nota**: A criação do APIM pode demorar 10-15 minutos. O tier **Developer** é gratuito mas tem limitações (apenas 1 unidade, não pode escalar).

### 1.3 Verificar estado do APIM

```bash
az apim show \
  --resource-group pooltracker-rg \
  --name pooltracker-apim-73479 \
  --query "{name:name, gatewayUrl:gatewayUrl, status:provisioningState}" \
  -o table
```

Aguardar até `provisioningState` ser `Succeeded`.

---

## 📥 Passo 2: Importar API Definition (OpenAPI/Swagger)

### 2.1 Obter Swagger JSON

```bash
curl -s "https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json" -o swagger.json
```

### 2.2 Importar API no APIM

```bash
APIM_NAME="pooltracker-apim-73479"  # Substituir pelo nome real
API_NAME="pooltracker-api"
API_PATH="pooltracker"
APP_SERVICE_URL="https://pooltracker-api-64853.azurewebsites.net"
SWAGGER_URL="${APP_SERVICE_URL}/swagger/v1/swagger.json"

az apim api import \
  --resource-group pooltracker-rg \
  --service-name "$APIM_NAME" \
  --api-id "$API_NAME" \
  --path "$API_PATH" \
  --specification-format OpenApi \
  --specification-url "$SWAGGER_URL" \
  --display-name "PoolTracker API" \
  --service-url "$APP_SERVICE_URL"
```

**Parâmetros**:
- `--api-id`: Identificador único da API no APIM
- `--path`: Path base da API (ex: `pooltracker` → `/pooltracker/api/...`)
- `--specification-format`: Formato da especificação (OpenApi, Swagger, Wsdl, etc.)
- `--specification-url`: URL do Swagger JSON
- `--display-name`: Nome amigável da API
- `--service-url`: URL do backend (App Service)

---

## 🔧 Passo 3: Configurar Backend

### 3.1 Criar Backend

```bash
APIM_NAME="pooltracker-apim-73479"
BACKEND_NAME="pooltracker-backend"
APP_SERVICE_URL="https://pooltracker-api-64853.azurewebsites.net"

az apim backend create \
  --resource-group pooltracker-rg \
  --service-name "$APIM_NAME" \
  --backend-id "$BACKEND_NAME" \
  --url "$APP_SERVICE_URL" \
  --protocol http
```

### 3.2 Associar Backend à API

```bash
APIM_NAME="pooltracker-apim-73479"
API_NAME="pooltracker-api"
BACKEND_NAME="pooltracker-backend"

az apim api update \
  --resource-group pooltracker-rg \
  --service-name "$APIM_NAME" \
  --api-id "$API_NAME" \
  --service-url "$APP_SERVICE_URL"
```

---

## 🧪 Passo 4: Testar API através do APIM

### 4.1 Obter Gateway URL

```bash
APIM_NAME="pooltracker-apim-73479"

GATEWAY_URL=$(az apim show \
  --resource-group pooltracker-rg \
  --name "$APIM_NAME" \
  --query "gatewayUrl" -o tsv)

echo "Gateway URL: $GATEWAY_URL"
```

### 4.2 Testar Endpoint

```bash
# Testar endpoint público
curl "${GATEWAY_URL}pooltracker/api/pool/status"

# Testar endpoint de meteorologia
curl "${GATEWAY_URL}pooltracker/api/weather/current"
```

---

## 🔐 Passo 5: Configurar Autenticação (Opcional)

### 5.1 Criar Subscription Key

Por padrão, o APIM requer uma subscription key. Pode criar uma subscription:

```bash
APIM_NAME="pooltracker-apim-73479"

# Criar subscription
az apim subscription create \
  --resource-group pooltracker-rg \
  --service-name "$APIM_NAME" \
  --subscription-id "pooltracker-subscription" \
  --display-name "PoolTracker Subscription" \
  --state active
```

### 5.2 Obter Subscription Key

```bash
SUBSCRIPTION_KEY=$(az apim subscription list \
  --resource-group pooltracker-rg \
  --service-name "$APIM_NAME" \
  --query "[0].primaryKey" -o tsv)

echo "Subscription Key: $SUBSCRIPTION_KEY"
```

### 5.3 Usar Subscription Key nas Requests

```bash
curl -H "Ocp-Apim-Subscription-Key: $SUBSCRIPTION_KEY" \
  "${GATEWAY_URL}pooltracker/api/pool/status"
```

---

## 📊 Passo 6: Configurar API Definition no App Service

### 6.1 Configurar OpenAPI Definition URL no App Service

```bash
az webapp config set \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --generic-configurations '{
    "openApiEnabled": true,
    "openApiUrl": "https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json"
  }'
```

**Nota**: Esta configuração pode não estar disponível via CLI. Pode ser necessário configurar manualmente no portal Azure.

### 6.2 Configurar via Portal Azure

1. Aceder ao [Azure Portal](https://portal.azure.com)
2. Navegar para: **Resource Groups** > **pooltracker-rg** > **pooltracker-api-64853**
3. No menu lateral, ir a **API** > **API Definition**
4. Selecionar **OpenAPI** como source
5. Inserir URL: `https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json`
6. Clicar em **Save**

---

## 🌐 URLs Importantes

Após a configuração, terá acesso a:

| Recurso | URL | Descrição |
|---------|-----|-----------|
| **APIM Gateway** | `https://{apim-name}.azure-api.net` | Gateway principal do APIM |
| **API Endpoint** | `https://{apim-name}.azure-api.net/pooltracker/api/...` | Endpoints da API através do APIM |
| **Developer Portal** | `https://{apim-name}.portal.azure-api.net` | Portal do desenvolvedor |
| **Swagger UI** | `https://pooltracker-api-64853.azurewebsites.net/swagger` | Swagger UI original |

---

## 🔍 Verificar Configuração

### Listar APIs no APIM

```bash
az apim api list \
  --resource-group pooltracker-rg \
  --service-name pooltracker-apim-73479 \
  --output table
```

### Ver detalhes da API

```bash
az apim api show \
  --resource-group pooltracker-rg \
  --service-name pooltracker-apim-73479 \
  --api-id pooltracker-api \
  --query "{name:displayName, path:path, serviceUrl:serviceUrl}" \
  -o table
```

### Listar Backends

```bash
az apim backend list \
  --resource-group pooltracker-rg \
  --service-name pooltracker-apim-73479 \
  --output table
```

---

## 🎯 Benefícios do Azure API Management

1. **Rate Limiting**: Controlar número de requests por segundo/minuto
2. **Caching**: Cache de respostas para melhor performance
3. **Transformação**: Modificar requests/responses
4. **Monitorização**: Analytics e logs detalhados
5. **Segurança**: Validação de requests, IP filtering
6. **Versionamento**: Gerir múltiplas versões da API
7. **Documentação**: Portal do desenvolvedor automático

---

## 💰 Custos

- **Developer Tier**: Gratuito (com limitações)
  - 1 unidade apenas
  - Não pode escalar
  - Adequado para desenvolvimento/testes

- **Basic/Standard/Premium**: Pagos
  - Escalabilidade
  - SLA garantido
  - Suporte avançado

---

## 📚 Referências

- [Azure API Management Documentation](https://docs.microsoft.com/azure/api-management/)
- [Importar API OpenAPI](https://docs.microsoft.com/azure/api-management/import-api-from-oas)
- [Configurar Backend](https://docs.microsoft.com/azure/api-management/api-management-howto-create-backends)

---

**Última Atualização**: 26 de Dezembro de 2025  
**Status**: ✅ Guia completo para configuração do APIM

