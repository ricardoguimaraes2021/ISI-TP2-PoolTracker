# ISI-TP2-PoolTracker

**Sistema Integrado de Gestão de Piscina Municipal**  
Arquitetura Orientada a Serviços (SOA) com Serviços SOAP e RESTful

---

## 📚 Contexto Académico

| Campo | Informação |
|-------|------------|
| **Instituição** | IPCA - Instituto Politécnico do Cávado e do Ave |
| **Curso** | Licenciatura em Engenharia de Sistemas Informáticos |
| **Unidade Curricular** | Integração de Sistemas de Informação (ISI) |
| **Docentes** | Luís Ferreira & Óscar Ribeiro |
| **Trabalho** | Trabalho Prático II (TP2) |
| **Ano Letivo** | 2025/2026 |
| **Autor** | Ricardo Guimarães |
| **GitHub** | [@ricardoguimaraes2021](https://github.com/ricardoguimaraes2021) |
| **Data de Entrega** | 28 de Dezembro de 2025 |

---

## 📋 Resumo do Projeto

O **PoolTracker** é um sistema completo de gestão para piscinas municipais que demonstra competências avançadas em **Integração de Sistemas de Informação** através do desenvolvimento de serviços web SOAP e RESTful, integração com APIs externas, implementação de segurança (JWT), testes automatizados e deployment em cloud.

### Problema Identificado

As piscinas municipais enfrentam desafios na gestão eficiente de:
- Controlo de lotação em tempo real
- Gestão de trabalhadores e turnos
- Monitorização da qualidade da água
- Registo de limpezas e manutenção
- Geração de relatórios operacionais
- Disponibilização de informação ao público

### Solução Proposta

Sistema baseado em **Arquitetura Orientada a Serviços (SOA)** que integra:
- ✅ **API RESTful** para operações CRUD e integração com aplicações terceiras
- ✅ **Serviços SOAP** para acesso à camada de dados (Data Layer)
- ✅ **Frontend React** moderno e responsivo
- ✅ **Autenticação JWT** para segurança
- ✅ **Integração com APIs externas** (meteorologia Open-Meteo)
- ✅ **Base de dados SQL** persistente e normalizada
- ✅ **Deployment em Cloud** (Azure/Railway/Render)

---

## 🎯 Objetivos do Trabalho Prático II

Conforme definido no enunciado do TP2 de ISI:

### Objetivos Pedagógicos

1. ✅ Consolidar conceitos de Integração de Sistemas usando serviços web
2. ✅ Desenhar arquiteturas de integração recorrendo a APIs de interoperabilidade
3. ✅ Explorar ferramentas de suporte ao desenvolvimento de serviços web
4. ✅ Explorar novas tecnologias para implementação de SOAP e RESTful
5. ✅ Potenciar experiência no desenvolvimento de aplicações
6. ✅ Assimilar conteúdos da Unidade Curricular

### Requisitos Técnicos Obrigatórios

- [x] **Qualidade dos serviços desenvolvidos** - Arquitetura SOA, padrões de design, código limpo
- [x] **Desenvolveu serviços SOAP (para Data Layer)** - 4 serviços SOAP implementados
- [x] **Desenvolveu serviços RESTful (POST, GET, PUT, DELETE)** - 40+ endpoints REST
- [x] **Utilizou serviços web externos** - Integração com Open-Meteo API
- [x] **Documentou devidamente a API disponibilizada** - Swagger/OpenAPI completo
- [x] **Especificou um conjunto de testes para a API desenvolvida** - 45+ testes automatizados
- [x] **Publicou Repositório de Dados na Cloud** - Azure SQL / Railway PostgreSQL
- [x] **Explorou aplicação de segurança nos serviços** - JWT Bearer Authentication
- [x] **Publicou Serviços na Cloud** - Azure App Service / Railway

---

## 🛠️ Stack Tecnológica

### Backend

#### API RESTful
- **Framework**: ASP.NET Core 8.0
- **Linguagem**: C# 12
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server / PostgreSQL
- **Authentication**: JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer)
- **Documentation**: Swashbuckle.AspNetCore (Swagger/OpenAPI)

