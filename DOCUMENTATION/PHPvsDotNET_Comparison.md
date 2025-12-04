# Comparação: PoolTracker_Online (PHP) vs PoolTracker (.NET)

## Resumo Executivo

O projeto **PoolTracker_Online** (PHP + React) é **significativamente mais completo** que a versão .NET, com **8 módulos funcionais** vs 2 módulos básicos. A versão PHP está **publicada online** (Hostinger) e demonstra funcionalidades avançadas que podem ser portadas para .NET para cumprir os requisitos do TP2.

**Recomendação**: Usar a **lógica de negócio e estrutura de dados** da versão PHP como blueprint para expandir a versão .NET.

---

## 📊 Comparação de Funcionalidades

### Funcionalidades Comuns (Ambas as Versões)

| Funcionalidade | .NET | PHP | Notas |
|----------------|------|-----|-------|
| **Gestão de Lotação** | ✅ | ✅ | Ambas implementam entrada/saída |
| **Estado Piscina (Abrir/Fechar)** | ✅ | ✅ | Lógica similar |
| **Integração Meteorologia** | ✅ | ✅ | Ambas usam Open-Meteo |
| **Painel Admin** | ✅ | ✅ | PHP muito mais completo |
| **Página Pública** | ✅ | ✅ | PHP com mais informação |
| **Autenticação Admin** | ✅ | ✅ | .NET usa API Key, PHP usa PIN |

### Funcionalidades EXCLUSIVAS da Versão PHP

| Funcionalidade | Complexidade | Valor para TP2 | Prioridade |
|----------------|--------------|----------------|------------|
| **Gestão de Trabalhadores** | 🟡 Média | ⭐⭐⭐ Alta | **ALTA** |
| **Sistema de Turnos** | 🟡 Média | ⭐⭐⭐ Alta | **ALTA** |
| **Qualidade da Água** | 🟢 Baixa | ⭐⭐⭐ Alta | **ALTA** |
| **Registo de Limpezas** | 🟢 Baixa | ⭐⭐ Média | MÉDIA |
| **Relatórios Diários** | 🔴 Alta | ⭐⭐⭐ Alta | **ALTA** |
| **Estatísticas/Gráficos** | 🟡 Média | ⭐⭐⭐ Alta | **ALTA** |
| **Lista de Compras** | 🟢 Baixa | ⭐ Baixa | BAIXA |
| **Histórico de Visitantes** | 🟡 Média | ⭐⭐ Média | MÉDIA |

---

## 🗄️ Comparação de Bases de Dados

### Versão .NET (Atual)
```
- Sem base de dados persistente
- Estado em memória (PoolService singleton)
- Perde dados ao reiniciar
```

### Versão PHP (PoolTracker_Online)
```sql
✅ pool_status          -- Estado da piscina
✅ daily_visitors       -- Visitantes diários
✅ workers              -- Cadastro de trabalhadores
✅ active_workers       -- Turnos ativos (manhã/tarde)
✅ cleanings            -- Limpezas (balneários/WC)
✅ water_quality        -- pH e temperatura (crianças/adultos)
✅ daily_reports        -- Relatórios automáticos
✅ shopping_list        -- Lista de compras
```

**8 tabelas** com relacionamentos, índices e foreign keys.

---

## 🏗️ Comparação de Arquitetura

### Backend

| Aspecto | .NET | PHP |
|---------|------|-----|
| **Framework** | ASP.NET Core 10 | PHP Puro (sem framework) |
| **Estrutura** | Controllers + Services | Controllers + Services + Models |
| **Persistência** | Memória (singleton) | MySQL via PDO |
| **API Endpoints** | 7 endpoints | ~30+ endpoints |
| **Autenticação** | Middleware customizado | Middleware customizado |
| **Documentação API** | OpenAPI nativo | Sem Swagger |

### Frontend

| Aspecto | .NET | PHP |
|---------|------|-----|
| **Framework** | React 18 + Vite | React 18 + Vite |
| **UI Library** | TailwindCSS | TailwindCSS + Shadcn/ui |
| **Componentes** | Básicos | Shadcn/ui (avançados) |
| **Gráficos** | ❌ Não | ✅ Recharts |
| **Páginas** | 2 (pública + admin) | 2 (pública + admin expandido) |
| **Notificações** | ❌ Não | ✅ React Hot Toast |

---

## 🎯 Funcionalidades Detalhadas da Versão PHP

