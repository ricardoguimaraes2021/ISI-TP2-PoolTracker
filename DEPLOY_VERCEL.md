# 🚀 Deploy do Frontend na Vercel

## Pré-requisitos

1. Conta na Vercel (gratuita): https://vercel.com/signup
2. GitHub conectado à Vercel (recomendado) ou Vercel CLI instalado
3. Backend Azure já deployado e funcional

## Método 1: Deploy via Vercel CLI (Recomendado)

### Passo 1: Login na Vercel

```bash
cd pooltracker-web
npx vercel login
```

### Passo 2: Configurar Variável de Ambiente

A variável de ambiente será configurada durante o deploy ou via dashboard da Vercel.

**Variável necessária:**
- `VITE_API_URL=https://pooltracker-api-64853.azurewebsites.net`

### Passo 3: Deploy

```bash
npx vercel --prod
```

Siga as instruções interativas:
- **Set up and deploy?** → Yes
- **Which scope?** → Seu username/team
- **Link to existing project?** → No (primeira vez)
- **Project name?** → pooltracker-web (ou deixar padrão)
- **Directory?** → ./
- **Override settings?** → No

### Passo 4: Adicionar Variável de Ambiente (se não foi adicionada)

Após o deploy, adicione a variável de ambiente:

```bash
npx vercel env add VITE_API_URL production
# Quando pedir o valor: https://pooltracker-api-64853.azurewebsites.net
```

Depois, faça redeploy:

```bash
npx vercel --prod
```

## Método 2: Deploy via GitHub (Mais Simples)

### Passo 1: Push do código para GitHub

```bash
git add .
git commit -m "Preparar deploy frontend Vercel"
git push origin main
```

### Passo 2: Conectar repositório na Vercel

1. Aceder a: https://vercel.com/new
2. **Import Git Repository** → Selecionar `ISI-TP2-PoolTracker`
3. **Root Directory** → `pooltracker-web`
4. **Framework Preset** → Vite (detectado automaticamente)
5. **Build Command** → `npm run build` (já configurado)
6. **Output Directory** → `dist` (já configurado)

### Passo 3: Configurar Variáveis de Ambiente

Na página de configuração do projeto:

1. **Environment Variables** → Adicionar:
   - **Name**: `VITE_API_URL`
   - **Value**: `https://pooltracker-api-64853.azurewebsites.net`
   - **Environment**: Production, Preview, Development (marcar todos)

2. **Deploy** → Clicar em "Deploy"

### Passo 4: Obter URL do Frontend

Após o deploy, a Vercel fornecerá uma URL como:
- `https://pooltracker-web-xxx.vercel.app`

## Passo 5: Configurar CORS no Backend Azure

Após obter a URL da Vercel, adicionar ao CORS do backend:

```bash
az webapp cors add \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --allowed-origins "https://pooltracker-web-xxx.vercel.app"
```

Ou via Azure Portal:
1. App Service → CORS
2. Adicionar origem: `https://pooltracker-web-xxx.vercel.app`
3. Salvar

## Passo 6: Atualizar CORS no código (se necessário)

Se o CORS estiver configurado no código, atualizar `Program.cs`:

```csharp
var allowedOrigins = new[]
{
    "http://localhost:5173",
    "https://pooltracker-web-xxx.vercel.app"
};
```

## Testes

1. Aceder à URL da Vercel
2. Verificar se a página pública carrega dados do backend
3. Testar login admin
4. Verificar se todas as funcionalidades funcionam

## URLs Finais

- **Frontend**: `https://pooltracker-web-xxx.vercel.app`
- **Backend API**: `https://pooltracker-api-64853.azurewebsites.net`
- **Swagger**: `https://pooltracker-api-64853.azurewebsites.net/swagger`

---

**Nota**: Substituir `xxx` pela URL real fornecida pela Vercel.