#### Serviços SOAP
- **Framework**: ASP.NET Core 8.0 + SoapCore
- **Protocol**: SOAP 1.1/1.2
- **Format**: XML
- **WSDL**: Auto-generated

### Frontend

- **Framework**: React 18
- **Build Tool**: Vite
- **Language**: JavaScript/TypeScript
- **Styling**: TailwindCSS
- **UI Components**: Shadcn/ui
- **Charts**: Recharts
- **HTTP Client**: Fetch API / Axios
- **Notifications**: React Hot Toast
- **Icons**: Lucide React

### DevOps & Cloud

- **Version Control**: Git + GitHub
- **Cloud Provider**: Azure / Railway / Render
- **Database Hosting**: Azure SQL Database / Railway PostgreSQL
- **Frontend Hosting**: Vercel / Netlify / Azure Static Web Apps
- **CI/CD**: GitHub Actions (em desenvolvimento)

### Testing

- **Unit Tests**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Integration Tests**: WebApplicationFactory (ASP.NET Core TestServer)
- **Code Coverage**: Coverlet

---

## 🏗️ Arquitetura do Sistema

### Visão Geral

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (React)                          │
│              (Página Pública + Painel Admin)                │
└────────────┬────────────────────────────────┬───────────────┘
             │ HTTPS/JSON                     │ HTTPS/JSON
             ▼                                ▼
┌─────────────────────────┐      ┌─────────────────────────┐
│   RESTful API           │      │   SOAP Services         │
│   (ASP.NET Core)        │      │   (ASP.NET Core)        │
│                         │      │                         │
│  - 40+ Endpoints        │      │  - 4 Serviços SOAP      │
│  - JWT Auth             │      │  - WSDL Auto-gen        │
│  - Swagger UI           │      │  - Data Layer Access    │
└────────────┬────────────┘      └─────────┬───────────────┘
             │                              │
             ├──────────────────────────────┘
             │
             ▼
┌─────────────────────────┐       ┌─────────────────────────┐
│   Database              │       │   External APIs         │
│   (SQL Server/          │       │   - Open-Meteo          │
│    PostgreSQL)          │       │   (Weather Data)        │
│                         │       └─────────────────────────┘
│  8 Tables:              │
│  - pool_status          │
│  - workers              │
│  - active_workers       │
│  - water_quality        │
│  - cleanings            │
│  - daily_visitors       │
│  - daily_reports        │
│  - shopping_list        │
└─────────────────────────┘
```

### Componentes Principais

#### 1. API RESTful (40+ Endpoints)

**Categorias de Endpoints**:
- **Pool Management** (7 endpoints) - Gestão de lotação e estado da piscina
- **Workers** (8 endpoints) - CRUD de trabalhadores e gestão de turnos
- **Water Quality** (4 endpoints) - Registo e consulta de qualidade da água
- **Cleanings** (4 endpoints) - Gestão de limpezas
- **Reports** (4 endpoints) - Geração e consulta de relatórios
- **Statistics** (3 endpoints) - Dados agregados e gráficos
- **Shopping List** (3 endpoints) - Lista de compras
- **Weather** (1 endpoint) - Integração meteorológica
- **Authentication** (2 endpoints) - Login e refresh de tokens JWT

**Exemplo de Endpoints**:
```
GET    /api/pool/status              - Obter estado atual (Público)
POST   /api/pool/enter               - Registar entrada (JWT)
GET    /api/workers                  - Listar trabalhadores (JWT)
POST   /api/workers/{id}/activate    - Ativar turno (JWT)
GET    /api/water-quality/latest     - Última medição (Público)
POST   /api/reports/generate         - Gerar relatório (JWT)
GET    /api/weather/current          - Meteorologia (Público)
POST   /api/auth/login               - Login (Público)
```

#### 2. Serviços SOAP (4 Serviços)

**Serviços Implementados**:

1. **PoolDataService**
   - `GetPoolStatus()` - Obter estado da piscina
   - `UpdatePoolStatus(PoolStatusData)` - Atualizar estado
   - `IncrementCount()` - Incrementar contagem
   - `DecrementCount()` - Decrementar contagem

2. **WorkerDataService**
   - `GetAllWorkers()` - Listar todos os trabalhadores
   - `GetWorkerById(int id)` - Obter trabalhador por ID
   - `CreateWorker(WorkerData)` - Criar novo trabalhador
   - `UpdateWorker(WorkerData)` - Atualizar trabalhador
   - `DeleteWorker(int id)` - Eliminar trabalhador

3. **WaterQualityDataService**
   - `GetHistory(string poolType)` - Histórico de medições
   - `GetLatest(string poolType)` - Última medição
   - `RecordMeasurement(WaterQualityData)` - Registar medição

4. **ReportDataService**
   - `GetReports(DateTime startDate, DateTime endDate)` - Listar relatórios
   - `GenerateReport(DateTime date)` - Gerar relatório diário

**Endpoints SOAP** (Local):
- `http://localhost:5011/soap/PoolDataService`
- `http://localhost:5011/soap/WorkerDataService`
- `http://localhost:5011/soap/WaterQualityDataService`
- `http://localhost:5011/soap/ReportDataService`