### 1. Gestão de Trabalhadores
**Controllers**: `WorkerController.php`  
**Endpoints**:
- `GET /api/workers` - Listar todos
- `POST /api/workers` - Criar novo
- `PUT /api/workers/{id}` - Atualizar
- `DELETE /api/workers/{id}` - Eliminar
- `POST /api/workers/{id}/activate` - Ativar turno
- `POST /api/workers/{id}/deactivate` - Desativar turno
- `GET /api/workers/active` - Trabalhadores em turno

**Campos**:
- `worker_id` (único)
- `name`
- `role` (nadador_salvador, bar, vigilante, bilheteira)
- `is_active`
- `shift_type` (manhã/tarde)

**Lógica de Negócio**:
- Turnos manhã (9h-14h) e tarde (14h-19h)
- Auto-desativação quando piscina fecha
- Contagem de turnos para relatórios
- Filtros por cargo

### 2. Qualidade da Água
**Controllers**: `WaterQualityController.php`  
**Endpoints**:
- `GET /api/water-quality` - Histórico
- `GET /api/water-quality/latest` - Última medição
- `POST /api/water-quality` - Registar medição

**Campos**:
- `pool_type` (crianças/adultos)
- `ph_level` (decimal 4,2)
- `temperature` (decimal 5,2)
- `measured_at`
- `notes`

**Exibição**:
- Página pública mostra última medição de cada piscina
- Admin tem histórico completo com gráficos

### 3. Limpezas
**Controllers**: `CleaningController.php`  
**Endpoints**:
- `GET /api/cleanings` - Histórico
- `GET /api/cleanings/latest` - Última limpeza
- `POST /api/cleanings` - Registar limpeza

**Campos**:
- `cleaning_type` (balneários/WC)
- `cleaned_at`
- `notes`

### 4. Relatórios Diários
**Controllers**: `ReportController.php`  
**Endpoints**:
- `GET /api/reports` - Listar relatórios
- `GET /api/reports/latest` - Último relatório
- `POST /api/reports/generate` - Gerar relatório

**Dados Incluídos**:
- Total de visitantes
- Ocupação máxima
- Ocupação média
- Horário abertura/fecho
- Qualidade da água (JSON)
- Trabalhadores ativos (JSON)
- Limpezas realizadas (JSON)

### 5. Estatísticas e Gráficos
**Visualizações**:
- Gráfico de fluxo de visitantes (últimos 7 dias)
- Gráfico de turnos por trabalhador
- Histórico de qualidade da água
- Ocupação ao longo do tempo

**Biblioteca**: Recharts (React)

### 6. Lista de Compras
**Controllers**: `ShoppingController.php`  
**Endpoints**:
- `GET /api/shopping` - Listar itens
- `POST /api/shopping` - Adicionar item
- `DELETE /api/shopping/{id}` - Remover item

**Categorias**:
- Bar
- Limpeza
- Qualidade (produtos químicos)

### 7. Visitantes Diários
**Controllers**: `VisitController.php`  
**Lógica**:
- Incrementa automaticamente ao registar entrada
- Tabela `daily_visitors` com unique constraint em `visit_date`
- Usado para gráficos e relatórios

---

## 🔄 Recomendações de Portabilidade para .NET

### Prioridade ALTA (Essencial para TP2)

#### 1. Adicionar Base de Dados Persistente
**Esforço**: 🔴 Alto (6-8h)  
**Impacto**: ⭐⭐⭐⭐⭐ Crítico

**Ações**:
- Criar projeto Entity Framework Core
- Migrar schema SQL da versão PHP
- Implementar DbContext com as 8 tabelas
- Substituir `PoolService` singleton por repositórios com EF Core
- Configurar connection string em `appsettings.json`

**Benefícios**:
- Persistência de dados
- Histórico completo
- Relatórios possíveis
- Alinhado com requisitos TP2 (repositório de dados)

#### 2. Gestão de Trabalhadores + Turnos
**Esforço**: 🟡 Médio (4-6h)  
**Impacto**: ⭐⭐⭐⭐ Muito Alto

**Ações**:
- Criar `WorkerController.cs`
- Criar `WorkerService.cs` com lógica de turnos
- Criar models `Worker.cs` e `ActiveWorker.cs`
- Implementar endpoints CRUD
- Adicionar lógica de auto-desativação ao fechar piscina

**Benefícios**:
- Demonstra CRUD completo
- Lógica de negócio complexa
- Integração com sistema existente

