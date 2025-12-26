# 🔧 Corrigir Variável de Ambiente VITE_API_URL

## Problema Identificado

A variável `VITE_API_URL` na Vercel tem uma **barra no final** da URL:
- ❌ `https://pooltracker-api-64853.azurewebsites.net/` (com barra)
- ✅ `https://pooltracker-api-64853.azurewebsites.net` (sem barra)

Isso pode causar problemas nas requisições.

## Solução

### Passo 1: Remover variável atual

```bash
cd pooltracker-web
npx vercel env rm VITE_API_URL production
```

Quando pedir confirmação, responder `y` (yes).

### Passo 2: Adicionar variável com URL correta

```bash
npx vercel env add VITE_API_URL production
```

Quando pedir o valor, digitar (sem barra no final):
```
https://pooltracker-api-64853.azurewebsites.net
```

### Passo 3: Adicionar para Preview e Development também

```bash
# Para Preview
npx vercel env add VITE_API_URL preview
# Valor: https://pooltracker-api-64853.azurewebsites.net

# Para Development
npx vercel env add VITE_API_URL development
# Valor: https://pooltracker-api-64853.azurewebsites.net
```

### Passo 4: Fazer redeploy

```bash
npx vercel --prod
```

Isso vai fazer rebuild com a variável corrigida.

### Passo 5: Limpar cache do browser

Após o redeploy:
1. Aceder a: https://pooltracker-web.vercel.app
2. Limpar cache: `Ctrl+Shift+R` (Windows/Linux) ou `Cmd+Shift+R` (Mac)
3. Ou testar em modo incógnito

## Verificação

Após corrigir, verificar se está correto:

```bash
npx vercel env ls
```

Deve mostrar `VITE_API_URL` para Production, Preview e Development, todas com a URL **sem barra no final**.

## Nota

A barra no final da URL pode causar problemas porque:
- O código faz `baseURL + endpoint` (ex: `baseURL + '/api/pool/status'`)
- Se `baseURL` já tem barra, fica: `https://...net//api/pool/status` (dupla barra)
- Isso pode causar erros de roteamento

