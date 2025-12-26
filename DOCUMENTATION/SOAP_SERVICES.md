# 🔌 Serviços SOAP - PoolTracker

## 📚 O que são Serviços SOAP?

**SOAP** (Simple Object Access Protocol) é um protocolo de comunicação baseado em **XML** usado para troca de informações estruturadas entre sistemas. É uma das tecnologias fundamentais para **integração de sistemas** e faz parte da arquitetura **SOA** (Service-Oriented Architecture).

### Características do SOAP

1. **Baseado em XML**: Todas as mensagens são formatadas em XML
2. **Protocolo independente**: Funciona sobre HTTP, HTTPS, SMTP, etc.
3. **Contrato bem definido**: WSDL (Web Services Description Language) descreve os serviços
4. **Padronizado**: Especificação W3C, amplamente suportado
5. **Tipado**: Estruturas de dados bem definidas (Data Contracts)

### Diferenças entre SOAP e REST

| Característica | SOAP | REST |
|----------------|------|------|
| **Formato** | XML | JSON, XML, etc. |
| **Protocolo** | HTTP, SMTP, etc. | Apenas HTTP |
| **Contrato** | WSDL obrigatório | OpenAPI opcional |
| **Estado** | Stateless ou Stateful | Stateless |
| **Uso** | Integração enterprise, sistemas legados | APIs modernas, web/mobile |
| **Complexidade** | Mais complexo | Mais simples |

### Por que usar SOAP?

No contexto do **TP2 de ISI**, os serviços SOAP são obrigatórios porque:

1. ✅ **Demonstra conhecimento** de diferentes protocolos de integração
2. ✅ **Data Layer**: Acesso estruturado à camada de dados
3. ✅ **Interoperabilidade**: Comunicação entre sistemas heterogéneos
4. ✅ **Padrão Enterprise**: Usado em sistemas corporativos e legados
5. ✅ **Contratos explícitos**: WSDL define claramente a interface

---

## 🏗️ Arquitetura dos Serviços SOAP no PoolTracker

### Estrutura do Projeto

```
PoolTracker.SOAP/
├── Contracts/              # Interfaces dos serviços (contratos)
│   ├── IPoolDataService.cs
│   ├── IWorkerDataService.cs
│   ├── IWaterQualityDataService.cs
│   └── IReportDataService.cs
├── Services/              # Implementações dos serviços
│   ├── PoolDataService.cs
│   ├── WorkerDataService.cs
│   ├── WaterQualityDataService.cs
│   └── ReportDataService.cs
├── DataContracts/          # Estruturas de dados (DTOs)
│   ├── PoolStatusData.cs
│   ├── WorkerData.cs
│   ├── WaterQualityData.cs
│   └── DailyReportData.cs
└── Program.cs             # Configuração e endpoints
```

### Fluxo de Dados

```
Cliente SOAP (SoapUI, Postman, etc.)
    │
    │ HTTP POST + XML (SOAP Envelope)
    ▼
┌─────────────────────────────────────┐
│   PoolTracker.SOAP                  │
│   (ASP.NET Core + SoapCore)         │
│                                      │
│   ┌──────────────────────────────┐  │
│   │  SOAP Endpoints:             │  │
│   │  /soap/PoolDataService       │  │
│   │  /soap/WorkerDataService     │  │
│   │  /soap/WaterQualityData...   │  │
│   │  /soap/ReportDataService     │  │
│   └──────────┬───────────────────┘  │
│              │                       │
│   ┌──────────▼───────────────────┐  │
│   │  Services (Business Logic)    │  │
│   └──────────┬───────────────────┘  │
│              │                       │
│   ┌──────────▼───────────────────┐  │
│   │  Repositories (Data Access)  │  │
│   └──────────┬───────────────────┘  │
└──────────────┼───────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   Azure SQL Database                │
│   (pooltracker)                     │
└─────────────────────────────────────┘
```

---

## 🔧 Serviços SOAP Implementados

### 1. PoolDataService

**Contrato**: `IPoolDataService`  
**Endpoint**: `/soap/PoolDataService`  
**WSDL**: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`

#### Métodos Disponíveis

| Método | Descrição | Parâmetros | Retorno |
|--------|-----------|------------|---------|
| `GetPoolStatus()` | Obter estado atual da piscina | Nenhum | `PoolStatusData` |
| `UpdatePoolStatus()` | Atualizar estado da piscina | `PoolStatusData` | `bool` |
| `IncrementCount()` | Incrementar contagem de visitantes | Nenhum | `int` (nova contagem) |
| `DecrementCount()` | Decrementar contagem de visitantes | Nenhum | `int` (nova contagem) |

#### Exemplo de Request (SOAP Envelope)

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetPoolStatus xmlns="http://tempuri.org/">
    </GetPoolStatus>
  </soap:Body>
</soap:Envelope>
```

