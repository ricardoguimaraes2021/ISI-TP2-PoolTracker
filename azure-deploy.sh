#!/bin/bash

# Script de Deploy Automático para Azure
# Execute: bash azure-deploy.sh

set -e

echo "🚀 Iniciando deploy do PoolTracker na Azure..."
echo ""

# Cores
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Verificar se Azure CLI está instalado
if ! command -v az &> /dev/null; then
    echo -e "${YELLOW}⚠️  Azure CLI não encontrado. Instalando...${NC}"
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
fi

# Verificar login
echo -e "${YELLOW}🔐 Verificando login na Azure...${NC}"
az account show > /dev/null 2>&1 || {
    echo "Fazendo login na Azure..."
    az login
}

# Definir variáveis
read -p "Nome do Resource Group (ex: pooltracker-rg): " RESOURCE_GROUP
read -p "Localização (ex: westeurope): " LOCATION
read -p "Nome do App Service (deve ser único): " APP_NAME
read -p "Nome do PostgreSQL Server (deve ser único): " SERVER_NAME
read -sp "Password do PostgreSQL (min 8 chars): " ADMIN_PASSWORD
echo ""

# Criar Resource Group
echo -e "${YELLOW}📦 Criando Resource Group...${NC}"
az group create --name $RESOURCE_GROUP --location $LOCATION

# Criar PostgreSQL (tentar Flexible Server primeiro)
echo -e "${YELLOW}🗄️  Criando PostgreSQL...${NC}"
az postgres flexible-server create \
  --resource-group $RESOURCE_GROUP \
  --name $SERVER_NAME \
  --location $LOCATION \
  --admin-user pooltracker_admin \
  --admin-password "$ADMIN_PASSWORD" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --version 16 \
  --storage-size 32 \
  --public-access 0.0.0.0 \
  || echo "⚠️  Flexible Server não disponível, tentando Single Server..."

# Criar base de dados
az postgres flexible-server db create \
  --resource-group $RESOURCE_GROUP \
  --server-name $SERVER_NAME \
  --database-name pooltracker \
  || az postgres db create \
    --resource-group $RESOURCE_GROUP \
    --server-name $SERVER_NAME \
    --database-name pooltracker

# Obter connection string
CONNECTION_STRING="Host=$SERVER_NAME.postgres.database.azure.com;Database=pooltracker;Username=pooltracker_admin;Password=$ADMIN_PASSWORD;Ssl Mode=Require;"

# Criar App Service Plan
echo -e "${YELLOW}🌐 Criando App Service...${NC}"
APP_SERVICE_PLAN="$RESOURCE_GROUP-plan"

az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku FREE \
  --is-linux

# Criar Web App
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $APP_NAME \
  --runtime "DOTNET|8.0"

# Configurar variáveis de ambiente
echo -e "${YELLOW}⚙️  Configurando variáveis de ambiente...${NC}"
JWT_KEY=$(openssl rand -base64 32)

az webapp config appsettings set \
  --resource-group $RESOURCE_GROUP \
  --name $APP_NAME \
  --settings \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__DefaultConnection=$CONNECTION_STRING" \
    "Jwt__Key=$JWT_KEY" \
    "Jwt__Issuer=PoolTrackerAPI" \
    "Jwt__Audience=PoolTrackerClients" \
    "Jwt__ExpiryMinutes=60"

# Build e deploy
echo -e "${YELLOW}📦 Fazendo build do projeto...${NC}"
cd PoolTracker.API
dotnet publish -c Release -o ./publish

echo -e "${YELLOW}🚀 Fazendo deploy...${NC}"
cd publish
zip -r ../deploy.zip . > /dev/null
cd ..

az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $APP_NAME \
  --src deploy.zip

# Obter URL
API_URL=$(az webapp show \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query defaultHostName \
  --output tsv)

echo ""
echo -e "${GREEN}✅ Deploy concluído!${NC}"
echo ""
echo "📍 URLs:"
echo "   API: https://$API_URL"
echo "   Swagger: https://$API_URL/swagger"
echo ""
echo "📝 Próximos passos:"
echo "   1. Testar API: curl https://$API_URL/api/pool/status"
echo "   2. Fazer deploy do frontend (ver GUIA_DEPLOY_AZURE.md)"
echo "   3. Atualizar CORS com URL do frontend"
echo ""