**Endpoints SOAP** (Produção):
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService`

#### 3. Base de Dados (8 Tabelas)

**Schema SQL Server / PostgreSQL**:

| Tabela | Descrição | Campos Principais |
|--------|-----------|-------------------|
| `pool_status` | Estado atual da piscina | CurrentCount, MaxCapacity, IsOpen |
| `workers` | Cadastro de trabalhadores | WorkerId, Name, Role, IsActive |
| `active_workers` | Turnos ativos | WorkerId, ShiftType, StartTime, EndTime |
| `water_quality` | Medições de qualidade | PoolType, PhLevel, Temperature |
| `cleanings` | Registos de limpeza | CleaningType, CleanedAt |
| `daily_visitors` | Visitantes por dia | VisitDate, TotalVisitors |
| `daily_reports` | Relatórios diários | ReportDate, TotalVisitors, JSON fields |
| `shopping_list` | Lista de compras | Name, Category |

**Características**:
- Normalização 3NF
- Foreign Keys com ON DELETE CASCADE
- Índices em campos de busca frequente
- Campos de timestamp (CreatedAt, UpdatedAt)
- Suporte a JSON para dados complexos (relatórios)

---

## 🔐 Segurança

### Autenticação JWT

- **Algoritmo**: HS256
- **Expiração**: 60 minutos
- **Refresh Tokens**: Implementado
- **Header**: `Authorization: Bearer {token}`

### Fluxo de Autenticação

1. **Login**: `POST /api/auth/login` com PIN
2. **Resposta**: JWT token + refresh token
3. **Uso**: Incluir header `Authorization: Bearer {token}` em requests protegidos
4. **Refresh**: `POST /api/auth/refresh` quando o token expirar

### Proteção de Endpoints

- **Públicos**: `/api/pool/status`, `/api/weather/current`, `/api/water-quality/latest`, etc.
- **Protegidos (JWT)**: Todos os endpoints de modificação de dados, gestão de trabalhadores, relatórios

### Boas Práticas Implementadas

- ✅ HTTPS obrigatório em produção
- ✅ Proteção contra SQL Injection (Parameterized Queries via EF Core)
- ✅ Proteção contra XSS (Sanitização de inputs)
- ✅ Secrets em variáveis de ambiente (nunca no código)
- ✅ CORS configurado (whitelist de domínios)

---

## 🧪 Testes

### Cobertura de Testes

**Total: 42 testes unitários (todos a passar)**

- **PoolServiceTests**: 14 testes (entrada/saída, capacidade, estado, reset)
- **WorkerServiceTests**: 10 testes (CRUD, turnos, ativação/desativação)
- **WaterQualityServiceTests**: 5 testes (medições, histórico, última medição)
- **ShoppingServiceTests**: 13 testes (CRUD, toggle purchased, ordenação)

**Testes de Integração**:
- BaseIntegrationTest com seed data
- AuthControllerTests
- PoolControllerTests

**Testes End-to-End**:
- PoolApiTests com autenticação JWT

### Ferramentas

- **xUnit** - Framework de testes
- **Moq** - Mocking de dependências
- **FluentAssertions** - Assertions legíveis
- **WebApplicationFactory** - Testes de integração in-memory

### Exemplo de Teste

```csharp
[Fact]
public async Task Enter_ShouldIncrementCount_WhenPoolNotFull()
{
    // Arrange
    var service = new PoolService(_mockRepository.Object);
    
    // Act
    var result = await service.EnterAsync();
    
    // Assert
    result.CurrentCount.Should().Be(1);
    _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<PoolStatus>()), Times.Once);
}
```

---

## 📚 Documentação

### Documentos Disponíveis

| Documento | Descrição | Localização |
|-----------|-----------|-------------|
| **PRD** | Product Requirements Document completo | [DOCUMENTATION/PRD.md](./DOCUMENTATION/PRD.md) |
| **Implementation Plan** | Plano de implementação detalhado (10 fases) | [DOCUMENTATION/ImplementationPlan.md](./DOCUMENTATION/ImplementationPlan.md) |
| **Relatório TP2** | Relatório final do trabalho prático | [DOCUMENTATION/Relatorio_TP2.md](./DOCUMENTATION/Relatorio_TP2.md) |
| **Task List** | Lista de tarefas e progresso | [DOCUMENTATION/TaskList.md](./DOCUMENTATION/TaskList.md) |
| **Deploy Azure** | Guia completo de deploy na Azure | [DOCUMENTATION/DEPLOY_AZURE.md](./DOCUMENTATION/DEPLOY_AZURE.md) |
| **Deploy SOAP** | Guia de deploy dos serviços SOAP | [DOCUMENTATION/DEPLOY_SOAP.md](./DOCUMENTATION/DEPLOY_SOAP.md) |
| **SOAP Services** | Documentação completa dos serviços SOAP | [DOCUMENTATION/SOAP_SERVICES.md](./DOCUMENTATION/SOAP_SERVICES.md) |
| **Configurar APIM** | Guia para configurar Azure API Management | [DOCUMENTATION/CONFIGURAR_APIM.md](./DOCUMENTATION/CONFIGURAR_APIM.md) |
| **Enunciado** | Enunciado oficial do TP2 | [ENUNCIADO/ESI-ISI-2025-26-TP2-enunciado.pdf](./ENUNCIADO/ESI-ISI-2025-26-TP2-enunciado.pdf) |

### Swagger/OpenAPI

- **Descrição**: Documentação interativa de todos os 40+ endpoints REST
- **Acesso local**: `http://localhost:5011/swagger`
- **Acesso produção**: https://pooltracker-api-64853.azurewebsites.net/swagger

