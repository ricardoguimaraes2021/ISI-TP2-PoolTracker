# Plano de Implementação: PoolTracker .NET Completo

## Objetivo

Expandir o projeto **PoolTracker .NET** com todas as funcionalidades da versão PHP, cumprindo **todos os requisitos** do enunciado TP2 de ISI.

---

## ✅ Requisitos do Enunciado (Checklist)

- [ ] **Qualidade dos serviços desenvolvidos**
- [ ] **Desenvolveu serviços SOAP (para Data Layer)**
- [ ] **Desenvolveu serviços RESTful (POST, GET, PUT, DELETE)**
- [ ] **Utilizou serviços web externos**
- [ ] **Documentou devidamente a API disponibilizada (Swagger/OpenAPI)**
- [ ] **Especificou um conjunto de testes para a API desenvolvida**
- [ ] **Publicou Repositório de Dados na Cloud**
- [ ] **Explorou aplicação de segurança nos serviços (OAuth/JWT)**
- [ ] **Publicou Serviços na Cloud**

---

## 🏗️ Arquitetura Proposta

```
PoolTracker.Solution/
├── PoolTracker.API/                    # RESTful API (ASP.NET Core)
│   ├── Controllers/                    # REST Controllers
│   ├── Services/                       # Business Logic
│   ├── Data/                          # Entity Framework DbContext
│   ├── Models/                        # Domain Models
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Middleware/                    # Auth Middleware
│   └── Program.cs
│
├── PoolTracker.SOAP/                   # SOAP Services (WCF)
│   ├── Services/                      # SOAP Service Implementations
│   ├── Contracts/                     # Service Contracts (Interfaces)
│   ├── DataContracts/                 # Data Contracts
│   └── Program.cs
│
├── PoolTracker.Core/                   # Shared Library
│   ├── Entities/                      # Database Entities
│   ├── Interfaces/                    # Repository Interfaces
│   └── DTOs/                          # Shared DTOs
│
├── PoolTracker.Infrastructure/         # Data Access Layer
│   ├── Data/                          # DbContext
│   ├── Repositories/                  # Repository Implementations
│   └── Migrations/                    # EF Core Migrations
│
├── PoolTracker.Tests/                  # Unit & Integration Tests
│   ├── UnitTests/
│   ├── IntegrationTests/
│   └── ApiTests/
│
└── pooltracker-web/                    # React Frontend
    └── src/
```

---

## 🗄️ Estrutura da Base de Dados

### Tabelas (Migradas da versão PHP)

```sql
-- 1. pool_status
CREATE TABLE pool_status (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CurrentCount INT NOT NULL DEFAULT 0,
    MaxCapacity INT NOT NULL DEFAULT 120,
    IsOpen BIT NOT NULL DEFAULT 1,
    LastUpdated DATETIME2 NOT NULL DEFAULT GETDATE(),
    LocationName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 2. workers
CREATE TABLE workers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    WorkerId NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL, -- nadador_salvador, bar, vigilante, bilheteira
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 3. active_workers
CREATE TABLE active_workers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    WorkerId NVARCHAR(50) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    ShiftType NVARCHAR(10) NULL, -- manha, tarde
    StartTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    EndTime DATETIME2 NULL,
    FOREIGN KEY (WorkerId) REFERENCES workers(WorkerId) ON DELETE CASCADE
);

-- 4. water_quality
CREATE TABLE water_quality (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PoolType NVARCHAR(20) NOT NULL, -- criancas, adultos
    PhLevel DECIMAL(4,2) NOT NULL,
    Temperature DECIMAL(5,2) NOT NULL,
    MeasuredAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) NULL
);

-- 5. cleanings
CREATE TABLE cleanings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CleaningType NVARCHAR(20) NOT NULL, -- balnearios, wc
    CleanedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    Notes NVARCHAR(MAX) NULL
);

-- 6. daily_visitors
CREATE TABLE daily_visitors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VisitDate DATE NOT NULL UNIQUE,
    TotalVisitors INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 7. daily_reports
CREATE TABLE daily_reports (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ReportDate DATE NOT NULL UNIQUE,
    TotalVisitors INT NOT NULL DEFAULT 0,
    MaxOccupancy INT NOT NULL DEFAULT 0,
    AvgOccupancy DECIMAL(5,2) NULL,
    OpeningTime DATETIME2 NULL,
    ClosingTime DATETIME2 NOT NULL,
    WaterQualityChildren NVARCHAR(MAX) NULL, -- JSON
    WaterQualityAdults NVARCHAR(MAX) NULL,   -- JSON
    ActiveWorkersCount NVARCHAR(MAX) NULL,   -- JSON
    CleaningRecords NVARCHAR(MAX) NULL,      -- JSON
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 8. shopping_list
CREATE TABLE shopping_list (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Category NVARCHAR(20) NOT NULL, -- bar, limpeza, qualidade
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 🔌 API RESTful - Endpoints Completos

### Pool Management
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/pool/status` | Obter estado atual | Público |
| POST | `/api/pool/enter` | Registar entrada | JWT |
| POST | `/api/pool/exit` | Registar saída | JWT |
| PUT | `/api/pool/count` | Definir contagem | JWT |
| PUT | `/api/pool/capacity` | Alterar capacidade | JWT |
| PUT | `/api/pool/open-status` | Abrir/fechar | JWT |
| DELETE | `/api/pool/reset` | Resetar sistema | JWT |