#### Exemplo de Response

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetPoolStatusResponse xmlns="http://tempuri.org/">
      <GetPoolStatusResult>
        <CurrentCount>42</CurrentCount>
        <MaxCapacity>120</MaxCapacity>
        <IsOpen>true</IsOpen>
        <LocationName>Piscina Municipal da Sobreposta</LocationName>
        <Address>R. da Piscina 22, 4715-553 Sobreposta</Address>
        <Phone>253 636 948</Phone>
      </GetPoolStatusResult>
    </GetPoolStatusResponse>
  </soap:Body>
</soap:Envelope>
```

---

### 2. WorkerDataService

**Contrato**: `IWorkerDataService`  
**Endpoint**: `/soap/WorkerDataService`  
**WSDL**: `https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService?wsdl`

#### Métodos Disponíveis

| Método | Descrição | Parâmetros | Retorno |
|--------|-----------|------------|---------|
| `GetAllWorkers()` | Listar todos os trabalhadores | Nenhum | `WorkerData[]` |
| `GetWorkerById()` | Obter trabalhador por ID | `int id` | `WorkerData` |
| `CreateWorker()` | Criar novo trabalhador | `WorkerData` | `int` (ID criado) |
| `UpdateWorker()` | Atualizar trabalhador | `WorkerData` | `bool` |
| `DeleteWorker()` | Eliminar trabalhador | `int id` | `bool` |

---

### 3. WaterQualityDataService

**Contrato**: `IWaterQualityDataService`  
**Endpoint**: `/soap/WaterQualityDataService`  
**WSDL**: `https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService?wsdl`

#### Métodos Disponíveis

| Método | Descrição | Parâmetros | Retorno |
|--------|-----------|------------|---------|
| `GetHistory()` | Histórico de medições | `string poolType` ("criancas" ou "adultos") | `WaterQualityData[]` |
| `GetLatest()` | Última medição | `string poolType` | `WaterQualityData` |
| `RecordMeasurement()` | Registar nova medição | `WaterQualityData` | `bool` |

---

### 4. ReportDataService

**Contrato**: `IReportDataService`  
**Endpoint**: `/soap/ReportDataService`  
**WSDL**: `https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService?wsdl`

#### Métodos Disponíveis

| Método | Descrição | Parâmetros | Retorno |
|--------|-----------|------------|---------|
| `GetReports()` | Listar relatórios por período | `DateTime startDate`, `DateTime endDate` | `DailyReportData[]` |
| `GenerateReport()` | Gerar relatório diário | `DateTime date` | `DailyReportData` |

---

## 🧪 Como Testar os Serviços SOAP

### Opção 1: SoapUI (Recomendado)

1. **Instalar SoapUI**: https://www.soapui.org/downloads/soapui.html
2. **Criar novo projeto SOAP**:
   - File → New SOAP Project
   - WSDL: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
3. **Testar métodos**:
   - Expandir o serviço
   - Clicar num método (ex: `GetPoolStatus`)
   - Clicar em "Request" e depois "Submit"

### Opção 2: Postman

1. **Importar WSDL**:
   - New → Import
   - URL: `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
2. **Criar Request SOAP**:
   - Selecionar método
   - Body → raw → XML
   - Colar SOAP envelope

### Opção 3: cURL (Linha de Comando)

```bash
# Obter WSDL
curl "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl"

# Chamar GetPoolStatus
curl -X POST "https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService" \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: \"http://tempuri.org/IPoolDataService/GetPoolStatus\"" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetPoolStatus xmlns="http://tempuri.org/">
    </GetPoolStatus>
  </soap:Body>
</soap:Envelope>'
```

### Opção 4: Código C# (Cliente SOAP)

```csharp
using System.ServiceModel;

// Criar binding
var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress("https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService");
var client = new PoolDataServiceClient(binding, endpoint);

// Chamar método
var status = client.GetPoolStatus();
Console.WriteLine($"Visitantes: {status.CurrentCount}/{status.MaxCapacity}");
```

---

## 📖 WSDL (Web Services Description Language)

O **WSDL** é um documento XML que descreve:
- **Endpoints** disponíveis
- **Métodos** de cada serviço
- **Parâmetros** de entrada e saída
- **Tipos de dados** (Data Contracts)
- **Binding** (protocolo e formato)