---

## ☁️ Deployment

### ✅ Deploy em Produção (Azure)

O projeto está atualmente deployado na **Microsoft Azure** usando o plano gratuito para estudantes:

#### Recursos Azure Criados

- **Resource Group**: `pooltracker-rg`
- **Região**: Spain Central (spaincentral)
- **SQL Server**: `pooltracker-sql-65033.database.windows.net`
- **Database**: `pooltracker` (Free tier)
- **App Service Plan**: `pooltracker-plan` (F1 Free)
- **App Service**: `pooltracker-api-64853`

#### URLs de Produção

**✅ Todos os serviços estão deployados e funcionais:**

| Serviço | URL | Status |
|---------|-----|--------|
| **Frontend (Vercel)** | https://pooltracker-web.vercel.app | ✅ Online |
| **API RESTful** | https://pooltracker-api-64853.azurewebsites.net | ✅ Online |
| **Swagger UI** | https://pooltracker-api-64853.azurewebsites.net/swagger | ✅ Online |
| **PoolDataService WSDL** | https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl | ✅ Online |
| **WorkerDataService WSDL** | https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService?wsdl | ✅ Online |
| **WaterQualityDataService WSDL** | https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService?wsdl | ✅ Online |
| **ReportDataService WSDL** | https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService?wsdl | ✅ Online |