### Workers
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/workers` | Listar todos | JWT |
| GET | `/api/workers/{id}` | Obter por ID | JWT |
| GET | `/api/workers/active` | Listar ativos | Público |
| POST | `/api/workers` | Criar trabalhador | JWT |
| PUT | `/api/workers/{id}` | Atualizar | JWT |
| DELETE | `/api/workers/{id}` | Eliminar | JWT |
| POST | `/api/workers/{id}/activate` | Ativar turno | JWT |
| POST | `/api/workers/{id}/deactivate` | Desativar turno | JWT |

### Water Quality
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/water-quality` | Histórico | Público |
| GET | `/api/water-quality/latest` | Última medição | Público |
| POST | `/api/water-quality` | Registar medição | JWT |
| DELETE | `/api/water-quality/{id}` | Eliminar registo | JWT |

### Cleanings
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/cleanings` | Histórico | JWT |
| GET | `/api/cleanings/latest` | Última limpeza | Público |
| POST | `/api/cleanings` | Registar limpeza | JWT |
| DELETE | `/api/cleanings/{id}` | Eliminar registo | JWT |

### Reports
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/reports` | Listar relatórios | JWT |
| GET | `/api/reports/latest` | Último relatório | JWT |
| GET | `/api/reports/{date}` | Relatório por data | JWT |
| POST | `/api/reports/generate` | Gerar relatório | JWT |

### Statistics
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/statistics/visitors` | Fluxo visitantes (7 dias) | JWT |
| GET | `/api/statistics/workers` | Turnos por trabalhador | JWT |
| GET | `/api/statistics/occupancy` | Ocupação média | JWT |

### Shopping List
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/shopping` | Listar itens | JWT |
| POST | `/api/shopping` | Adicionar item | JWT |
| DELETE | `/api/shopping/{id}` | Remover item | JWT |

### Weather (Externo)
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/weather/current` | Meteorologia atual | Público |

### Authentication
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Login (gera JWT) | Público |
| POST | `/api/auth/refresh` | Refresh token | JWT |

---

## 🧼 Serviços SOAP (Data Layer)

### Contratos de Serviço

```csharp
[ServiceContract]
public interface IPoolDataService
{
    [OperationContract]
    PoolStatusData GetPoolStatus();
    
    [OperationContract]
    void UpdatePoolStatus(PoolStatusData status);
    
    [OperationContract]
    void IncrementCount();
    
    [OperationContract]
    void DecrementCount();
}

[ServiceContract]
public interface IWorkerDataService
{
    [OperationContract]
    List<WorkerData> GetAllWorkers();
    
