# 🚀 Instruções de Deploy na Vercel

## Passo 1: Login na Vercel

Execute no terminal (dentro da pasta `pooltracker-web`):

```bash
npx vercel login
```

Isso abrirá o browser para fazer login. Use sua conta GitHub, Google ou email.

## Passo 2: Deploy

Após o login, execute:

```bash
npx vercel --prod
```

**Responda às perguntas:**
- **Set up and deploy "~/pooltracker-web"?** → `Y` (Yes)
- **Which scope?** → Selecione seu username/team
- **Link to existing project?** → `N` (No - primeira vez)
- **What's your project's name?** → `pooltracker-web` (ou Enter para padrão)
- **In which directory is your code located?** → `./` (Enter)

Aguarde o build e deploy...

## Passo 3: Configurar Variável de Ambiente

Após o deploy, você receberá uma URL como: `https://pooltracker-web-xxx.vercel.app`

Agora configure a variável de ambiente:

```bash
npx vercel env add VITE_API_URL production
```

Quando pedir o valor, digite:
```
https://pooltracker-api-64853.azurewebsites.net
```

Depois, faça redeploy:

```bash
npx vercel --prod
```

## Passo 4: Configurar CORS no Azure

Execute o script criado (na raiz do projeto):

```bash
cd ..
./config_cors_vercel.sh
```

Ou manualmente:

```bash
az webapp cors add \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --allowed-origins "https://pooltracker-web-xxx.vercel.app"
```

**Substitua `xxx` pela URL real fornecida pela Vercel.**

## Passo 5: Atualizar CORS no appsettings (Opcional)

Se quiser adicionar também no código, atualize `appsettings.Production.json`:

```json
"Cors": {
  "AllowedOrigins": [
    "https://pooltracker-web-xxx.vercel.app"
  ]
}
```

E atualize no Azure:

```bash
az webapp config appsettings set \
  --resource-group pooltracker-rg \
  --name pooltracker-api-64853 \
  --settings Cors__AllowedOrigins__0="https://pooltracker-web-xxx.vercel.app"
```

## Passo 6: Testar

1. Aceda à URL da Vercel: `https://pooltracker-web-xxx.vercel.app`
2. Verifique se a página pública carrega dados do backend
3. Teste o login admin (PIN: 1234)
4. Verifique se todas as funcionalidades funcionam

## Troubleshooting

### Erro CORS
- Verifique se a URL do frontend está correta no CORS do Azure
- Verifique se a variável `VITE_API_URL` está configurada na Vercel

### Erro 404
- Verifique se o `vercel.json` está correto
- Verifique se o build foi bem-sucedido

### Erro de conexão com API
- Verifique se o backend Azure está online
- Verifique se a URL da API está correta na variável de ambiente

