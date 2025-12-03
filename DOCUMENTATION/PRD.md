# Product Requirements Document (PRD)
# PoolTracker - Sistema Integrado de Gestão de Piscina Municipal

---

## Informação do Documento

**Projeto**: PoolTracker - Sistema de Gestão de Piscina Municipal  
**Versão**: 2.0 (.NET Edition)  
**Autor**: Ricardo Guimarães  
**Data**: Dezembro 2025  
**Unidade Curricular**: Integração de Sistemas de Informação (ISI)  
**Instituição**: IPCA - Instituto Politécnico do Cávado e do Ave  
**Curso**: Licenciatura em Engenharia de Sistemas Informáticos  

---

## Índice

1. [Resumo Executivo](#1-resumo-executivo)
2. [Visão do Produto](#2-visão-do-produto)
3. [Objetivos do Projeto](#3-objetivos-do-projeto)
4. [Stakeholders e Utilizadores](#4-stakeholders-e-utilizadores)
5. [Requisitos Funcionais](#5-requisitos-funcionais)
6. [Requisitos Não-Funcionais](#6-requisitos-não-funcionais)
7. [Especificações Técnicas](#7-especificações-técnicas)
8. [Arquitetura do Sistema](#8-arquitetura-do-sistema)
9. [User Stories](#9-user-stories)
10. [Alinhamento com Objetivos Académicos](#10-alinhamento-com-objetivos-académicos)
11. [Plano de Implementação](#11-plano-de-implementação)
12. [Métricas de Sucesso](#12-métricas-de-sucesso)
13. [Riscos e Mitigações](#13-riscos-e-mitigações)
14. [Glossário](#14-glossário)

---

## 1. Resumo Executivo

### 1.1 Contexto

O PoolTracker é um sistema integrado de gestão para piscinas municipais, desenvolvido como projeto académico no âmbito da unidade curricular de **Integração de Sistemas de Informação (ISI)**. O sistema visa demonstrar competências em desenvolvimento de serviços web (SOAP e RESTful), integração de sistemas, segurança, testes automatizados e deployment em cloud.

### 1.2 Problema

Atualmente, as piscinas municipais enfrentam desafios na gestão eficiente de:
- Controlo de lotação em tempo real
- Gestão de trabalhadores e turnos
- Monitorização da qualidade da água
- Registo de limpezas e manutenção
- Geração de relatórios operacionais
- Disponibilização de informação ao público

### 1.3 Solução Proposta

Desenvolvimento de um sistema completo baseado em **Arquitetura Orientada a Serviços (SOA)** que integra:
- **API RESTful** para operações CRUD e integração com aplicações terceiras
- **Serviços SOAP** para acesso à camada de dados (Data Layer)
- **Frontend React** moderno e responsivo para utilizadores públicos e administradores
- **Autenticação JWT** para segurança
- **Integração com APIs externas** (meteorologia)
- **Base de dados SQL** persistente e normalizada
- **Deployment em Cloud** (Azure/Railway/Render)

### 1.4 Valor Entregue

- ✅ Sistema funcional e completo para gestão de piscinas
- ✅ Demonstração prática de conceitos de Integração de Sistemas
- ✅ Cumprimento de todos os requisitos do Trabalho Prático II (TP2)
- ✅ Portfolio técnico robusto para o aluno
- ✅ Base reutilizável para projetos futuros

---

## 2. Visão do Produto

### 2.1 Declaração de Visão

*"Criar um sistema de gestão de piscinas municipais moderno, escalável e seguro, que demonstre excelência técnica em integração de sistemas de informação através de serviços web SOAP e RESTful, servindo tanto utilizadores públicos como administradores com interfaces intuitivas e dados em tempo real."*

### 2.2 Objetivos de Negócio

1. **Eficiência Operacional**: Reduzir o tempo gasto em tarefas administrativas através de automação
2. **Transparência**: Disponibilizar informação em tempo real ao público
3. **Qualidade**: Garantir controlo rigoroso da qualidade da água e limpeza
4. **Conformidade**: Manter registos detalhados para auditorias
5. **Escalabilidade**: Permitir expansão futura para múltiplas piscinas

### 2.3 Princípios de Design

- **API-First**: Toda a lógica de negócio acessível via APIs
- **Mobile-First**: Interface responsiva para todos os dispositivos
- **Security-First**: Autenticação e autorização em todos os endpoints sensíveis
- **Data-Driven**: Decisões baseadas em métricas e relatórios
- **User-Centric**: Foco na experiência do utilizador

---

## 3. Objetivos do Projeto

### 3.1 Objetivos Académicos

Conforme definido no enunciado do TP2 de ISI:

1. ✅ **Consolidar conceitos** de Integração de Sistemas usando serviços web
2. ✅ **Desenhar arquiteturas** de integração recorrendo a APIs de interoperabilidade
3. ✅ **Explorar ferramentas** de suporte ao desenvolvimento de serviços web
4. ✅ **Explorar novas tecnologias** para implementação de SOAP e RESTful
5. ✅ **Potenciar experiência** no desenvolvimento de aplicações
6. ✅ **Assimilar conteúdos** da Unidade Curricular

### 3.2 Objetivos Técnicos

1. **Desenvolver serviços SOAP** para acesso à camada de dados
2. **Desenvolver API RESTful** com operações CRUD completas
3. **Documentar API** usando OpenAPI/Swagger
4. **Implementar autenticação** OAuth/JWT
5. **Criar testes automatizados** (unitários, integração, end-to-end)
6. **Publicar na cloud** serviços e base de dados
7. **Integrar serviços externos** (meteorologia, geolocalização)

### 3.3 Objetivos de Qualidade

- **Code Coverage**: Mínimo 70% nos testes
- **Performance**: APIs respondem em <200ms (95% dos casos)
- **Uptime**: 99% de disponibilidade em produção
- **Security**: Zero vulnerabilidades críticas (scan de segurança)
- **Documentation**: 100% dos endpoints documentados

---

## 4. Stakeholders e Utilizadores

### 4.1 Stakeholders

| Stakeholder | Interesse | Expectativa |
|-------------|-----------|-------------|
| **Docentes ISI** | Avaliação académica | Cumprimento de requisitos TP2 |
| **Aluno (Ricardo)** | Aprendizagem e nota | Sistema funcional e documentado |
| **Gestão da Piscina** | Operação eficiente | Ferramenta útil e confiável |
| **Utilizadores Públicos** | Informação | Interface simples e atualizada |

### 4.2 Personas de Utilizadores

#### Persona 1: Administrador da Piscina
- **Nome**: Carlos Silva, 35 anos
- **Papel**: Gestor da Piscina Municipal
- **Objetivos**: Controlar lotação, gerir trabalhadores, gerar relatórios
- **Necessidades**: Dashboard completo, dados em tempo real, relatórios automáticos
- **Frustrações**: Sistemas lentos, falta de histórico, processos manuais

#### Persona 2: Rececionista
- **Nome**: Ana Costa, 28 anos
- **Papel**: Funcionária da receção
- **Objetivos**: Registar entradas/saídas rapidamente
- **Necessidades**: Interface rápida, sem erros, mobile-friendly
- **Frustrações**: Sistemas complexos, muitos cliques

#### Persona 3: Utilizador Público
- **Nome**: João Pereira, 42 anos
- **Papel**: Pai de família
- **Objetivos**: Saber se a piscina está aberta e quantas pessoas estão
- **Necessidades**: Informação rápida, meteorologia, horários
- **Frustrações**: Informação desatualizada, sites lentos

#### Persona 4: Nadador-Salvador
- **Nome**: Maria Santos, 25 anos
- **Papel**: Nadadora-Salvadora
- **Objetivos**: Registar medições de qualidade da água
- **Necessidades**: Formulários simples, histórico de medições
- **Frustrações**: Papel e caneta, perda de registos

---

## 5. Requisitos Funcionais

### 5.1 Módulo: Gestão de Lotação

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-001 | Sistema deve permitir registar entrada de pessoa | ALTA | Como rececionista, quero registar entrada para controlar lotação |
| RF-002 | Sistema deve permitir registar saída de pessoa | ALTA | Como rececionista, quero registar saída para libertar capacidade |
| RF-003 | Sistema deve impedir entrada quando lotação máxima atingida | ALTA | Como administrador, quero garantir segurança respeitando capacidade |
| RF-004 | Sistema deve mostrar lotação atual em tempo real | ALTA | Como utilizador público, quero saber quantas pessoas estão na piscina |
| RF-005 | Sistema deve permitir alterar capacidade máxima | MÉDIA | Como administrador, quero ajustar capacidade conforme necessário |
| RF-006 | Sistema deve permitir definir contagem manualmente | MÉDIA | Como administrador, quero corrigir erros de contagem |
| RF-007 | Sistema deve resetar contagem ao fechar piscina | ALTA | Como administrador, quero garantir consistência de dados |

### 5.2 Módulo: Gestão de Trabalhadores

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-008 | Sistema deve permitir criar trabalhador | ALTA | Como administrador, quero cadastrar novos funcionários |
| RF-009 | Sistema deve permitir editar dados de trabalhador | ALTA | Como administrador, quero atualizar informações |
| RF-010 | Sistema deve permitir eliminar trabalhador | MÉDIA | Como administrador, quero remover funcionários inativos |
| RF-011 | Sistema deve permitir ativar turno de trabalhador | ALTA | Como administrador, quero registar início de turno |
| RF-012 | Sistema deve permitir desativar turno de trabalhador | ALTA | Como administrador, quero registar fim de turno |
| RF-013 | Sistema deve suportar turnos manhã (9h-14h) e tarde (14h-19h) | ALTA | Como administrador, quero organizar turnos |
| RF-014 | Sistema deve desativar automaticamente turnos ao fechar piscina | ALTA | Como administrador, quero automação de processos |
| RF-015 | Sistema deve mostrar trabalhadores ativos ao público | MÉDIA | Como utilizador público, quero saber quem está de serviço |
| RF-016 | Sistema deve filtrar trabalhadores por cargo | MÉDIA | Como administrador, quero visualizar por função |

### 5.3 Módulo: Qualidade da Água

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-017 | Sistema deve permitir registar medição de pH | ALTA | Como nadador-salvador, quero registar pH medido |
| RF-018 | Sistema deve permitir registar temperatura da água | ALTA | Como nadador-salvador, quero registar temperatura |
| RF-019 | Sistema deve distinguir piscina de crianças e adultos | ALTA | Como nadador-salvador, quero separar registos por piscina |
| RF-020 | Sistema deve mostrar última medição ao público | MÉDIA | Como utilizador público, quero saber qualidade da água |
| RF-021 | Sistema deve manter histórico de medições | ALTA | Como administrador, quero analisar tendências |
| RF-022 | Sistema deve permitir adicionar notas às medições | BAIXA | Como nadador-salvador, quero registar observações |

### 5.4 Módulo: Limpezas

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-023 | Sistema deve permitir registar limpeza de balneários | MÉDIA | Como funcionário de limpeza, quero registar trabalho |
| RF-024 | Sistema deve permitir registar limpeza de WC | MÉDIA | Como funcionário de limpeza, quero registar trabalho |
| RF-025 | Sistema deve mostrar última limpeza ao público | BAIXA | Como utilizador público, quero saber estado de limpeza |
| RF-026 | Sistema deve manter histórico de limpezas | MÉDIA | Como administrador, quero controlar frequência |

### 5.5 Módulo: Relatórios e Estatísticas

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-027 | Sistema deve gerar relatório diário automaticamente | ALTA | Como administrador, quero relatórios automáticos |
| RF-028 | Sistema deve incluir total de visitantes no relatório | ALTA | Como administrador, quero saber fluxo diário |
| RF-029 | Sistema deve incluir ocupação máxima no relatório | ALTA | Como administrador, quero saber picos |
| RF-030 | Sistema deve incluir ocupação média no relatório | MÉDIA | Como administrador, quero métricas agregadas |
| RF-031 | Sistema deve incluir dados de qualidade da água no relatório | ALTA | Como administrador, quero conformidade |
| RF-032 | Sistema deve incluir contagem de turnos no relatório | MÉDIA | Como administrador, quero controlar horas trabalhadas |
| RF-033 | Sistema deve mostrar gráfico de fluxo de visitantes (7 dias) | MÉDIA | Como administrador, quero visualizar tendências |
| RF-034 | Sistema deve mostrar gráfico de turnos por trabalhador | MÉDIA | Como administrador, quero distribuição equitativa |
| RF-035 | Sistema deve permitir filtrar relatórios por período | MÉDIA | Como administrador, quero análises personalizadas |

### 5.6 Módulo: Meteorologia

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-036 | Sistema deve mostrar temperatura atual | ALTA | Como utilizador público, quero saber clima |
| RF-037 | Sistema deve mostrar condição meteorológica | ALTA | Como utilizador público, quero saber se está a chover |
| RF-038 | Sistema deve mostrar velocidade do vento | MÉDIA | Como utilizador público, quero informação completa |
| RF-039 | Sistema deve usar cache para evitar rate limits | ALTA | Como desenvolvedor, quero otimizar custos |
| RF-040 | Sistema deve integrar com Open-Meteo API | ALTA | Como desenvolvedor, quero usar serviços gratuitos |

### 5.7 Módulo: Autenticação e Autorização

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-041 | Sistema deve permitir login com PIN | ALTA | Como administrador, quero acesso seguro |
| RF-042 | Sistema deve gerar JWT token após login | ALTA | Como desenvolvedor, quero autenticação stateless |
| RF-043 | Sistema deve validar JWT em endpoints protegidos | ALTA | Como desenvolvedor, quero segurança |
| RF-044 | Sistema deve permitir refresh de token | MÉDIA | Como utilizador, quero sessão contínua |
| RF-045 | Sistema deve permitir logout | MÉDIA | Como utilizador, quero encerrar sessão |
| RF-046 | Endpoints públicos não devem exigir autenticação | ALTA | Como utilizador público, quero acesso livre |

### 5.8 Módulo: Lista de Compras

| ID | Requisito | Prioridade | User Story |
|----|-----------|------------|------------|
| RF-047 | Sistema deve permitir adicionar item à lista | BAIXA | Como administrador, quero organizar compras |
| RF-048 | Sistema deve permitir remover item da lista | BAIXA | Como administrador, quero atualizar lista |
| RF-049 | Sistema deve categorizar itens (bar, limpeza, qualidade) | BAIXA | Como administrador, quero organização |

---

## 6. Requisitos Não-Funcionais

### 6.1 Performance

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-001 | Tempo de resposta de APIs RESTful | < 200ms para 95% dos requests |
| RNF-002 | Tempo de resposta de serviços SOAP | < 500ms para 95% dos requests |
| RNF-003 | Tempo de carregamento da página pública | < 2 segundos (First Contentful Paint) |
| RNF-004 | Tempo de carregamento do painel admin | < 3 segundos (First Contentful Paint) |
| RNF-005 | Capacidade de escala | Suportar até 1000 requests/minuto |

### 6.2 Segurança

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-006 | Comunicação HTTPS | Todas as comunicações em produção via HTTPS |
| RNF-007 | Proteção contra SQL Injection | Usar Prepared Statements / Parameterized Queries |
| RNF-008 | Proteção contra XSS | Sanitização de inputs no frontend |
| RNF-009 | Tokens JWT seguros | Expiração em 60 minutos, algoritmo HS256 |
| RNF-010 | Secrets não expostos | API keys em variáveis de ambiente |
| RNF-011 | CORS configurado | Whitelist de domínios autorizados |

### 6.3 Disponibilidade

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-012 | Uptime em produção | 99% (permite ~7h downtime/mês) |
| RNF-013 | Backup de base de dados | Backup automático diário |
| RNF-014 | Recovery Time Objective (RTO) | < 4 horas |
| RNF-015 | Recovery Point Objective (RPO) | < 24 horas |

### 6.4 Usabilidade

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-016 | Responsividade | Funcional em mobile, tablet, desktop |
| RNF-017 | Acessibilidade | Conformidade WCAG 2.1 Nível AA |
| RNF-018 | Internacionalização | Textos em português (Brasil/Portugal)|
| RNF-019 | Feedback visual | Todas as ações com confirmação visual |

### 6.5 Manutenibilidade

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-020 | Code coverage | Mínimo 70% de cobertura de testes |
| RNF-021 | Documentação de código | 100% dos métodos públicos documentados |
| RNF-022 | Documentação de API | 100% dos endpoints no Swagger |
| RNF-023 | Versionamento | Git com commits descritivos |
| RNF-024 | Padrões de código | Seguir convenções C# / TypeScript |

### 6.6 Compatibilidade

| ID | Requisito | Critério de Aceitação |
|----|-----------|----------------------|
| RNF-025 | Navegadores suportados | Chrome, Firefox, Safari, Edge (últimas 2 versões) |
| RNF-026 | Base de dados | SQL Server / PostgreSQL / MySQL |
| RNF-027 | .NET Version | .NET 8.0 ou superior |
| RNF-028 | Node.js Version | Node.js 18+ |

---

## 7. Especificações Técnicas

### 7.1 Stack Tecnológica

#### Backend

**API RESTful**
- **Framework**: ASP.NET Core 8.0
- **Linguagem**: C# 12
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server / PostgreSQL
- **Authentication**: JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer)
- **Documentation**: Swashbuckle (Swagger/OpenAPI)

**Serviços SOAP**
- **Framework**: ASP.NET Core + SoapCore
- **Protocol**: SOAP 1.1/1.2
- **Format**: XML
- **WSDL**: Auto-generated

#### Frontend

- **Framework**: React 18
- **Build Tool**: Vite
- **Language**: JavaScript/TypeScript
- **Styling**: TailwindCSS
- **UI Components**: Shadcn/ui
- **Charts**: Recharts
- **HTTP Client**: Fetch API / Axios
- **Notifications**: React Hot Toast
- **Icons**: Lucide React

#### DevOps & Cloud

- **Version Control**: Git + GitHub
- **CI/CD**: GitHub Actions (opcional)
- **Cloud Provider**: Azure / Railway / Render
- **Database Hosting**: Azure SQL / Railway PostgreSQL
- **Frontend Hosting**: Vercel / Netlify / Azure Static Web Apps

#### Testing

- **Unit Tests**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Integration Tests**: WebApplicationFactory
- **Code Coverage**: Coverlet

### 7.2 Estrutura de Base de Dados

**8 Tabelas Principais**:

1. **pool_status** - Estado atual da piscina
2. **workers** - Cadastro de trabalhadores
3. **active_workers** - Turnos ativos
4. **water_quality** - Medições de qualidade da água
5. **cleanings** - Registos de limpeza
6. **daily_visitors** - Visitantes por dia
7. **daily_reports** - Relatórios diários
8. **shopping_list** - Lista de compras

**Características**:
- Normalização 3NF
- Foreign Keys com ON DELETE CASCADE
- Índices em campos de busca frequente
- Campos de timestamp (CreatedAt, UpdatedAt)
- Suporte a JSON para dados complexos

### 7.3 APIs Externas

| API | Propósito | Endpoint | Autenticação |
|-----|-----------|----------|--------------|
| Open-Meteo | Dados meteorológicos | `https://api.open-meteo.com/v1/forecast` | Não requerida |

### 7.4 Endpoints da API

**Total: 40+ endpoints**

**Categorias**:
- Pool Management (7 endpoints)
- Workers (8 endpoints)
- Water Quality (4 endpoints)
- Cleanings (4 endpoints)
- Reports (4 endpoints)
- Statistics (3 endpoints)
- Shopping List (3 endpoints)
- Weather (1 endpoint)
- Authentication (2 endpoints)

Ver seção "API RESTful - Endpoints Completos" no Implementation Plan para detalhes.

### 7.5 Serviços SOAP

**4 Serviços SOAP**:

1. **PoolDataService**
   - GetPoolStatus()
   - UpdatePoolStatus()
   - IncrementCount()
   - DecrementCount()

2. **WorkerDataService**
   - GetAllWorkers()
   - GetWorkerById()
   - CreateWorker()
   - UpdateWorker()
   - DeleteWorker()

3. **WaterQualityDataService**
   - GetHistory()
   - GetLatest()
   - RecordMeasurement()

4. **ReportDataService**
   - GetReports()
   - GenerateReport()

---

## 8. Arquitetura do Sistema

### 8.1 Diagrama de Arquitetura de Alto Nível

```
┌─────────────────────────────────────────────────────────────┐
│                         Frontend                             │
│                    (React + Vite + Tailwind)                │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Public Page │  │  Admin Panel │  │  Components  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└────────────┬────────────────────────────────┬───────────────┘
             │ HTTPS (JSON)                   │ HTTPS (JSON)
             ▼                                ▼
┌────────────────────────────────────────────────────────────┐
│                      API Gateway / CORS                     │
└────────────┬────────────────────────────────┬──────────────┘
             │                                │
             ▼                                ▼
┌─────────────────────────┐      ┌─────────────────────────┐
│   RESTful API           │      │   SOAP Services         │
│   (ASP.NET Core)        │      │   (ASP.NET Core +       │
│                         │      │    SoapCore)            │
│  ┌──────────────────┐   │      │                         │
│  │   Controllers    │   │      │  ┌──────────────────┐   │
│  │   - Pool         │   │      │  │  SOAP Services   │   │
│  │   - Workers      │   │      │  │  - PoolData      │   │
│  │   - WaterQuality │   │      │  │  - WorkerData    │   │
│  │   - Cleanings    │   │      │  │  - WaterQuality  │   │
│  │   - Reports      │   │      │  │  - ReportData    │   │
│  │   - Statistics   │   │      │  └──────────────────┘   │
│  │   - Shopping     │   │      │                         │
│  │   - Weather      │   │      └─────────┬───────────────┘
│  │   - Auth         │   │                │
│  └────────┬─────────┘   │                │
│           │             │                │
│  ┌────────▼─────────┐   │                │
│  │   Services       │   │                │
│  │  (Business Logic)│◄──┼────────────────┘
│  └────────┬─────────┘   │
│           │             │
│  ┌────────▼─────────┐   │
│  │  EF Core DbContext│  │
│  └────────┬─────────┘   │
└───────────┼─────────────┘
            │
            ▼
┌─────────────────────────┐       ┌─────────────────────────┐
│   Database              │       │   External APIs         │
│   (SQL Server/          │       │   - Open-Meteo          │
│    PostgreSQL)          │       │   (Weather Data)        │
│                         │       │                         │
│  8 Tables:              │       └─────────────────────────┘
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

### 8.2 Fluxo de Autenticação

```
User                Frontend            AuthController         JWT Service
 │                     │                      │                    │
 │  Enter PIN          │                      │                    │
 ├────────────────────►│                      │                    │
 │                     │  POST /api/auth/login│                    │
 │                     ├─────────────────────►│                    │
 │                     │                      │  Validate PIN      │
 │                     │                      ├───────────────────►│
 │                     │                      │  Generate JWT      │
 │                     │                      │◄───────────────────┤
 │                     │  { token, refresh }  │                    │
 │                     │◄─────────────────────┤                    │
 │  Store in memory    │                      │                    │
 │◄────────────────────┤                      │                    │
 │                     │                      │                    │
 │  API Request        │                      │                    │
 ├────────────────────►│ GET /api/workers     │                    │
 │                     │  Header: Authorization: Bearer {token}   │
 │                     ├─────────────────────►│                    │
 │                     │                      │  Validate JWT      │
 │                     │                      ├───────────────────►│
 │                     │                      │  OK                │
 │                     │                      │◄───────────────────┤
 │                     │  { data }            │                    │
 │  Display Data       │◄─────────────────────┤                    │
 │◄────────────────────┤                      │                    │
```

### 8.3 Padrões Arquiteturais Utilizados

1. **Layered Architecture** (Camadas)
   - Presentation Layer (Controllers)
   - Business Logic Layer (Services)
   - Data Access Layer (Repositories)
   - Database Layer (Entity Framework)

2. **Repository Pattern**
   - Abstração de acesso a dados
   - Facilita testes unitários
   - Reduz acoplamento

3. **Dependency Injection**
   - Injeção de serviços via constructor
   - Configuração em Program.cs
   - Facilita mocking em testes

4. **DTO Pattern**
   - Separação entre Models e DTOs
   - Controlo de dados expostos
   - Validação de inputs

5. **Service Pattern**
   - Lógica de negócio encapsulada
   - Reutilização de código
   - Single Responsibility Principle

---

## 9. User Stories

### 9.1 Épico: Gestão de Lotação

**US-001**: Como **rececionista**, quero **registar entrada de pessoa** para **controlar lotação atual**

**Critérios de Aceitação**:
- Given a piscina está aberta
- And a lotação atual < capacidade máxima
- When clico em "Entrou"
- Then a contagem incrementa em 1
- And a UI atualiza imediatamente

**US-002**: Como **rececionista**, quero **registar saída de pessoa** para **libertar capacidade**

**Critérios de Aceitação**:
- Given a piscina tem pessoas dentro (count > 0)
- When clico em "Saiu"
- Then a contagem decrementa em 1
- And a UI atualiza imediatamente

**US-003**: Como **utilizador público**, quero **ver lotação atual** para **decidir se vou à piscina**

**Critérios de Aceitação**:
- Given acedo à página pública
- When a página carrega
- Then vejo "X/Y pessoas" (X=atual, Y=capacidade)
- And vejo percentagem de ocupação
- And vejo barra de progresso visual

### 9.2 Épico: Gestão de Trabalhadores

**US-004**: Como **administrador**, quero **cadastrar novo trabalhador** para **gerir equipa**

**Critérios de Aceitação**:
- Given estou no painel admin
- When preencho formulário (nome, cargo, worker_id)
- And clico em "Criar"
- Then trabalhador é adicionado à lista
- And recebo confirmação visual

**US-005**: Como **administrador**, quero **ativar turno de trabalhador** para **registar presença**

**Critérios de Aceitação**:
- Given tenho um trabalhador cadastrado
- When seleciono turno (manhã/tarde)
- And clico em "Ativar Turno"
- Then trabalhador aparece como "Em turno"
- And público vê trabalhador ativo

### 9.3 Épico: Qualidade da Água

**US-006**: Como **nadador-salvador**, quero **registar medição de pH** para **garantir qualidade**

**Critérios de Aceitação**:
- Given estou autenticado
- When seleciono piscina (crianças/adultos)
- And insiro valor de pH (ex: 7.2)
- And insiro temperatura (ex: 26.5)
- And clico em "Registar"
- Then medição é guardada
- And aparece no histórico

**US-007**: Como **utilizador público**, quero **ver qualidade da água** para **saber se é seguro nadar**

**Critérios de Aceitação**:
- Given existe medição recente (< 24h)
- When acedo à página pública
- Then vejo pH e temperatura de cada piscina
- And vejo timestamp da última medição

### 9.4 Épico: Relatórios

**US-008**: Como **administrador**, quero **gerar relatório diário** para **análise operacional**

**Critérios de Aceitação**:
- Given a piscina foi fechada
- When sistema gera relatório automático
- Then relatório inclui:
  - Total de visitantes
  - Ocupação máxima
  - Ocupação média
  - Qualidade da água (JSON)
  - Trabalhadores ativos (JSON)
  - Limpezas realizadas (JSON)

**US-009**: Como **administrador**, quero **ver gráfico de visitantes** para **identificar tendências**

**Critérios de Aceitação**:
- Given existem dados dos últimos 7 dias
- When acedo ao dashboard
- Then vejo gráfico de linha com visitantes/dia
- And posso fazer hover para ver números exatos

### 9.5 Épico: Segurança

**US-010**: Como **administrador**, quero **fazer login com PIN** para **acessar painel admin**

**Critérios de Aceitação**:
- Given acedo à rota /admin
- When insiro PIN correto
- Then recebo JWT token
- And acesso painel admin
- And token expira em 60 minutos

---

## 10. Alinhamento com Objetivos Académicos

### 10.1 Mapeamento Requisitos TP2

| Requisito TP2 | Implementação no PoolTracker | Evidência |
|---------------|------------------------------|-----------|
| **Qualidade dos serviços** | Arquitetura SOA, Repository Pattern, DI, Clean Code | Código-fonte, testes |
| **Serviços SOAP (Data Layer)** | 4 serviços SOAP (Pool, Worker, WaterQuality, Report) | Endpoints SOAP + WSDL |
| **Serviços RESTful (CRUD)** | 40+ endpoints REST com GET, POST, PUT, DELETE | Swagger documentation |
| **Serviços externos** | Integração Open-Meteo API | WeatherController.cs |
| **Documentação API** | Swagger UI completo com XML comments | /swagger endpoint |
| **Testes sobre API** | 45+ testes (unit + integration + e2e) | PoolTracker.Tests/ |
| **Repositório na Cloud** | Azure SQL / Railway PostgreSQL | Connection string produção |
| **Segurança nos serviços** | JWT Bearer Authentication | AuthController + Middleware |
| **Serviços na Cloud** | Azure App Service / Railway | URLs de produção |

### 10.2 Conceitos de ISI Demonstrados

1. **Interoperabilidade**
   - Comunicação SOAP (XML) e REST (JSON)
   - Integração com APIs externas
   - Múltiplos clientes (web, potencialmente mobile)

2. **Arquitetura Orientada a Serviços (SOA)**
   - Serviços reutilizáveis
   - Baixo acoplamento
   - Contratos bem definidos (WSDL, OpenAPI)

3. **Segurança**
   - Autenticação (JWT)
   - Autorização (Role-based potencialmente)
   - HTTPS
   - API Keys

4. **Qualidade**
   - Testes automatizados
   - Code coverage
   - Documentação
   - Padrões de código

---

## 11. Plano de Implementação

### 11.1 Roadmap de 4 Semanas

**Semana 1: Fundação + API Core** (18-22h)
- Fase 1: Fundação (10-12h)
- Fase 2: RESTful API Core - Parte 1 (8-10h)

**Semana 2: Funcionalidades + JWT** (18-24h)
- Fase 2: RESTful API Core - Parte 2 (8-10h)
- Fase 3: Funcionalidades Avançadas (6-8h)
- Fase 4: Autenticação JWT (4-6h)

**Semana 3: SOAP + Swagger + Testes** (21-26h)
- Fase 5: Serviços SOAP (8-10h)
- Fase 6: Documentação Swagger (3-4h)
- Fase 7: Testes Automatizados (10-12h)

**Semana 4: Frontend + Deploy + Documentação** (18-24h)
- Fase 8: Frontend Expandido (8-10h)
- Fase 9: Deploy na Cloud (6-8h)
- Fase 10: Documentação e Relatório (4-6h)

**Total**: 75-96 horas (~20h/semana)

### 11.2 Milestones

| Milestone | Data Alvo | Deliverables |
|-----------|-----------|--------------|
| **M1: MVP Backend** | Semana 1 | API REST funcional, DB persistente |
| **M2: Feature Complete** | Semana 2 | Todos os módulos implementados, JWT |
| **M3: SOA Complete** | Semana 3 | SOAP services, Swagger, Testes |
| **M4: Production Ready** | Semana 4 | Deploy cloud, Documentação completa |

### 11.3 Critérios de Done

Para cada fase:
- ✅ Código implementado e commitado
- ✅ Testes criados e passando
- ✅ Documentação atualizada
- ✅ Revisão de código feita
- ✅ Integração com fases anteriores validada

---

## 12. Métricas de Sucesso

### 12.1 Métricas Técnicas

| Métrica | Target | Como Medir |
|---------|--------|------------|
| **Code Coverage** | ≥ 70% | Coverlet report |
| **API Response Time** | < 200ms (p95) | Application Insights / Logs |
| **Build Success Rate** | 100% | GitHub Actions |
| **Security Vulnerabilities** | 0 critical | OWASP ZAP scan |
| **Endpoints Documentados** | 100% | Swagger UI |

### 12.2 Métricas Académicas

| Métrica | Target | Como Medir |
|---------|--------|------------|
| **Requisitos TP2 Cumpridos** | 9/9 | Checklist final |
| **Qualidade do Relatório** | Excellent | Avaliação docente |
| **Apresentação** | Excellent | Avaliação docente |
| **Demonstração Funcional** | 100% features | Live demo |

### 12.3 Métricas de Usabilidade

| Métrica | Target | Como Medir |
|---------|--------|------------|
| **Tempo para registar entrada** | < 5 segundos | Teste com utilizadores |
| **Tempo para consultar lotação** | < 2 segundos | Performance testing |
| **Taxa de erro em formulários** | < 5% | Analytics |

---

## 13. Riscos e Mitigações

### 13.1 Riscos Técnicos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| **Complexidade SOAP em .NET Core** | Média | Alto | Usar SoapCore (biblioteca testada), alocar tempo extra |
| **Problemas de integração EF Core** | Baixa | Médio | Seguir best practices, testes de integração |
| **Rate limiting Open-Meteo** | Baixa | Baixo | Implementar cache de 60s |
| **Falha de deploy na cloud** | Média | Alto | Testar em ambiente de staging primeiro |
| **Perda de dados em produção** | Baixa | Alto | Backups automáticos diários |

### 13.2 Riscos de Projeto

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| **Atraso no cronograma** | Média | Alto | Priorizar requisitos obrigatórios TP2 |
| **Mudanças de requisitos** | Baixa | Médio | Manter foco nos requisitos TP2 |
| **Problemas de desempenho** | Média | Médio | Profiling e otimização contínua |

### 13.3 Riscos Académicos

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| **Não cumprir requisitos TP2** | Baixa | Crítico | Checklist constante, validação com docente |
| **Documentação insuficiente** | Média | Alto | Alocar tempo dedicado (Fase 10) |
| **Falha na apresentação** | Baixa | Alto | Praticar demo, ter backups |

---

## 14. Glossário

| Termo | Definição |
|-------|-----------|
| **API** | Application Programming Interface - interface para comunicação entre sistemas |
| **CORS** | Cross-Origin Resource Sharing - mecanismo de segurança HTTP |
| **CRUD** | Create, Read, Update, Delete - operações básicas de dados |
| **DTO** | Data Transfer Object - objeto para transferência de dados entre camadas |
| **EF Core** | Entity Framework Core - ORM da Microsoft |
| **JWT** | JSON Web Token - padrão para tokens de autenticação |
| **ORM** | Object-Relational Mapping - mapeamento objeto-relacional |
| **PRD** | Product Requirements Document - documento de requisitos |
| **REST** | Representational State Transfer - estilo arquitetural |
| **SOA** | Service-Oriented Architecture - arquitetura orientada a serviços |
| **SOAP** | Simple Object Access Protocol - protocolo de comunicação |
| **WSDL** | Web Services Description Language - linguagem de descrição de serviços |
| **Swagger** | Framework para documentação de APIs REST |

---

## Aprovações

| Stakeholder | Assinatura | Data |
|-------------|------------|------|
| Aluno (Ricardo Guimarães) | ___________ | __/__/2025 |
| Docente ISI | ___________ | __/__/2025 |

---

**Fim do Documento**

*Versão 1.0 - Dezembro 2025*  
*PoolTracker - Sistema Integrado de Gestão de Piscina Municipal*
