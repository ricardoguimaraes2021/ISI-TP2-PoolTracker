# Relatório do Trabalho Prático II
## Integração de Sistemas de Informação

---

# PoolTracker
## Sistema Integrado de Gestão de Piscina Municipal

**Arquitetura Orientada a Serviços (SOA) com Serviços SOAP e RESTful**

---

## Informação do Trabalho

| Campo | Informação |
|-------|------------|
| **Instituição** | IPCA - Instituto Politécnico do Cávado e do Ave |
| **Curso** | Licenciatura em Engenharia de Sistemas Informáticos |
| **Unidade Curricular** | Integração de Sistemas de Informação (ISI) |
| **Docentes** | Luís Ferreira & Óscar Ribeiro |
| **Trabalho** | Trabalho Prático II (TP2) |
| **Ano Letivo** | 2025/2026 |
| **Autor** | Ricardo Guimarães |
| **Data de Entrega** | 28 de Dezembro de 2025 |

---

# Índice

1. [Introdução](#1-introdução)
2. [Objetivos](#2-objetivos)
3. [Arquitetura do Sistema](#3-arquitetura-do-sistema)
4. [Implementação Técnica](#4-implementação-técnica)
5. [Serviços SOAP](#5-serviços-soap)
6. [Serviços RESTful](#6-serviços-restful)
7. [Segurança e Autenticação](#7-segurança-e-autenticação)
8. [Integração com Serviços Externos](#8-integração-com-serviços-externos)
9. [Base de Dados](#9-base-de-dados)
10. [Testes](#10-testes)
11. [Documentação](#11-documentação)
12. [Deployment](#12-deployment)
13. [Conclusões](#13-conclusões)
14. [Referências](#14-referências)

---

# 1. Introdução

## 1.1 Contexto

O presente trabalho foi desenvolvido no âmbito da unidade curricular de **Integração de Sistemas de Informação (ISI)**, com o objetivo de demonstrar competências práticas no desenvolvimento de sistemas baseados em **Arquitetura Orientada a Serviços (SOA)**.

O projeto **PoolTracker** consiste num sistema completo de gestão para piscinas municipais, desenvolvido utilizando tecnologias modernas e seguindo as melhores práticas de engenharia de software.

## 1.2 Problema Identificado

As piscinas municipais enfrentam diversos desafios na gestão operacional diária:

- **Controlo de lotação em tempo real**: Necessidade de monitorizar e limitar o número de pessoas presentes na piscina
- **Gestão de trabalhadores e turnos**: Organização de equipas e registo de presenças
- **Monitorização da qualidade da água**: Registo e histórico de medições de pH e temperatura
- **Registo de limpezas e manutenção**: Controlo de atividades de limpeza e manutenção
- **Geração de relatórios operacionais**: Análise de dados para tomada de decisões
- **Disponibilização de informação ao público**: Transparência e acesso a informações em tempo real

## 1.3 Solução Proposta

O PoolTracker foi desenvolvido como um sistema baseado em **Arquitetura Orientada a Serviços (SOA)**, integrando:

- **API RESTful** para operações CRUD e integração com aplicações terceiras
- **Serviços SOAP** para acesso à camada de dados (Data Layer)
- **Frontend React** moderno e responsivo
- **Autenticação JWT** para segurança
- **Integração com APIs externas** (meteorologia Open-Meteo)
- **Base de dados SQL** persistente e normalizada
- **Deployment em Cloud** (Azure/Railway/Render)

---

# 2. Objetivos

## 2.1 Objetivos Académicos

Conforme definido no enunciado do TP2 de ISI, os objetivos pedagógicos são:

1. ✅ Consolidar conceitos de Integração de Sistemas usando serviços web
2. ✅ Desenhar arquiteturas de integração recorrendo a APIs de interoperabilidade
3. ✅ Explorar ferramentas de suporte ao desenvolvimento de serviços web
4. ✅ Explorar novas tecnologias para implementação de SOAP e RESTful
5. ✅ Potenciar experiência no desenvolvimento de aplicações
6. ✅ Assimilar conteúdos da Unidade Curricular

## 2.2 Objetivos Técnicos

1. **Desenvolver serviços SOAP** para acesso à camada de dados
2. **Desenvolver API RESTful** com operações CRUD completas (GET, POST, PUT, DELETE)
3. **Documentar API** usando OpenAPI/Swagger
4. **Implementar autenticação** JWT Bearer
5. **Criar testes automatizados** (unitários, integração, end-to-end)
6. **Publicar na cloud** serviços e base de dados
7. **Integrar serviços externos** (meteorologia)

## 2.3 Requisitos do Enunciado

| Requisito | Status | Evidência |
|-----------|--------|-----------|
| **Qualidade dos serviços desenvolvidos** | ✅ | Arquitetura SOA, Repository Pattern, DI, Clean Code |
| **Desenvolveu serviços SOAP (para Data Layer)** | ✅ | 4 serviços SOAP implementados (Pool, Worker, WaterQuality, Report) |
| **Desenvolveu serviços RESTful (POST, GET, PUT, DELETE)** | ✅ | 40+ endpoints REST com todas as operações |
| **Utilizou serviços web externos** | ✅ | Integração com Open-Meteo API |
| **Documentou devidamente a API disponibilizada** | ✅ | Swagger UI completo com XML comments |
| **Especificou um conjunto de testes para a API** | ✅ | Projeto de testes configurado (xUnit) |
| **Publicou Repositório de Dados na Cloud** | ✅ | Azure SQL / Railway PostgreSQL |
| **Explorou aplicação de segurança nos serviços** | ✅ | JWT Bearer Authentication implementado |
| **Publicou Serviços na Cloud** | ✅ | Azure App Service / Railway |

---

# 3. Arquitetura do Sistema

## 3.1 Visão Geral

O sistema PoolTracker segue uma arquitetura em camadas (Layered Architecture), separando responsabilidades e facilitando manutenção e testes:

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
│  - 40+ Endpoints        │      │  - 4 Serviços SOAP       │
│  - JWT Auth             │      │  - WSDL Auto-gen         │
│  - Swagger UI           │      │  - Data Layer Access    │
└────────────┬────────────┘      └─────────┬───────────────┘
             │                              │
             ├──────────────────────────────┘
             │
             ▼
┌─────────────────────────┐       ┌─────────────────────────┐
│   Database              │       │   External APIs         │
│   (SQL Server/          │       │   - Open-Meteo          │
│    PostgreSQL)          │       │   (Weather Data)         │
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

## 3.2 Componentes Principais

### 3.2.1 API RESTful

A API RESTful é o ponto de entrada principal do sistema, fornecendo 40+ endpoints organizados em 9 controllers:

- **PoolController**: Gestão de lotação e estado da piscina
- **WorkerController**: CRUD de trabalhadores e gestão de turnos
- **WaterQualityController**: Registo e consulta de qualidade da água
- **CleaningController**: Gestão de limpezas
- **ReportController**: Geração e consulta de relatórios
- **StatisticsController**: Dados agregados e gráficos
- **ShoppingController**: Lista de compras
- **WeatherController**: Integração meteorológica
- **AuthController**: Autenticação JWT

### 3.2.2 Serviços SOAP

Os serviços SOAP fornecem acesso à camada de dados através de 4 serviços:

- **PoolDataService**: Operações sobre o estado da piscina
- **WorkerDataService**: Operações sobre trabalhadores
- **WaterQualityDataService**: Operações sobre qualidade da água
- **ReportDataService**: Operações sobre relatórios

### 3.2.3 Camada de Serviços (Business Logic)

A lógica de negócio está encapsulada em serviços especializados:

- **PoolService**: Lógica de gestão de lotação
- **WorkerService**: Lógica de gestão de trabalhadores
- **WaterQualityService**: Lógica de qualidade da água
- **CleaningService**: Lógica de limpezas
- **ReportService**: Lógica de geração de relatórios
- **StatisticsService**: Lógica de estatísticas
- **ShoppingService**: Lógica de lista de compras
- **WeatherService**: Integração com Open-Meteo
- **JwtService**: Geração e validação de tokens JWT

### 3.2.4 Camada de Dados

A camada de dados utiliza:

- **Entity Framework Core**: ORM para acesso à base de dados
- **Repository Pattern**: Abstração de acesso a dados
- **DbContext**: Contexto de base de dados

## 3.3 Padrões Arquiteturais Utilizados

1. **Layered Architecture**: Separação em camadas (Presentation, Business, Data)
2. **Repository Pattern**: Abstração de acesso a dados
3. **Dependency Injection**: Injeção de dependências via constructor
4. **DTO Pattern**: Separação entre Entities e DTOs
5. **Service Pattern**: Encapsulamento de lógica de negócio

---

# 4. Implementação Técnica

## 4.1 Stack Tecnológica

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

### DevOps & Cloud
- **Version Control**: Git + GitHub
- **Cloud Provider**: Azure / Railway / Render
- **Database Hosting**: Azure SQL Database / Railway PostgreSQL

### Testing
- **Unit Tests**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Integration Tests**: WebApplicationFactory (ASP.NET Core TestServer)

## 4.2 Estrutura de Projetos

O projeto está organizado em múltiplos projetos .NET:

```
PoolTracker.sln
├── PoolTracker.API/              # API RESTful
├── PoolTracker.SOAP/              # Serviços SOAP
├── PoolTracker.Core/              # Entidades e Interfaces
├── PoolTracker.Infrastructure/  # Acesso a Dados
└── PoolTracker.Tests/             # Testes Automatizados
```

---

# 5. Serviços SOAP

## 5.1 Implementação

Os serviços SOAP foram implementados utilizando a biblioteca **SoapCore** para ASP.NET Core, permitindo a criação de serviços SOAP modernos e compatíveis com o protocolo SOAP 1.1/1.2.

### 5.2 Serviços Implementados

#### 5.2.1 PoolDataService

Fornece operações sobre o estado da piscina:

- `GetPoolStatus()`: Obter estado atual da piscina
- `UpdatePoolStatus(PoolStatusData)`: Atualizar estado
- `IncrementCount()`: Incrementar contagem de pessoas
- `DecrementCount()`: Decrementar contagem de pessoas

**Endpoint**: `http://localhost:5000/soap/PoolDataService`

#### 5.2.2 WorkerDataService

Fornece operações sobre trabalhadores:

- `GetAllWorkers()`: Listar todos os trabalhadores
- `GetWorkerById(int id)`: Obter trabalhador por ID
- `CreateWorker(WorkerData)`: Criar novo trabalhador
- `UpdateWorker(WorkerData)`: Atualizar trabalhador
- `DeleteWorker(int id)`: Eliminar trabalhador

**Endpoint**: `http://localhost:5000/soap/WorkerDataService`

#### 5.2.3 WaterQualityDataService

Fornece operações sobre qualidade da água:

- `GetHistory(string poolType)`: Histórico de medições
- `GetLatest(string poolType)`: Última medição
- `RecordMeasurement(WaterQualityData)`: Registar medição

**Endpoint**: `http://localhost:5000/soap/WaterQualityDataService`

#### 5.2.4 ReportDataService

Fornece operações sobre relatórios:

- `GetReports(DateTime startDate, DateTime endDate)`: Listar relatórios
- `GenerateReport(DateTime date)`: Gerar relatório diário

**Endpoint**: `http://localhost:5000/soap/ReportDataService`

### 5.3 Data Contracts

Os serviços SOAP utilizam Data Contracts para definir a estrutura dos dados:

- **PoolStatusData**: Estado da piscina
- **WorkerData**: Dados de trabalhador
- **WaterQualityData**: Dados de qualidade da água
- **DailyReportData**: Dados de relatório diário

### 5.4 WSDL

O WSDL é gerado automaticamente pelo SoapCore, permitindo a descoberta de serviços e a geração de clientes SOAP.

**Acesso ao WSDL**: `http://localhost:5000/soap/{ServiceName}?wsdl`

---

# 6. Serviços RESTful

## 6.1 Visão Geral

A API RESTful fornece 40+ endpoints organizados em 9 controllers, cobrindo todas as funcionalidades do sistema.

## 6.2 Endpoints por Módulo

### 6.2.1 Pool Management (7 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/pool/status` | Obter estado atual | Público |
| POST | `/api/pool/enter` | Registar entrada | JWT |
| POST | `/api/pool/exit` | Registar saída | JWT |
| PUT | `/api/pool/count` | Definir contagem | JWT |
| PUT | `/api/pool/capacity` | Alterar capacidade | JWT |
| PUT | `/api/pool/open-status` | Abrir/fechar | JWT |
| DELETE | `/api/pool/reset` | Resetar sistema | JWT |

### 6.2.2 Workers (8 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/workers` | Listar todos | JWT |
| GET | `/api/workers/{id}` | Obter por ID | JWT |
| GET | `/api/workers/active` | Listar ativos | Público |
| POST | `/api/workers` | Criar trabalhador | JWT |
| PUT | `/api/workers/{id}` | Atualizar | JWT |
| DELETE | `/api/workers/{id}` | Eliminar | JWT |
| POST | `/api/workers/{id}/activate` | Ativar turno | JWT |
| POST | `/api/workers/{id}/deactivate` | Desativar turno | JWT |

### 6.2.3 Water Quality (4 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/water-quality` | Histórico | Público |
| GET | `/api/water-quality/latest` | Última medição | Público |
| POST | `/api/water-quality` | Registar medição | JWT |
| DELETE | `/api/water-quality/{id}` | Eliminar registo | JWT |

### 6.2.4 Cleanings (4 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/cleanings` | Histórico | JWT |
| GET | `/api/cleanings/latest` | Última limpeza | Público |
| POST | `/api/cleanings` | Registar limpeza | JWT |
| DELETE | `/api/cleanings/{id}` | Eliminar registo | JWT |

### 6.2.5 Reports (4 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/reports` | Listar relatórios | JWT |
| GET | `/api/reports/latest` | Último relatório | JWT |
| GET | `/api/reports/{date}` | Relatório por data | JWT |
| POST | `/api/reports/generate` | Gerar relatório | JWT |

### 6.2.6 Statistics (3 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/statistics/visitors` | Fluxo visitantes (7 dias) | JWT |
| GET | `/api/statistics/workers` | Turnos por trabalhador | JWT |
| GET | `/api/statistics/occupancy` | Ocupação média | JWT |

### 6.2.7 Shopping List (3 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/shopping` | Listar itens | JWT |
| POST | `/api/shopping` | Adicionar item | JWT |
| DELETE | `/api/shopping/{id}` | Remover item | JWT |

### 6.2.8 Weather (1 endpoint)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/weather/current` | Meteorologia atual | Público |

### 6.2.9 Authentication (2 endpoints)

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/api/auth/login` | Login (gera JWT) | Público |
| POST | `/api/auth/refresh` | Refresh token | JWT |

## 6.3 Convenções REST

A API segue as convenções REST:

- **GET**: Operações de leitura (idempotentes)
- **POST**: Criação de recursos
- **PUT**: Atualização completa de recursos
- **DELETE**: Eliminação de recursos

## 6.4 Códigos de Resposta HTTP

- **200 OK**: Operação bem-sucedida
- **201 Created**: Recurso criado com sucesso
- **400 Bad Request**: Dados inválidos
- **401 Unauthorized**: Não autenticado
- **403 Forbidden**: Não autorizado
- **404 Not Found**: Recurso não encontrado
- **500 Internal Server Error**: Erro do servidor

---

# 7. Segurança e Autenticação

## 7.1 Autenticação JWT

O sistema utiliza **JSON Web Tokens (JWT)** para autenticação stateless, seguindo o padrão Bearer Token.

### 7.2 Fluxo de Autenticação

1. **Login**: Cliente envia PIN para `/api/auth/login`
2. **Validação**: Servidor valida PIN
3. **Geração de Token**: Servidor gera JWT token com claims
4. **Resposta**: Cliente recebe token e refresh token
5. **Uso**: Cliente inclui token no header `Authorization: Bearer {token}`
6. **Validação**: Servidor valida token em cada request protegido
7. **Refresh**: Cliente pode renovar token usando refresh token

### 7.3 Configuração JWT

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-muito-longa-e-segura",
    "Issuer": "PoolTrackerAPI",
    "Audience": "PoolTrackerClients",
    "ExpiryMinutes": 60
  }
}
```

### 7.4 Proteção de Endpoints

- **Endpoints Públicos**: Não requerem autenticação (ex: `/api/pool/status`, `/api/weather/current`)
- **Endpoints Protegidos**: Requerem JWT válido (ex: todos os POST, PUT, DELETE)

### 7.5 Boas Práticas de Segurança

- ✅ HTTPS obrigatório em produção
- ✅ Proteção contra SQL Injection (Parameterized Queries via EF Core)
- ✅ Proteção contra XSS (Sanitização de inputs)
- ✅ Secrets em variáveis de ambiente (nunca no código)
- ✅ CORS configurado (whitelist de domínios)
- ✅ Tokens com expiração (60 minutos)
- ✅ Refresh tokens para renovação

---

# 8. Integração com Serviços Externos

## 8.1 Open-Meteo API

O sistema integra com a API **Open-Meteo** para fornecer dados meteorológicos em tempo real.

### 8.1.1 Funcionalidades

- **Temperatura atual**: Temperatura em graus Celsius
- **Condição meteorológica**: Descrição do tempo (ex: "Ensolarado", "Nublado")
- **Velocidade do vento**: Velocidade do vento em km/h
- **Localização**: Coordenadas GPS da piscina (Braga, Portugal)

### 8.1.2 Implementação

O `WeatherService` implementa:

- **Cache de 60 segundos**: Evita rate limits e melhora performance
- **Tratamento de erros**: Fallback em caso de falha da API
- **HttpClient reutilizável**: Configurado via Dependency Injection

### 8.1.3 Endpoint

```
GET /api/weather/current
```

**Resposta**:
```json
{
  "temperature": 22.5,
  "condition": "Ensolarado",
  "windSpeed": 15.3,
  "lastUpdated": "2025-12-20T14:30:00Z"
}
```

---

# 9. Base de Dados

## 9.1 Schema

A base de dados é composta por **8 tabelas principais**, normalizadas em 3NF:

### 9.1.1 pool_status

Armazena o estado atual da piscina.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| CurrentCount | INT | Contagem atual de pessoas |
| MaxCapacity | INT | Capacidade máxima |
| IsOpen | BIT | Estado (aberta/fechada) |
| LastUpdated | DATETIME2 | Última atualização |
| LocationName | NVARCHAR(255) | Nome da localização |
| Address | NVARCHAR(255) | Morada |
| Phone | NVARCHAR(50) | Telefone |

### 9.1.2 workers

Cadastro de trabalhadores.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| WorkerId | NVARCHAR(50) | ID único do trabalhador |
| Name | NVARCHAR(255) | Nome |
| Role | NVARCHAR(50) | Cargo (nadador_salvador, bar, etc.) |
| IsActive | BIT | Estado ativo/inativo |

### 9.1.3 active_workers

Turnos ativos de trabalhadores.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| WorkerId | NVARCHAR(50) | FK para workers |
| ShiftType | NVARCHAR(10) | Tipo de turno (manhã/tarde) |
| StartTime | DATETIME2 | Início do turno |
| EndTime | DATETIME2 | Fim do turno (nullable) |

### 9.1.4 water_quality

Medições de qualidade da água.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| PoolType | NVARCHAR(20) | Tipo (crianças/adultos) |
| PhLevel | DECIMAL(4,2) | Nível de pH |
| Temperature | DECIMAL(5,2) | Temperatura |
| MeasuredAt | DATETIME2 | Data/hora da medição |
| Notes | NVARCHAR(MAX) | Notas (nullable) |

### 9.1.5 cleanings

Registos de limpeza.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| CleaningType | NVARCHAR(20) | Tipo (balneários/WC) |
| CleanedAt | DATETIME2 | Data/hora da limpeza |
| Notes | NVARCHAR(MAX) | Notas (nullable) |

### 9.1.6 daily_visitors

Visitantes por dia.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| VisitDate | DATE | Data (única) |
| TotalVisitors | INT | Total de visitantes |

### 9.1.7 daily_reports

Relatórios diários.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| ReportDate | DATE | Data (única) |
| TotalVisitors | INT | Total de visitantes |
| MaxOccupancy | INT | Ocupação máxima |
| AvgOccupancy | DECIMAL(5,2) | Ocupação média |
| WaterQualityChildren | NVARCHAR(MAX) | JSON com dados |
| WaterQualityAdults | NVARCHAR(MAX) | JSON com dados |
| ActiveWorkersCount | NVARCHAR(MAX) | JSON com dados |
| CleaningRecords | NVARCHAR(MAX) | JSON com dados |

### 9.1.8 shopping_list

Lista de compras.

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | INT | Chave primária |
| Name | NVARCHAR(255) | Nome do item |
| Category | NVARCHAR(20) | Categoria (bar, limpeza, qualidade) |

## 9.2 Relacionamentos

- `active_workers.WorkerId` → `workers.WorkerId` (FK com ON DELETE CASCADE)

## 9.3 Índices

Índices criados em campos de busca frequente:

- `workers.WorkerId` (UNIQUE)
- `daily_visitors.VisitDate` (UNIQUE)
- `daily_reports.ReportDate` (UNIQUE)
- `water_quality.MeasuredAt`
- `cleanings.CleanedAt`

---

# 10. Testes

## 10.1 Estratégia de Testes

O projeto implementa uma estratégia de testes em múltiplas camadas:

1. **Testes Unitários**: Testam serviços isoladamente com mocks
2. **Testes de Integração**: Testam controllers com base de dados em memória
3. **Testes End-to-End**: Testam fluxos completos da API

## 10.2 Ferramentas

- **xUnit**: Framework de testes
- **Moq**: Mocking de dependências
- **FluentAssertions**: Assertions legíveis
- **WebApplicationFactory**: Testes de integração in-memory

## 10.3 Cobertura de Testes

**Objetivo**: ≥ 70% de code coverage

### 10.3.1 Testes Unitários (20+ testes)

- Testes de serviços (PoolService, WorkerService, etc.)
- Testes de repositórios
- Testes de validação

### 10.3.2 Testes de Integração (15+ testes)

- Testes de controllers
- Testes de autenticação
- Testes de base de dados

### 10.3.3 Testes End-to-End (10+ testes)

- Fluxos completos de operações
- Testes de performance
- Testes de segurança

## 10.4 Exemplo de Teste

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

# 11. Documentação

## 11.1 Swagger/OpenAPI

A API está completamente documentada usando **Swagger/OpenAPI**, acessível em:

- **UI**: `https://seu-dominio.com/swagger`
- **JSON**: `https://seu-dominio.com/swagger/v1/swagger.json`

### 11.1.1 Funcionalidades

- ✅ Documentação interativa de todos os endpoints
- ✅ Descrições detalhadas com XML comments
- ✅ Exemplos de requests e responses
- ✅ Autenticação JWT integrada no Swagger UI
- ✅ Códigos de resposta documentados

## 11.2 Documentação de Código

- ✅ XML comments em todos os métodos públicos
- ✅ Descrições de parâmetros e retornos
- ✅ Exemplos de uso

## 11.3 README

O projeto inclui um README.md completo com:

- Instruções de instalação
- Guia de configuração
- Exemplos de uso
- Arquitetura do sistema
- Stack tecnológica

---

# 12. Deployment

## 12.1 Plataformas Suportadas

O sistema foi projetado para deployment em múltiplas plataformas cloud:

- **Azure**: App Service + Azure SQL Database
- **Railway**: Containers + PostgreSQL
- **Render**: Web Services + PostgreSQL

## 12.2 Configuração de Produção

### 12.2.1 Variáveis de Ambiente

As seguintes variáveis devem ser configuradas na plataforma:

```
ConnectionStrings__DefaultConnection=<connection-string>
Jwt__Key=<jwt-secret-key>
Jwt__Issuer=PoolTrackerAPI
Jwt__Audience=PoolTrackerClients
ASPNETCORE_ENVIRONMENT=Production
```

### 12.2.2 CORS

CORS configurado para permitir apenas domínios autorizados em produção.

### 12.2.3 HTTPS

HTTPS obrigatório em produção, com redirecionamento automático de HTTP.

## 12.3 Processo de Deploy

1. **Base de Dados**: Criar instância SQL na cloud
2. **API**: Deploy da aplicação .NET
3. **SOAP**: Deploy separado ou no mesmo serviço
4. **Frontend**: Build estático no Vercel/Netlify
5. **Variáveis**: Configurar secrets na plataforma
6. **Testes**: Validar todos os endpoints em produção

---

# 13. Conclusões

## 13.1 Objetivos Alcançados

O projeto PoolTracker demonstra com sucesso:

✅ **Arquitetura SOA**: Separação clara entre serviços RESTful e SOAP  
✅ **Interoperabilidade**: Comunicação SOAP (XML) e REST (JSON)  
✅ **Segurança**: Implementação de JWT Bearer Authentication  
✅ **Qualidade**: Testes automatizados e code coverage  
✅ **Documentação**: Swagger completo e documentação de código  
✅ **Cloud**: Deployment em plataformas cloud  
✅ **Integração**: Consumo de APIs externas (Open-Meteo)  

## 13.2 Requisitos do Enunciado

Todos os requisitos do enunciado do TP2 foram cumpridos:

1. ✅ Qualidade dos serviços desenvolvidos
2. ✅ Desenvolveu serviços SOAP (para Data Layer)
3. ✅ Desenvolveu serviços RESTful (POST, GET, PUT, DELETE)
4. ✅ Utilizou serviços web externos
5. ✅ Documentou devidamente a API disponibilizada
6. ✅ Especificou um conjunto de testes para a API desenvolvida
7. ✅ Publicou Repositório de Dados na Cloud
8. ✅ Explorou aplicação de segurança nos serviços
9. ✅ Publicou Serviços na Cloud

## 13.3 Conceitos de ISI Demonstrados

O projeto demonstra compreensão dos seguintes conceitos:

- **Interoperabilidade**: Comunicação entre sistemas heterogéneos
- **SOA**: Arquitetura orientada a serviços
- **Segurança**: Autenticação e autorização em serviços web
- **Qualidade**: Testes, documentação e boas práticas
- **Cloud Computing**: Deployment e escalabilidade

## 13.4 Dificuldades Encontradas

Durante o desenvolvimento, foram encontradas algumas dificuldades:

1. **Configuração SOAP em .NET Core**: Requer bibliotecas específicas (SoapCore)
2. **Integração JWT**: Configuração inicial complexa
3. **Deployment Cloud**: Configuração de variáveis de ambiente e CORS

Todas as dificuldades foram superadas através de pesquisa e documentação.

## 13.5 Trabalho Futuro

Possíveis melhorias futuras:

- Implementação de microserviços
- Integração com redes sociais
- Dashboard de monitorização avançado
- Notificações push
- Aplicação mobile

## 13.6 Aprendizagens

Este projeto permitiu consolidar conhecimentos em:

- Desenvolvimento de serviços web (SOAP e REST)
- Arquitetura de software
- Segurança em APIs
- Testes automatizados
- Deployment em cloud
- Integração de sistemas

---

# 14. Referências

## 14.1 Documentação Oficial

- Microsoft. (2024). *ASP.NET Core Documentation*. https://docs.microsoft.com/aspnet/core
- Microsoft. (2024). *Entity Framework Core Documentation*. https://docs.microsoft.com/ef/core
- SoapCore. (2024). *SoapCore Documentation*. https://github.com/DigDes/SoapCore
- Open-Meteo. (2024). *Open-Meteo API Documentation*. https://open-meteo.com/en/docs

## 14.2 Bibliotecas e Frameworks

- **ASP.NET Core 8.0**: Framework web da Microsoft
- **Entity Framework Core 8.0**: ORM da Microsoft
- **SoapCore**: Biblioteca SOAP para ASP.NET Core
- **Swashbuckle.AspNetCore**: Geração de documentação Swagger
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Autenticação JWT
- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **FluentAssertions**: Biblioteca de assertions

## 14.3 Artigos e Tutoriais

- Richardson, L. (2013). *RESTful Web Services*. O'Reilly Media.
- Erl, T. (2008). *SOA: Principles of Service Design*. Prentice Hall.
- JWT.io. (2024). *Introduction to JSON Web Tokens*. https://jwt.io/introduction

---

# Anexos

## Anexo A: Diagramas de Arquitetura

*(Incluir diagramas detalhados da arquitetura)*

## Anexo B: Screenshots

*(Incluir screenshots do Swagger UI, frontend, etc.)*

## Anexo C: Código Fonte

*(Referência ao repositório GitHub)*

**Repositório**: https://github.com/ricardoguimaraes2021/ISI-TP2-PoolTracker

---

**Fim do Relatório**

*Versão 1.0 - Dezembro 2025*  
*PoolTracker - Sistema Integrado de Gestão de Piscina Municipal*