#### 3. Qualidade da Água
**Esforço**: 🟢 Baixo (2-3h)  
**Impacto**: ⭐⭐⭐⭐ Muito Alto

**Ações**:
- Criar `WaterQualityController.cs`
- Criar model `WaterQuality.cs`
- Endpoints para registar e consultar medições
- Atualizar página pública para exibir dados

**Benefícios**:
- Funcionalidade visível ao público
- Demonstra integração frontend-backend
- Enriquece o projeto

#### 4. Relatórios Diários Automáticos
**Esforço**: 🔴 Alto (5-7h)  
**Impacto**: ⭐⭐⭐⭐⭐ Crítico

**Ações**:
- Criar `ReportController.cs`
- Criar `ReportService.cs` com lógica de agregação
- Implementar geração automática (trigger ao fechar piscina)
- Armazenar dados em JSON (como na versão PHP)

**Benefícios**:
- Demonstra processamento de dados complexo
- Alinhado com requisitos de "Dashboard de Monitorização"
- Impressiona avaliadores

#### 5. Gráficos e Estatísticas (Frontend)
**Esforço**: 🟡 Médio (3-4h)  
**Impacto**: ⭐⭐⭐⭐ Muito Alto

**Ações**:
- Instalar Recharts no frontend React
- Criar componentes de gráficos
- Endpoint para dados agregados (últimos 7 dias)
- Integrar no painel admin

**Benefícios**:
- UI profissional
- Demonstra capacidades de visualização
- Alinhado com "Dashboard de Monitorização"

### Prioridade MÉDIA (Recomendado)

#### 6. Registo de Limpezas
**Esforço**: 🟢 Baixo (2h)  
**Impacto**: ⭐⭐⭐ Médio

**Ações**:
- Criar `CleaningController.cs`
- Model `Cleaning.cs`
- Exibir última limpeza na página pública

#### 7. Histórico de Visitantes
**Esforço**: 🟢 Baixo (1-2h)  
**Impacto**: ⭐⭐ Baixo

**Ações**:
- Tabela `daily_visitors`
- Incrementar automaticamente ao registar entrada
- Usar para gráficos

### Prioridade BAIXA (Opcional)

#### 8. Lista de Compras
**Esforço**: 🟢 Baixo (1-2h)  
**Impacto**: ⭐ Muito Baixo

**Ações**:
- Criar `ShoppingController.cs`
- CRUD básico

---

## 🎨 Melhorias de UI/UX da Versão PHP

### Componentes Shadcn/ui
A versão PHP usa **Shadcn/ui**, uma biblioteca de componentes React moderna:
- `Card`, `Button`, `Input`, `Select`, `Switch`, `Badge`
- Componentes acessíveis e estilizados
- Fácil de integrar na versão .NET

**Recomendação**: Migrar componentes Shadcn/ui para o frontend .NET

### React Hot Toast
Sistema de notificações elegante:
```javascript
toast.success("Trabalhador ativado com sucesso!");
toast.error("Erro ao registar medição");
```

**Recomendação**: Adicionar ao frontend .NET

### Recharts
Gráficos interativos:
- LineChart (fluxo de visitantes)
- BarChart (turnos por trabalhador)
- AreaChart (ocupação)

**Recomendação**: Essencial para dashboard de monitorização

---

## 📋 Alinhamento com Requisitos TP2

### Como as Funcionalidades PHP Ajudam a Cumprir o Enunciado

| Requisito TP2 | Funcionalidade PHP a Portar | Benefício |
|---------------|----------------------------|-----------|
| **Serviços RESTful CRUD** | Trabalhadores, Qualidade Água, Limpezas | Demonstra CRUD completo em múltiplos recursos |
| **Repositório de Dados** | Base de dados MySQL com 8 tabelas | Cumpre requisito de persistência |
| **Dashboard de Monitorização** | Relatórios + Gráficos + Estatísticas | Cumpre sugestão do enunciado |
| **Integração de Serviços** | Meteorologia (já existe) + possível georreferenciação | Demonstra consumo de APIs externas |
| **Aplicação Cliente** | Frontend React expandido | Demonstra todos os serviços desenvolvidos |

---

## 🚀 Plano de Implementação Sugerido

### Fase 1: Fundação (Prioridade CRÍTICA)
**Tempo estimado**: 8-10 horas

1. **Adicionar Entity Framework Core**
   - Instalar pacotes NuGet
   - Criar DbContext
   - Migrar schema SQL
   - Configurar connection string

