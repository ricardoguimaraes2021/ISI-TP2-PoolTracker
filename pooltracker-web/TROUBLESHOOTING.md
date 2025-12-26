# 🔧 Troubleshooting - Frontend

## Problema: ERR_BLOCKED_BY_CLIENT ou localhost:5011

### Sintomas
- Erros no console: `Failed to load resource: net::ERR_BLOCKED_BY_CLIENT`
- Frontend tenta aceder a `localhost:5011` em vez da URL de produção
- Erros de conexão com a API

### Causas Possíveis

1. **Testando localmente** (mais comum)
   - Se estás a executar `npm run dev` localmente, o frontend vai usar `localhost:5011`
   - Isso é **normal** para desenvolvimento local
   - **Solução**: Testar na URL de produção: https://pooltracker-web.vercel.app

2. **Cache do browser**
   - O browser pode ter cacheado uma versão antiga do código
   - **Solução**: Limpar cache (Ctrl+Shift+R ou Cmd+Shift+R) ou modo incógnito

3. **Variável de ambiente não aplicada**
   - A variável `VITE_API_URL` precisa estar configurada na Vercel
   - O build precisa ser feito com a variável
   - **Solução**: Verificar se a variável está configurada e fazer redeploy

4. **Bloqueador de anúncios/extensão**
   - Algumas extensões do browser bloqueiam requests
   - **Solução**: Desativar extensões ou testar em modo incógnito

### Soluções

#### 1. Verificar se estás a testar em produção

**URL de Produção**: https://pooltracker-web.vercel.app

Se estás a testar localmente (`npm run dev`), o frontend vai usar `localhost:5011` por padrão. Isso é **correto** para desenvolvimento local.

#### 2. Limpar cache do browser

- **Chrome/Edge**: Ctrl+Shift+R (Windows/Linux) ou Cmd+Shift+R (Mac)
- **Firefox**: Ctrl+F5 (Windows/Linux) ou Cmd+Shift+R (Mac)
- Ou usar modo incógnito/privado

#### 3. Verificar variável de ambiente na Vercel

```bash
cd pooltracker-web
npx vercel env ls
```

Deve mostrar `VITE_API_URL` configurada para Production.

#### 4. Fazer redeploy (se necessário)

```bash
cd pooltracker-web
npx vercel --prod
```

#### 5. Verificar console do browser

1. Abrir DevTools (F12)
2. Ir ao tab "Console"
3. Verificar qual URL está a ser usada
4. Verificar erros de CORS ou conexão

#### 6. Testar conexão direta com a API

```bash
curl https://pooltracker-api-64853.azurewebsites.net/api/pool/status
```

Deve retornar JSON com o estado da piscina.

### Verificação Rápida

1. ✅ Estás a aceder a https://pooltracker-web.vercel.app (não localhost)?
2. ✅ Limpaste o cache do browser?
3. ✅ A variável `VITE_API_URL` está configurada na Vercel?
4. ✅ Fizeste redeploy após configurar a variável?

### Se o problema persistir

1. Verificar logs da Vercel:
   ```bash
   npx vercel logs https://pooltracker-web.vercel.app
   ```

2. Verificar se o backend está online:
   ```bash
   curl https://pooltracker-api-64853.azurewebsites.net/api/pool/status
   ```

3. Verificar CORS no Azure:
   ```bash
   az webapp cors show --resource-group pooltracker-rg --name pooltracker-api-64853
   ```

4. Testar em modo incógnito para descartar cache/extensões

---

**Nota**: Se estás a desenvolver localmente, é normal que o frontend use `localhost:5011`. Para testar em produção, sempre acede à URL da Vercel.