    [OperationContract]
    WorkerData GetWorkerById(int id);
    
    [OperationContract]
    int CreateWorker(WorkerData worker);
    
    [OperationContract]
    void UpdateWorker(WorkerData worker);
    
    [OperationContract]
    void DeleteWorker(int id);
}

[ServiceContract]
public interface IWaterQualityDataService
{
    [OperationContract]
    List<WaterQualityData> GetHistory(string poolType);
    
    [OperationContract]
    WaterQualityData GetLatest(string poolType);
    
    [OperationContract]
    void RecordMeasurement(WaterQualityData measurement);
}

[ServiceContract]
public interface IReportDataService
{
    [OperationContract]
    List<DailyReportData> GetReports(DateTime startDate, DateTime endDate);
    
    [OperationContract]
    DailyReportData GenerateReport(DateTime date);
}
```

### Endpoints SOAP
- `http://localhost:5000/soap/PoolDataService`
- `http://localhost:5000/soap/WorkerDataService`
- `http://localhost:5000/soap/WaterQualityDataService`
- `http://localhost:5000/soap/ReportDataService`

---

## 🔐 Segurança - OAuth/JWT

### Implementação

```csharp
// Configuração JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

### Fluxo de Autenticação

1. **Login**: `POST /api/auth/login` com PIN
2. **Resposta**: JWT token + refresh token
3. **Uso**: Header `Authorization: Bearer {token}`
4. **Refresh**: `POST /api/auth/refresh` quando expirar

### Configuração (appsettings.json)

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-muito-longa-e-segura-aqui",
    "Issuer": "PoolTrackerAPI",
    "Audience": "PoolTrackerClients",
    "ExpiryMinutes": 60
  }
}
```

---

## 📚 Documentação API - Swagger/OpenAPI

### Configuração

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PoolTracker API",
        Version = "v1",
        Description = "API completa para gestão de piscina municipal",
        Contact = new OpenApiContact
        {
            Name = "Ricardo Guimarães",
            Email = "ricardo@example.com"
        }
    });
    
    // JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PoolTracker API v1");
    options.RoutePrefix = "swagger";
});
```

### Endpoint Swagger
- **UI**: `https://seu-dominio.com/swagger`
- **JSON**: `https://seu-dominio.com/swagger/v1/swagger.json`

---

## 🧪 Testes Automatizados

### Estrutura de Testes

```
PoolTracker.Tests/
├── UnitTests/
│   ├── Services/
│   │   ├── PoolServiceTests.cs
│   │   ├── WorkerServiceTests.cs
│   │   ├── WaterQualityServiceTests.cs
│   │   └── ReportServiceTests.cs
│   └── Repositories/
│       └── PoolRepositoryTests.cs
│
├── IntegrationTests/
│   ├── Controllers/
│   │   ├── PoolControllerTests.cs
│   │   ├── WorkerControllerTests.cs
│   │   └── AuthControllerTests.cs
│   └── Database/
│       └── DatabaseIntegrationTests.cs
│
└── ApiTests/
    ├── PoolApiTests.cs
    ├── WorkerApiTests.cs
    └── AuthenticationTests.cs
```

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
    Assert.Equal(1, result.CurrentCount);
    _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<PoolStatus>()), Times.Once);
}

[Fact]
public async Task GetPoolStatus_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/pool/status");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Assert.Contains("currentCount", content);
}
```

### Ferramentas
- **xUnit** - Framework de testes
- **Moq** - Mocking
- **FluentAssertions** - Assertions legíveis
- **WebApplicationFactory** - Testes de integração

---

## ☁️ Publicação na Cloud

### Opções de Plataforma

#### Opção 1: Azure (Recomendado)
- **API**: Azure App Service
- **SOAP**: Azure App Service (separado)
- **Database**: Azure SQL Database
- **Frontend**: Azure Static Web Apps

#### Opção 2: Railway
- **API + SOAP**: Railway (containers)
- **Database**: Railway PostgreSQL
- **Frontend**: Vercel/Netlify

#### Opção 3: Render
- **API + SOAP**: Render Web Services
- **Database**: Render PostgreSQL
- **Frontend**: Render Static Site

### Configuração Azure (Exemplo)

```bash
# Login
az login

