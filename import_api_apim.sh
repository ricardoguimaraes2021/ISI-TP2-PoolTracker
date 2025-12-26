#!/bin/bash

# Script para importar API no Azure API Management
# Aguarda o APIM ficar pronto e depois importa a API

set -e

APIM_NAME="pooltracker-apim-73479"
RESOURCE_GROUP="pooltracker-rg"
API_NAME="pooltracker-api"
SWAGGER_URL="https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json"
APP_SERVICE_URL="https://pooltracker-api-64853.azurewebsites.net"

echo "=== Aguardar APIM ficar pronto ==="
echo "APIM: $APIM_NAME"
echo ""

# Verificar estado
STATUS=$(az apim show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APIM_NAME" \
  --query "provisioningState" -o tsv 2>/dev/null || echo "Unknown")

echo "Estado atual: $STATUS"

if [ "$STATUS" != "Succeeded" ]; then
    echo "⏳ Aguardando APIM ficar pronto (pode demorar 10-15 minutos)..."
    echo "   (Pode cancelar e executar este script mais tarde)"
    echo ""
    
    # Aguardar até estar pronto (timeout de 30 minutos)
    az apim wait --created \
      --resource-group "$RESOURCE_GROUP" \
      --name "$APIM_NAME" \
      --timeout 1800 || {
        echo ""
        echo "⚠️  Timeout ou erro. Verificar manualmente:"
        echo "   az apim show --resource-group $RESOURCE_GROUP --name $APIM_NAME --query provisioningState -o tsv"
        exit 1
    }
    
    echo "✅ APIM pronto!"
else
    echo "✅ APIM já está pronto"
fi

echo ""
echo "=== Importar API ==="

# Verificar se API já existe
EXISTING_API=$(az apim api list \
  --resource-group "$RESOURCE_GROUP" \
  --service-name "$APIM_NAME" \
  --query "[?name=='$API_NAME'].name" -o tsv 2>/dev/null || echo "")

if [ -n "$EXISTING_API" ]; then
    echo "⚠️  API já existe. Atualizando..."
    az apim api import \
      --resource-group "$RESOURCE_GROUP" \
      --service-name "$APIM_NAME" \
      --api-id "$API_NAME" \
      --path "pooltracker" \
      --specification-format OpenApi \
      --specification-url "$SWAGGER_URL" \
      --display-name "PoolTracker API" \
      --service-url "$APP_SERVICE_URL"
    echo "✅ API atualizada"
else
    echo "📦 Importando API..."
    az apim api import \
      --resource-group "$RESOURCE_GROUP" \
      --service-name "$APIM_NAME" \
      --api-id "$API_NAME" \
      --path "pooltracker" \
      --specification-format OpenApi \
      --specification-url "$SWAGGER_URL" \
      --display-name "PoolTracker API" \
      --service-url "$APP_SERVICE_URL"
    echo "✅ API importada"
fi

echo ""
echo "=== Obter Gateway URL ==="
GATEWAY_URL=$(az apim show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APIM_NAME" \
  --query "gatewayUrl" -o tsv)

echo "Gateway URL: $GATEWAY_URL"
echo "API Endpoint: ${GATEWAY_URL}pooltracker/api/pool/status"
echo ""
echo "✅ Configuração concluída!"
echo ""
echo "🧪 Testar:"
echo "  curl ${GATEWAY_URL}pooltracker/api/pool/status"