### Aceder ao WSDL

Para qualquer serviço SOAP, adicione `?wsdl` ao URL:

**URLs de Produção (Azure):**
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService?wsdl`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService?wsdl`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService?wsdl`
- ✅ `https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService?wsdl`

**URLs Locais (Desenvolvimento):**
- `http://localhost:5011/soap/PoolDataService?wsdl`
- `http://localhost:5011/soap/WorkerDataService?wsdl`
- `http://localhost:5011/soap/WaterQualityDataService?wsdl`
- `http://localhost:5011/soap/ReportDataService?wsdl`

### Estrutura do WSDL

```xml
<?xml version="1.0" encoding="utf-8"?>
<definitions xmlns="http://schemas.xmlsoap.org/wsdl/">
  <!-- Types: Definições de tipos de dados -->
  <types>
    <schema>
      <!-- PoolStatusData, WorkerData, etc. -->
    </schema>
  </types>
  
  <!-- Messages: Estrutura das mensagens -->
  <message name="GetPoolStatusRequest"/>
  <message name="GetPoolStatusResponse"/>
  
  <!-- PortType: Interface do serviço -->
  <portType name="IPoolDataService">
    <operation name="GetPoolStatus">
      <input message="GetPoolStatusRequest"/>
      <output message="GetPoolStatusResponse"/>
    </operation>
  </portType>
  
  <!-- Binding: Protocolo (SOAP) -->
  <binding name="IPoolDataServiceBinding" type="IPoolDataService">
    <soap:binding style="document" transport="http://schemas.xmlsoap.org/soap/http"/>
  </binding>
  
  <!-- Service: Endpoint real -->
  <service name="PoolDataService">
    <port name="IPoolDataService" binding="IPoolDataServiceBinding">
      <soap:address location="https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService"/>
    </port>
  </service>
</definitions>
```

---

## 🔐 Segurança

Atualmente, os serviços SOAP **não têm autenticação** implementada. Para produção, recomenda-se:

1. **WS-Security**: Adicionar headers de autenticação no SOAP envelope
2. **HTTPS obrigatório**: Encriptar todas as comunicações
3. **API Keys**: Validar chave de API no header
4. **IP Whitelist**: Restringir acesso por IP (Azure Firewall)

---

## 📊 Comparação: SOAP vs REST no PoolTracker

| Aspecto | SOAP (Data Layer) | REST (API Layer) |
|---------|-------------------|------------------|
| **Propósito** | Acesso estruturado à base de dados | Operações de negócio e UI |
| **Formato** | XML | JSON |
| **Endpoints** | 4 serviços, 15+ métodos | 40+ endpoints |
| **Uso** | Integração com sistemas externos | Frontend, mobile apps |
| **Complexidade** | Mais verboso, mas mais estruturado | Mais simples e direto |
| **Contrato** | WSDL obrigatório | OpenAPI/Swagger |

---

## 🎯 Casos de Uso

### Quando usar SOAP?

1. **Integração com sistemas legados** que só suportam SOAP
2. **Comunicação entre empresas** (B2B) com contratos bem definidos
3. **Transações críticas** que precisam de garantias de entrega
4. **Sistemas enterprise** que exigem WS-Security

### Quando usar REST?

1. **APIs modernas** para web e mobile
2. **Integração rápida** com frontend
3. **APIs públicas** com documentação Swagger
4. **Microservices** leves e rápidos

---

## 📚 Referências

- [SOAP Specification (W3C)](https://www.w3.org/TR/soap/)
- [WSDL Specification (W3C)](https://www.w3.org/TR/wsdl)
- [SoapCore Documentation](https://github.com/DigDes/SoapCore)
- [Microsoft: SOAP Web Services](https://docs.microsoft.com/dotnet/core/additional-tools/svcutil-guide)

---

**Última Atualização**: 26 de Dezembro de 2025  
**Status**: ✅ 4 Serviços SOAP implementados, integrados e deployados

## 🌐 URLs de Produção

Todos os serviços SOAP estão disponíveis em produção:

- **PoolDataService**: https://pooltracker-api-64853.azurewebsites.net/soap/PoolDataService
- **WorkerDataService**: https://pooltracker-api-64853.azurewebsites.net/soap/WorkerDataService
- **WaterQualityDataService**: https://pooltracker-api-64853.azurewebsites.net/soap/WaterQualityDataService
- **ReportDataService**: https://pooltracker-api-64853.azurewebsites.net/soap/ReportDataService

**WSDL**: Adicionar `?wsdl` ao final de cada URL para aceder ao contrato do serviço.