# Criar Resource Group
az group create --name PoolTrackerRG --location westeurope

# Criar SQL Database
az sql server create --name pooltracker-sql --resource-group PoolTrackerRG --location westeurope --admin-user sqladmin --admin-password SuaPassword123!
az sql db create --resource-group PoolTrackerRG --server pooltracker-sql --name PoolTrackerDB --service-objective S0

# Criar App Service Plan
az appservice plan create --name PoolTrackerPlan --resource-group PoolTrackerRG --sku B1 --is-linux

# Criar Web Apps
az webapp create --resource-group PoolTrackerRG --plan PoolTrackerPlan --name pooltracker-api --runtime "DOTNETCORE:8.0"
az webapp create --resource-group PoolTrackerRG --plan PoolTrackerPlan --name pooltracker-soap --runtime "DOTNETCORE:8.0"

# Deploy
dotnet publish -c Release
az webapp deployment source config-zip --resource-group PoolTrackerRG --name pooltracker-api --src api.zip
```

---

## 📋 Plano de Implementação Faseado

### Fase 1: Fundação (Semana 1)
**Tempo estimado**: 10-12 horas
**Status**: ✅ COMPLETA

- [x] Criar estrutura de projetos (API, SOAP, Core, Infrastructure, Tests)
- [x] Configurar Entity Framework Core
- [x] Criar DbContext e Entities
- [x] Criar migrations e aplicar schema
- [x] Configurar connection strings
- [x] Implementar Repository Pattern
- [x] Configurar Dependency Injection

### Fase 2: RESTful API Core (Semana 1-2)
**Tempo estimado**: 8-10 horas
**Status**: ✅ COMPLETA

- [x] Migrar PoolController para usar EF Core
- [x] Implementar WorkerController + Service
- [x] Implementar WaterQualityController + Service
- [x] Implementar CleaningController + Service
- [x] Implementar VisitService (integrado no PoolService)
- [x] Adicionar operação DELETE em todos os controllers
- [x] Validação de DTOs

### Fase 3: Funcionalidades Avançadas (Semana 2)
**Tempo estimado**: 6-8 horas
**Status**: ✅ COMPLETA

- [x] Implementar ReportController + Service
- [x] Lógica de geração automática de relatórios
- [x] StatisticsController para gráficos
- [x] ShoppingListController
- [x] Integração com Open-Meteo (WeatherService implementado)

### Fase 4: Autenticação JWT (Semana 2)
**Tempo estimado**: 4-6 horas
**Status**: ✅ COMPLETA

- [x] Criar AuthController
- [x] Implementar geração de JWT tokens
- [x] Configurar JWT Bearer Authentication
- [x] Adicionar `[Authorize]` nos endpoints protegidos
- [x] Implementar refresh tokens
- [ ] Atualizar frontend para usar JWT (pendente Fase 8)

### Fase 5: Serviços SOAP (Semana 3)
**Tempo estimado**: 8-10 horas
**Status**: 🚧 EM PROGRESSO

- [x] Criar projeto PoolTracker.SOAP
- [ ] Definir Service Contracts
- [ ] Definir Data Contracts
- [ ] Implementar PoolDataService
- [ ] Implementar WorkerDataService
- [ ] Implementar WaterQualityDataService
- [ ] Implementar ReportDataService
- [ ] Configurar endpoints SOAP
- [ ] Gerar WSDL

### Fase 6: Documentação Swagger (Semana 3)
**Tempo estimado**: 3-4 horas

- [ ] Configurar Swashbuckle
- [ ] Adicionar XML comments em todos os endpoints
- [ ] Configurar autenticação JWT no Swagger
- [ ] Adicionar exemplos de requests/responses
- [ ] Testar UI do Swagger

### Fase 7: Testes Automatizados (Semana 3-4)
**Tempo estimado**: 10-12 horas

- [ ] Configurar projeto de testes (xUnit)
- [ ] Testes unitários de Services (mínimo 20 testes)
- [ ] Testes de integração de Controllers (mínimo 15 testes)
- [ ] Testes de API end-to-end (mínimo 10 testes)
- [ ] Testes de autenticação
- [ ] Configurar code coverage

### Fase 8: Frontend Expandido (Semana 4)
**Tempo estimado**: 8-10 horas

- [ ] Migrar componentes Shadcn/ui da versão PHP
- [ ] Instalar e configurar Recharts
- [ ] Criar páginas de gestão de trabalhadores
- [ ] Criar páginas de qualidade da água
- [ ] Criar dashboard de relatórios com gráficos
- [ ] Implementar autenticação JWT no frontend
- [ ] Adicionar React Hot Toast

### Fase 9: Deploy na Cloud (Semana 4)
**Tempo estimado**: 6-8 horas

- [ ] Escolher plataforma (Azure/Railway/Render)
- [ ] Criar base de dados na cloud
- [ ] Configurar variáveis de ambiente
- [ ] Deploy da API RESTful
- [ ] Deploy dos serviços SOAP
- [ ] Deploy do frontend
- [ ] Configurar CORS para produção
- [ ] Testar todos os endpoints em produção

### Fase 10: Documentação e Relatório (Semana 4)
**Tempo estimado**: 4-6 horas

- [ ] Atualizar README.md
- [ ] Documentar arquitetura
- [ ] Criar guia de instalação
- [ ] Documentar endpoints SOAP e REST
- [ ] Preparar relatório final do TP2
- [ ] Screenshots e evidências

---

## ⏱️ Estimativa Total

| Fase | Tempo Estimado |
|------|----------------|
| Fase 1: Fundação | 10-12h |
| Fase 2: RESTful API Core | 8-10h |
| Fase 3: Funcionalidades Avançadas | 6-8h |
| Fase 4: Autenticação JWT | 4-6h |
| Fase 5: Serviços SOAP | 8-10h |
| Fase 6: Documentação Swagger | 3-4h |
| Fase 7: Testes Automatizados | 10-12h |
| Fase 8: Frontend Expandido | 8-10h |
| Fase 9: Deploy na Cloud | 6-8h |
| Fase 10: Documentação | 4-6h |
| **TOTAL** | **67-86 horas** |

**Distribuição sugerida**: 4 semanas, ~20h/semana

---

## 🎯 Alinhamento com Requisitos TP2

| Requisito | Como será cumprido |
|-----------|-------------------|
| **Qualidade dos serviços** | Arquitetura limpa, Repository Pattern, DI, testes |
| **Serviços SOAP** | Projeto PoolTracker.SOAP com 4 serviços (Fase 5) |
| **Serviços RESTful** | 40+ endpoints com GET, POST, PUT, DELETE (Fase 2-3) |
| **Serviços externos** | Open-Meteo (já implementado) |
| **Documentação API** | Swagger UI completo (Fase 6) |
| **Testes** | 45+ testes automatizados (Fase 7) |
| **Repositório na Cloud** | Azure SQL / Railway PostgreSQL (Fase 9) |
| **Segurança** | JWT Bearer Authentication (Fase 4) |
| **Serviços na Cloud** | Azure App Service / Railway (Fase 9) |

---

## 📦 Pacotes NuGet Necessários

### PoolTracker.API
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
```

### PoolTracker.SOAP
```xml
<PackageReference Include="SoapCore" Version="1.1.0" />
<PackageReference Include="System.ServiceModel.Primitives" Version="6.0.0" />
```

### PoolTracker.Tests
```xml
<PackageReference Include="xunit" Version="2.6.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
```

---

## 🚀 Próximos Passos Imediatos

1. **Criar estrutura de projetos** (Fase 1)
2. **Configurar Entity Framework** e criar migrations
3. **Começar implementação dos controllers** (Fase 2)

Deseja que comece a implementação? Posso começar pela Fase 1 (Fundação) criando a estrutura de projetos e configurando o Entity Framework Core.
