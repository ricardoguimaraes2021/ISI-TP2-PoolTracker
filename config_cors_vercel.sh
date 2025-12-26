#!/bin/bash
# Script para configurar CORS no Azure após deploy do frontend na Vercel

echo "=== Configurar CORS no Azure App Service ==="
echo ""
echo "Insira a URL do frontend Vercel (ex: https://pooltracker-web-xxx.vercel.app):"
read VERCEL_URL

if [ -z "$VERCEL_URL" ]; then
    echo "❌ URL não fornecida. Abortando."
    exit 1
fi

echo ""
echo "Configurando CORS para: $VERCEL_URL"
echo ""

az webapp cors add \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --allowed-origins "$VERCEL_URL"

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ CORS configurado com sucesso!"
    echo ""
    echo "Verificar configuração:"
    az webapp cors show \
      --resource-group pooltracker-rg \
      --name pooltracker-api-64853
else
    echo ""
    echo "❌ Erro ao configurar CORS"
    exit 1
fi