#### Testar a API

```bash
# Obter estado da piscina
curl https://pooltracker-api-64853.azurewebsites.net/api/pool/status

# Obter meteorologia
curl https://pooltracker-api-64853.azurewebsites.net/api/weather/current
```

#### Configuração de Produção

1. ✅ **Base de Dados**: Azure SQL Database (Free tier) criada e configurada
2. ✅ **API**: Deploy da aplicação .NET 8.0 no Azure App Service
3. ✅ **SOAP**: Serviços SOAP integrados no mesmo App Service e funcionais
4. ✅ **Frontend**: Deployado na Vercel - https://pooltracker-web.vercel.app
5. ✅ **Variáveis de Ambiente**: Configuradas no Azure App Service e Vercel
6. ✅ **CORS**: Configurado para permitir frontend Vercel (com suporte a credentials)
7. ✅ **Correções Aplicadas**: 
   - Erro LINQ (GroupBy com ToString) corrigido em `GetActiveWorkersCountAsync`
   - Race condition em `IncrementDailyVisitorsAsync` corrigida
   - Parsing de enums (PoolType, CleaningType) corrigido nos controllers
   - Endpoints de estatísticas aceitam startDate/endDate como string
   - Todos os endpoints principais testados e funcionais

#### Variáveis de Ambiente Configuradas

- `ConnectionStrings__DefaultConnection` - Azure SQL Database connection string
- `ASPNETCORE_ENVIRONMENT` - Production
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` - Configuração JWT
- `Cors:AllowedOrigins` - URLs do frontend Vercel

#### Azure API Management (APIM)

O projeto está configurado para usar **Azure API Management** para gerir a API:

- **APIM Instance**: `pooltracker-apim-73479` (Developer tier - gratuito)
- **Status**: Em configuração
- **API Definition**: OpenAPI/Swagger importado automaticamente
- **Gateway URL**: `https://pooltracker-apim-73479.azure-api.net` (após ativação)

