#!/bin/bash

# Script para adicionar domínios Vercel ao CORS do Azure App Service
# PoolTracker - TP2 ISI

set -e

RESOURCE_GROUP="pooltracker-rg"
APP_SERVICE_NAME="pooltracker-api-64853"

echo "=== Adicionar Domínios Vercel ao CORS ==="
echo ""

# Lista de domínios Vercel conhecidos
VERCEL_ORIGINS=(
  "https://pooltracker-web.vercel.app"
  "https://pooltracker-nincxtb33-crilly2001-gmailcoms-projects.vercel.app"
  "https://pooltracker-lwr7diekl-crilly2001-gmailcoms-projects.vercel.app"
)

echo "📋 Domínios a adicionar:"
for origin in "${VERCEL_ORIGINS[@]}"; do
  echo "  • $origin"
done
echo ""

# Verificar configuração atual
echo "🔍 Verificando configuração atual..."
CURRENT_ORIGINS=$(az webapp cors show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APP_SERVICE_NAME" \
  --query "allowedOrigins" -o tsv 2>/dev/null || echo "")

echo "Origens atuais: $CURRENT_ORIGINS"
echo ""

# Adicionar cada domínio
echo "📦 Adicionando domínios..."
for origin in "${VERCEL_ORIGINS[@]}"; do
  echo "  Adicionando: $origin"
  az webapp cors add \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APP_SERVICE_NAME" \
    --allowed-origins "$origin" 2>&1 | grep -v "dump_bash_state" || true
done

echo ""
echo "✅ Verificando configuração final..."
az webapp cors show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APP_SERVICE_NAME" \
  --output json

echo ""
echo "🔄 Reiniciando App Service para aplicar mudanças..."
az webapp restart \
  --resource-group "$RESOURCE_GROUP" \
  --name "$APP_SERVICE_NAME"

echo ""
echo "✅ CORS configurado!"
echo ""
echo "🧪 Testar:"
echo "  curl -H 'Origin: https://pooltracker-lwr7diekl-crilly2001-gmailcoms-projects.vercel.app' \\"
echo "       -H 'Access-Control-Request-Method: GET' \\"
echo "       -X OPTIONS \\"
echo "       https://pooltracker-api-64853.azurewebsites.net/api/pool/status -v"