2. **Migrar PoolService para usar EF Core**
   - Substituir estado em memória
   - Implementar repositório

### Fase 2: Funcionalidades Core (Prioridade ALTA)
**Tempo estimado**: 10-12 horas

3. **Gestão de Trabalhadores**
   - Controller + Service + Models
   - Endpoints CRUD
   - Sistema de turnos

4. **Qualidade da Água**
   - Controller + Model
   - Endpoints de registo e consulta
   - Integração na página pública

5. **Relatórios Diários**
   - Controller + Service
   - Lógica de agregação
   - Geração automática

### Fase 3: UI/UX (Prioridade ALTA)
**Tempo estimado**: 4-6 horas

6. **Gráficos e Estatísticas**
   - Instalar Recharts
   - Criar componentes de gráficos
   - Endpoints de dados agregados

7. **Componentes Shadcn/ui**
   - Migrar componentes da versão PHP
   - Melhorar UI do admin

### Fase 4: Funcionalidades Secundárias (Prioridade MÉDIA)
**Tempo estimado**: 3-4 horas

8. **Limpezas**
9. **Histórico de Visitantes**

### Fase 5: Requisitos TP2 Faltantes (Prioridade CRÍTICA)
**Tempo estimado**: 15-20 horas

10. **Serviços SOAP** (ver relatório anterior)
11. **Swagger UI**
12. **Testes Automatizados**
13. **OAuth/JWT**
14. **Deploy na Cloud**

---

## 📊 Estimativa Total de Esforço

| Categoria | Tempo Estimado |
|-----------|----------------|
| **Portabilidade PHP → .NET** | 25-32 horas |
| **Requisitos TP2 Faltantes** | 15-20 horas |
| **TOTAL** | **40-52 horas** |

---

## ✅ Conclusão e Recomendações Finais

### Estratégia Recomendada

**Opção A: Expandir Versão .NET (Recomendado)**
- ✅ Manter stack .NET (alinhado com UC)
- ✅ Portar funcionalidades da versão PHP
- ✅ Adicionar requisitos TP2 faltantes (SOAP, Swagger, Cloud)
- ⏱️ Tempo: 40-52 horas

**Vantagens**:
- Demonstra domínio de .NET (foco da UC)
- Projeto mais robusto e completo
- Cumpre todos os requisitos do enunciado

**Opção B: Adaptar Versão PHP**
- ⚠️ Adicionar serviços SOAP em PHP
- ⚠️ Adicionar Swagger
- ⚠️ Já está publicado (Hostinger)
- ⏱️ Tempo: 15-20 horas

**Desvantagens**:
- PHP não é o foco da UC (aulas são em .NET C#)
- Menos alinhado com objetivos pedagógicos

### Recomendação Final

**Usar a versão PHP como BLUEPRINT** para expandir a versão .NET:

1. **Copiar a estrutura de dados** (schema SQL)
2. **Portar a lógica de negócio** (trabalhadores, qualidade água, relatórios)
3. **Reutilizar componentes React** (frontend é compatível)
4. **Adicionar requisitos TP2** (SOAP, Swagger, testes, cloud)

Desta forma:
- ✅ Aproveita o trabalho já feito
- ✅ Demonstra domínio de .NET
- ✅ Cumpre requisitos do enunciado
- ✅ Projeto impressionante e completo

---

## 📁 Ficheiros Chave para Consulta

### Da Versão PHP (para referência)
- `database/schema_complete.sql` - Schema completo
- `backend/app/Controllers/WorkerController.php` - Lógica de trabalhadores
- `backend/app/Controllers/ReportController.php` - Lógica de relatórios
- `backend/app/Services/` - Lógica de negócio
- `frontend/src/pages/admin/admin.jsx` - UI do painel admin
- `frontend/src/components/` - Componentes reutilizáveis

### Para Criar na Versão .NET
- `PoolTracker.API/Data/PoolTrackerDbContext.cs` - Entity Framework
- `PoolTracker.API/Models/` - Adicionar Worker, WaterQuality, Cleaning, Report
- `PoolTracker.API/Controllers/` - Adicionar novos controllers
- `PoolTracker.API/Services/` - Adicionar novos services
- `pooltracker-web/src/components/` - Migrar componentes Shadcn/ui

---

**Próximos Passos**: Decidir entre Opção A ou B e criar plano de implementação detalhado.