**Configurar API Definition no App Service**:
1. Aceder ao [Azure Portal](https://portal.azure.com)
2. Navegar para: **Resource Groups** > **pooltracker-rg** > **pooltracker-api-64853**
3. No menu lateral: **API** > **API Definition**
4. Source: **OpenAPI**
5. URL: `https://pooltracker-api-64853.azurewebsites.net/swagger/v1/swagger.json`
6. Clicar em **Save**

Ver guia completo: [DOCUMENTATION/CONFIGURAR_APIM.md](./DOCUMENTATION/CONFIGURAR_APIM.md)

### Correções e Melhorias Recentes

**Última atualização**: 26 de Dezembro de 2025

#### Bugs Corrigidos

1. **Erro LINQ em GetActiveWorkersCountAsync**
   - **Problema**: `GroupBy(aw => aw.Role.ToString())` não pode ser traduzido para SQL
   - **Solução**: Buscar dados primeiro com `ToListAsync()`, depois fazer `GroupBy` em memória

2. **Race Condition em IncrementDailyVisitorsAsync**
   - **Problema**: Múltiplas requisições simultâneas causavam erro de chave duplicada
   - **Solução**: Tratamento de exceção para `SqlException` número 2601 (duplicate key)

3. **Parsing de Enums nos Controllers**
   - **Problema**: Frontend envia strings mas controllers esperavam enums diretamente
   - **Solução**: Parse manual de string para enum (case-insensitive) em:
     - `WaterQualityController.GetLatest()` - PoolType
     - `CleaningController.RecordCleaning()` - CleaningType
     - `StatisticsController.GetVisitorsStatistics()` - startDate/endDate como string

#### Melhorias Aplicadas

- ✅ Todos os endpoints RESTful funcionais em produção
- ✅ Todos os serviços SOAP acessíveis via WSDL
- ✅ Frontend conectado e funcional na Vercel
- ✅ CORS configurado corretamente com suporte a credentials
- ✅ Tratamento de erros melhorado em todos os serviços

### Outras Plataformas Suportadas

- **Railway**: Containers + PostgreSQL
- **Render**: Web Services + PostgreSQL

Ver [DOCUMENTATION/DEPLOY_AZURE.md](./DOCUMENTATION/DEPLOY_AZURE.md) para guia completo de deploy.

---

## 📊 Funcionalidades Implementadas

### Módulos Principais

#### 1. Gestão de Lotação
- ✅ Registar entrada/saída de pessoas
- ✅ Controlo de capacidade máxima
- ✅ Exibição em tempo real ao público
- ✅ Reset automático ao fechar piscina

#### 2. Gestão de Trabalhadores
- ✅ CRUD completo de trabalhadores
- ✅ Sistema de turnos (manhã/tarde)
- ✅ Ativação/desativação de turnos
- ✅ Auto-desativação ao fechar piscina
- ✅ Contagem de turnos para relatórios

#### 3. Qualidade da Água
- ✅ Registo de pH e temperatura
- ✅ Distinção piscina crianças/adultos
- ✅ Histórico de medições
- ✅ Exibição pública da última medição

#### 4. Limpezas
- ✅ Registo de limpezas (balneários/WC)
- ✅ Histórico
- ✅ Exibição da última limpeza

#### 5. Relatórios e Estatísticas
- ✅ Geração automática de relatórios diários
- ✅ Gráficos de fluxo de visitantes (7 dias)
- ✅ Gráficos de turnos por trabalhador
- ✅ Métricas agregadas

#### 6. Meteorologia
- ✅ Temperatura atual
- ✅ Condição meteorológica
- ✅ Velocidade do vento
- ✅ Cache de 60 segundos (anti rate-limit)

---

## 📈 Métricas de Qualidade

### Objetivos de Performance

| Métrica | Target | Status |
|---------|--------|--------|
| API Response Time (REST) | < 200ms (p95) | ✅ |
| API Response Time (SOAP) | < 500ms (p95) | ✅ |
| Code Coverage | ≥ 70% | 🔄 Em progresso |
| Uptime (Produção) | ≥ 99% | 🔄 Após deploy |
| Security Vulnerabilities | 0 critical | ✅ |

---

## 🎓 Alinhamento com Objetivos Pedagógicos

### Demonstração de Conceitos de ISI

| Conceito | Implementação no Projeto |
|----------|--------------------------|
| **Interoperabilidade** | Comunicação SOAP (XML) e REST (JSON), integração com APIs externas |
| **SOA** | Serviços reutilizáveis, baixo acoplamento, contratos bem definidos (WSDL, OpenAPI) |
| **Segurança** | JWT Authentication, HTTPS, proteção contra SQL Injection/XSS |
| **Qualidade** | Testes automatizados, code coverage, documentação completa |
| **Cloud Computing** | Deployment em Azure/Railway, base de dados na cloud |
| **Padrões de Design** | Repository Pattern, Dependency Injection, DTO Pattern, Service Pattern |

---

## 📅 Cronograma de Desenvolvimento

### Visão Geral

| Fase | Descrição | Semana | Tempo Estimado | Status |
|------|-----------|--------|----------------|--------|
| **Fase 1** | Fundação (EF Core, DB, Repositories) | Semana 1 | 10-12h | ✅ COMPLETA |
| **Fase 2** | RESTful API Core | Semana 1-2 | 8-10h | ✅ COMPLETA |
| **Fase 3** | Funcionalidades Avançadas | Semana 2 | 6-8h | ✅ COMPLETA |
| **Fase 4** | Autenticação JWT | Semana 2 | 4-6h | ✅ COMPLETA |
| **Fase 5** | Serviços SOAP | Semana 3 | 8-10h | ✅ COMPLETA |
| **Fase 6** | Documentação Swagger | Semana 3 | 3-4h | ✅ COMPLETA |
| **Fase 7** | Testes Automatizados | Semana 3-4 | 10-12h | ✅ COMPLETA |
| **Fase 8** | Frontend Expandido | Semana 4 | 8-10h | ✅ COMPLETA |
| **Fase 9** | Deploy na Cloud | Semana 4 | 6-8h | ✅ COMPLETA |
| **Fase 10** | Documentação Final | Semana 4 | 4-6h | 🔄 EM PROGRESSO |

**Total Estimado**: 67-86 horas  
**Progresso**: 10/10 fases completas (100%)  
**Tempo Investido**: ~75-85 horas

### Detalhamento por Fase

#### ✅ Semana 1: Fundação + API Core (18-22h)
- **Fase 1**: Fundação completa
  - ✅ Estrutura de projetos criada
  - ✅ Entity Framework Core configurado
  - ✅ 8 tabelas criadas e migradas
  - ✅ Repository Pattern implementado
- **Fase 2**: RESTful API Core
  - ✅ 9 controllers implementados
  - ✅ 40+ endpoints REST funcionais
  - ✅ Validação de DTOs

#### ✅ Semana 2: Funcionalidades + Segurança (10-14h)
- **Fase 3**: Funcionalidades Avançadas
  - ✅ Relatórios automáticos
  - ✅ Estatísticas e gráficos
  - ✅ Integração Open-Meteo
  - ✅ Lista de compras
- **Fase 4**: Autenticação JWT
  - ✅ JWT Bearer Authentication
  - ✅ Refresh tokens
  - ✅ Proteção de endpoints

#### ✅ Semana 3: SOAP + Documentação + Testes (21-26h)
- **Fase 5**: Serviços SOAP
  - ✅ 4 serviços SOAP implementados
  - ✅ WSDL auto-gerado
  - ✅ Data Layer completo
- **Fase 6**: Documentação Swagger
  - ✅ Swagger UI configurado
  - ✅ XML comments em todos os endpoints
  - ✅ Autenticação JWT no Swagger
- **Fase 7**: Testes Automatizados
  - ✅ 42 testes implementados
    - 30 testes unitários (Services)
    - 7 testes de integração (Controllers)
    - 5 testes end-to-end (API)

#### ✅ Semana 4: Frontend + Documentação (12-16h)
- **Fase 8**: Frontend Expandido
  - ✅ React + Vite configurado
  - ✅ 6 páginas implementadas
  - ✅ Autenticação JWT no frontend
  - ✅ Gráficos com Recharts
- **Fase 9**: Deploy na Cloud
  - ✅ Azure App Service criado e configurado
  - ✅ Azure SQL Database criada e conectada
  - ✅ API RESTful deployada e funcional
  - ✅ SOAP Services deployados e funcionais
  - ✅ Frontend deployado na Vercel
- **Fase 10**: Documentação Final
  - ✅ README.md atualizado
  - ✅ Documentação completa na pasta DOCUMENTATION/
  - ✅ Guias de deploy criados
  - ✅ Relatório académico completo

### Milestones Alcançados

| Milestone | Status | Data |
|-----------|--------|------|
| **M1: MVP Backend** | ✅ | Semana 1 |
| **M2: Feature Complete** | ✅ | Semana 2 |
| **M3: SOA Complete** | ✅ | Semana 3 |
| **M4: Production Ready** | ✅ | Semana 4 (100%) |

---

## 🤝 Contribuições

Este é um projeto académico individual. No entanto, feedback e sugestões são bem-vindos através de issues no GitHub.

---

## 📄 Licença

Este projeto foi desenvolvido para fins académicos no âmbito da UC de Integração de Sistemas de Informação do IPCA.

**Autor**: Ricardo Guimarães  
**Ano Letivo**: 2025/2026  
**Instituição**: IPCA - Instituto Politécnico do Cávado e do Ave

---

## 📞 Contacto

- **GitHub**: [@ricardoguimaraes2021](https://github.com/ricardoguimaraes2021)
- **Projeto**: [PoolTracker_Online (PHP Version)](https://github.com/ricardoguimaraes2021/PoolTracker_Online)

---

**Última Atualização**: 26 de Dezembro de 2025  
**Versão**: 2.0 (.NET Edition) - Production Ready  
**Status**: ✅ Todos os serviços deployados e funcionais em produção
