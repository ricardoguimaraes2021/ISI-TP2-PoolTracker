# Task List

## Analysis Phase (Completed)

- [x] Locate and read the PDF requirement file
- [x] Clone the GitHub repository `https://github.com/ricardoguimaraes2021/PoolTracker.git`
- [x] Analyze the repository structure and features
- [x] Compare repository features with PDF requirements
- [x] Create a report/summary of the findings
- [x] Clone PoolTracker_Online repository (Laravel + React)
- [x] Analyze new features and functionality
- [x] Compare with .NET version
- [x] Identify features that can be ported to .NET
- [x] Create recommendations report
- [x] Create comprehensive implementation plan

---

## Implementation Phase

### Fase 1: Fundação (10-12h)
- [ ] Criar estrutura de projetos (API, SOAP, Core, Infrastructure, Tests)
- [ ] Configurar Entity Framework Core
- [ ] Criar DbContext e Entities (8 tabelas)
- [ ] Criar migrations e aplicar schema
- [ ] Configurar connection strings
- [ ] Implementar Repository Pattern
- [ ] Configurar Dependency Injection

### Fase 2: RESTful API Core (8-10h)
- [ ] Migrar PoolController para usar EF Core
- [ ] Implementar WorkerController + Service
- [ ] Implementar WaterQualityController + Service
- [ ] Implementar CleaningController + Service
- [ ] Implementar VisitorController + Service
- [ ] Adicionar operação DELETE em todos os controllers
- [ ] Validação de DTOs

### Fase 3: Funcionalidades Avançadas (6-8h)
- [ ] Implementar ReportController + Service
- [ ] Lógica de geração automática de relatórios
- [ ] StatisticsController para gráficos
- [ ] ShoppingListController
- [ ] Manter integração com Open-Meteo

### Fase 4: Autenticação JWT (4-6h)
- [ ] Criar AuthController
- [ ] Implementar geração de JWT tokens
- [ ] Configurar JWT Bearer Authentication
- [ ] Adicionar [Authorize] nos endpoints protegidos
- [ ] Implementar refresh tokens
- [ ] Atualizar frontend para usar JWT

### Fase 5: Serviços SOAP (8-10h)
- [ ] Criar projeto PoolTracker.SOAP
- [ ] Definir Service Contracts
- [ ] Definir Data Contracts
- [ ] Implementar PoolDataService
- [ ] Implementar WorkerDataService
- [ ] Implementar WaterQualityDataService
- [ ] Implementar ReportDataService
- [ ] Configurar endpoints SOAP
- [ ] Gerar WSDL

### Fase 6: Documentação Swagger (3-4h)
- [ ] Configurar Swashbuckle
- [ ] Adicionar XML comments em todos os endpoints
- [ ] Configurar autenticação JWT no Swagger
- [ ] Adicionar exemplos de requests/responses
- [ ] Testar UI do Swagger

### Fase 7: Testes Automatizados (10-12h)
- [ ] Configurar projeto de testes (xUnit)
- [ ] Testes unitários de Services (mínimo 20 testes)
- [ ] Testes de integração de Controllers (mínimo 15 testes)
- [ ] Testes de API end-to-end (mínimo 10 testes)
- [ ] Testes de autenticação
- [ ] Configurar code coverage

### Fase 8: Frontend Expandido (8-10h)
- [ ] Migrar componentes Shadcn/ui da versão PHP
- [ ] Instalar e configurar Recharts
- [ ] Criar páginas de gestão de trabalhadores
- [ ] Criar páginas de qualidade da água
- [ ] Criar dashboard de relatórios com gráficos
- [ ] Implementar autenticação JWT no frontend
- [ ] Adicionar React Hot Toast

### Fase 9: Deploy na Cloud (6-8h)
- [ ] Escolher plataforma (Azure/Railway/Render)
- [ ] Criar base de dados na cloud
- [ ] Configurar variáveis de ambiente
- [ ] Deploy da API RESTful
- [ ] Deploy dos serviços SOAP
- [ ] Deploy do frontend
- [ ] Configurar CORS para produção
- [ ] Testar todos os endpoints em produção

### Fase 10: Documentação e Relatório (4-6h)
- [ ] Atualizar README.md
- [ ] Documentar arquitetura
- [ ] Criar guia de instalação
- [ ] Documentar endpoints SOAP e REST
- [ ] Preparar relatório final do TP2
- [ ] Screenshots e evidências

---

## Requisitos TP2 (Checklist Final)

- [ ] ✅ Qualidade dos serviços desenvolvidos
- [ ] ✅ Desenvolveu serviços SOAP (para Data Layer)
- [ ] ✅ Desenvolveu serviços RESTful (POST, GET, PUT, DELETE)
- [ ] ✅ Utilizou serviços web externos
- [ ] ✅ Documentou devidamente a API disponibilizada
- [ ] ✅ Especificou um conjunto de testes para a API desenvolvida
- [ ] ✅ Publicou Repositório de Dados na Cloud
- [ ] ✅ Explorou aplicação de segurança nos serviços
- [ ] ✅ Publicou Serviços na Cloud
