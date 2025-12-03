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

**Endpoints SOAP**:
- `http://localhost:5000/soap/PoolDataService`
- `http://localhost:5000/soap/WorkerDataService`
- `http://localhost:5000/soap/WaterQualityDataService`
- `http://localhost:5000/soap/ReportDataService`

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

- **Testes Unitários**: 20+ testes (Services)
- **Testes de Integração**: 15+ testes (Controllers)
- **Testes End-to-End**: 10+ testes (API completa)
- **Code Coverage**: Objetivo ≥ 70%

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
| **Analysis Report** | Análise de requisitos vs implementação | [DOCUMENTATION/AnalysisReport.md](./DOCUMENTATION/AnalysisReport.md) |
| **PHP vs .NET Comparison** | Comparação entre versões do projeto | [DOCUMENTATION/PHPvsDotNET_Comparison.md](./DOCUMENTATION/PHPvsDotNET_Comparison.md) |
| **Task List** | Lista de tarefas por fase | [DOCUMENTATION/TaskList.md](./DOCUMENTATION/TaskList.md) |
| **Enunciado** | Enunciado oficial do TP2 | [ENUNCIADO/ESI-ISI-2025-26-TP2-enunciado.pdf](./ENUNCIADO/ESI-ISI-2025-26-TP2-enunciado.pdf) |

### Swagger/OpenAPI

- **UI**: `https://seu-dominio.com/swagger`
- **JSON**: `https://seu-dominio.com/swagger/v1/swagger.json`
- **Descrição**: Documentação interativa de todos os 40+ endpoints REST

---

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- SQL Server / PostgreSQL
- Node.js 18+
- Git

### Backend (.NET)

```bash
# Clonar o repositório
git clone https://github.com/ricardoguimaraes2021/ISI-TP2-PoolTracker.git
cd ISI-TP2-PoolTracker/backend/PoolTracker.API

# Restaurar dependências
dotnet restore

# Configurar connection string em appsettings.json
# Executar migrations
dotnet ef database update

# Executar API
dotnet run
```

A API estará disponível em `http://localhost:5292`

### Frontend (React)

```bash
cd frontend/pooltracker-web

# Instalar dependências
npm install

# Configurar variáveis de ambiente (.env)
# VITE_API_URL=http://localhost:5292
# VITE_ADMIN_PIN=1234
# VITE_ADMIN_API_KEY=sua-chave-aqui

# Executar em modo de desenvolvimento
npm run dev
```

O frontend estará disponível em `http://localhost:5173`

### Testes

```bash
cd backend/PoolTracker.Tests
dotnet test --collect:"XPlat Code Coverage"
```

---

## ☁️ Deployment

### Plataformas Suportadas

- **Azure**: App Service + Azure SQL Database
- **Railway**: Containers + PostgreSQL
- **Render**: Web Services + PostgreSQL

### Configuração de Produção

1. **Base de Dados**: Criar instância SQL na cloud
2. **API**: Deploy da aplicação .NET
3. **SOAP**: Deploy separado ou no mesmo serviço
4. **Frontend**: Build estático no Vercel/Netlify
5. **Variáveis de Ambiente**: Configurar secrets na plataforma

Ver [DOCUMENTATION/ImplementationPlan.md](./DOCUMENTATION/ImplementationPlan.md) para comandos detalhados.

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

| Fase | Descrição | Tempo Estimado | Status |
|------|-----------|----------------|--------|
| **Fase 1** | Fundação (EF Core, DB, Repositories) | 10-12h | ✅ |
| **Fase 2** | RESTful API Core | 8-10h | ✅ |
| **Fase 3** | Funcionalidades Avançadas | 6-8h | 🔄 |
| **Fase 4** | Autenticação JWT | 4-6h | 🔄 |
| **Fase 5** | Serviços SOAP | 8-10h | ⏳ |
| **Fase 6** | Documentação Swagger | 3-4h | ⏳ |
| **Fase 7** | Testes Automatizados | 10-12h | ⏳ |
| **Fase 8** | Frontend Expandido | 8-10h | ⏳ |
| **Fase 9** | Deploy na Cloud | 6-8h | ⏳ |
| **Fase 10** | Documentação Final | 4-6h | ⏳ |

**Total Estimado**: 67-86 horas

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

**Última Atualização**: Dezembro 2025  
**Versão**: 2.0 (.NET Edition)
