#!/bin/bash

# Script para configurar Azure API Management e importar API Definition
# PoolTracker - TP2 ISI

set -e

echo "=== Configurar Azure API Management ==="
echo ""

# Variáveis
RESOURCE_GROUP="pooltracker-rg"
APIM_NAME="pooltracker-apim-$(date +%s | tail -c 6)"
LOCATION="spaincentral"
APP_SERVICE_URL="https://pooltracker-api-64853.azurewebsites.net"
SWAGGER_URL="${APP_SERVICE_URL}/swagger/v1/swagger.json"
API_NAME="pooltracker-api"
API_DISPLAY_NAME="PoolTracker API"
API_PATH="pooltracker"

echo "📋 Configuração:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  APIM Name: $APIM_NAME"
echo "  Location: $LOCATION"
echo "  App Service URL: $APP_SERVICE_URL"
echo "  Swagger URL: $SWAGGER_URL"
echo ""

# Verificar se o resource group existe
echo "🔍 Verificando resource group..."
if ! az group show --name "$RESOURCE_GROUP" &>/dev/null; then
    echo "❌ Resource group não encontrado. Criando..."
    az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
else
    echo "✅ Resource group encontrado"
fi

# Verificar se já existe um APIM
echo ""
echo "🔍 Verificando se já existe um APIM..."
EXISTING_APIM=$(az apim list --resource-group "$RESOURCE_GROUP" --query "[0].name" -o tsv 2>/dev/null || echo "")

if [ -n "$EXISTING_APIM" ] && [ "$EXISTING_APIM" != "None" ]; then
    echo "✅ APIM existente encontrado: $EXISTING_APIM"
    APIM_NAME="$EXISTING_APIM"
    USE_EXISTING=true
else
    echo "📦 Criando novo Azure API Management (Developer tier - gratuito)..."
    USE_EXISTING=false
    
    # Criar APIM (Developer tier é gratuito)
    az apim create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$APIM_NAME" \
        --location "$LOCATION" \
        --publisher-name "Ricardo Guimarães" \
        --publisher-email "a20469@alunos.ipca.pt" \
        --sku-name Developer \
        --sku-capacity 1 \
        --enable-managed-identity
    
    echo "✅ APIM criado: $APIM_NAME"
    echo "⏳ Aguardando criação completa (pode demorar 10-15 minutos)..."
    
    # Aguardar até o APIM estar pronto
    az apim wait --created \
        --resource-group "$RESOURCE_GROUP" \
        --name "$APIM_NAME" \
        --timeout 1800
    
    echo "✅ APIM pronto!"
fi

# Obter Swagger JSON
echo ""
echo "📥 Obtendo definição OpenAPI..."
if [ ! -f "swagger.json" ]; then
    curl -s "$SWAGGER_URL" -o swagger.json
    echo "✅ Swagger JSON obtido"
else
    echo "✅ Swagger JSON já existe localmente"
fi

# Verificar se a API já existe
echo ""
echo "🔍 Verificando se a API já existe no APIM..."
EXISTING_API=$(az apim api list \
    --resource-group "$RESOURCE_GROUP" \
    --service-name "$APIM_NAME" \
    --query "[?name=='$API_NAME'].name" -o tsv 2>/dev/null || echo "")

if [ -n "$EXISTING_API" ]; then
    echo "⚠️  API já existe. Atualizando..."
    # Atualizar API existente
    az apim api import \
        --resource-group "$RESOURCE_GROUP" \
        --service-name "$APIM_NAME" \
        --api-id "$API_NAME" \
        --path "$API_PATH" \
        --specification-format OpenApi \
        --specification-url "$SWAGGER_URL" \
        --display-name "$API_DISPLAY_NAME" \
        --service-url "$APP_SERVICE_URL"
    
    echo "✅ API atualizada"
else
    echo "📦 Importando API no APIM..."
    # Importar API
    az apim api import \
        --resource-group "$RESOURCE_GROUP" \
        --service-name "$APIM_NAME" \
        --api-id "$API_NAME" \
        --path "$API_PATH" \
        --specification-format OpenApi \
        --specification-url "$SWAGGER_URL" \
        --display-name "$API_DISPLAY_NAME" \
        --service-url "$APP_SERVICE_URL"
    
    echo "✅ API importada"
fi

# Configurar backend
echo ""
echo "🔧 Configurando backend..."
BACKEND_NAME="pooltracker-backend"

# Verificar se backend já existe
EXISTING_BACKEND=$(az apim backend list \
    --resource-group "$RESOURCE_GROUP" \
    --service-name "$APIM_NAME" \
    --query "[?name=='$BACKEND_NAME'].name" -o tsv 2>/dev/null || echo "")

if [ -z "$EXISTING_BACKEND" ]; then
    echo "📦 Criando backend..."
    az apim backend create \
        --resource-group "$RESOURCE_GROUP" \
        --service-name "$APIM_NAME" \
        --backend-id "$BACKEND_NAME" \
        --url "$APP_SERVICE_URL" \
        --protocol http
    
    echo "✅ Backend criado"
else
    echo "✅ Backend já existe"
fi

# Obter URL do APIM
echo ""
echo "🌐 Obtendo URL do APIM..."
APIM_URL=$(az apim show \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APIM_NAME" \
    --query "gatewayUrl" -o tsv)

echo ""
echo "=== ✅ CONFIGURAÇÃO CONCLUÍDA ==="
echo ""
echo "📊 Informações do APIM:"
echo "  Nome: $APIM_NAME"
echo "  Gateway URL: $APIM_URL"
echo "  API Path: $API_PATH"
echo "  API URL: ${APIM_URL}${API_PATH}"
echo ""
echo "🔗 URLs importantes:"
echo "  Portal do Desenvolvedor: https://$APIM_NAME.portal.azure-api.net"
echo "  Gateway: $APIM_URL"
echo "  API Endpoint: ${APIM_URL}${API_PATH}"
echo ""
echo "📝 Próximos passos:"
echo "  1. Aceder ao portal: https://portal.azure.com"
echo "  2. Navegar para: Resource Groups > $RESOURCE_GROUP > $APIM_NAME"
echo "  3. Configurar políticas se necessário"
echo "  4. Testar a API através do APIM Gateway"
echo ""
echo "🧪 Testar API:"
echo "  curl ${APIM_URL}${API_PATH}/api/pool/status"
echo ""

