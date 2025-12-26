using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace PoolTracker.API.Configuration;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "PoolTracker API",
            Version = "v1",
            Description = @"
API completa para gestão de piscina municipal. Sistema desenvolvido no âmbito do TP2 de ISI (Integração de Sistemas de Informação).

## Funcionalidades Principais:

### Gestão de Lotação (`/api/pool`)
- Controlo de entrada/saída de visitantes
- Gestão de capacidade máxima
- Abrir/fechar piscina (desativa automaticamente todos os turnos ao fechar)
- Reset do sistema

### Gestão de Trabalhadores (`/api/workers`)
- CRUD completo de trabalhadores
- Sistema de turnos (Manhã/Tarde)
- Ativar/desativar turnos individuais
- Estatísticas de turnos por período (`/api/workers/shift-stats`)
- Histórico de turnos por trabalhador (`/api/workers/{workerId}/shifts`)
- Lista de trabalhadores ativos (endpoint público)

### Qualidade da Água (`/api/water-quality`)
- Registo de medições (pH, temperatura) para piscinas de crianças e adultos
- Histórico com filtros de data
- Última medição por tipo de piscina

### Limpezas (`/api/cleaning`)
- Registo de limpezas de balneários e WC
- Histórico de limpezas recentes
- Últimas limpezas (endpoint público)

### Relatórios (`/api/reports`)
- Geração automática de relatórios diários (ao fechar a piscina)
- Lista de relatórios com filtros de data
- Estatísticas de visitantes

### Estatísticas (`/api/statistics`)
- Estatísticas de visitantes por período
- Dados para gráficos e visualizações

### Lista de Compras (`/api/shopping`)
- CRUD completo de itens
- Filtros por categoria (Bar, Limpeza, Qualidade)
- Marcar itens como comprados/não comprados (`PATCH /api/shopping/{id}/toggle-purchased`)
- Ordenação automática (itens não comprados primeiro)
- Registo de data de compra

### Meteorologia (`/api/weather`)
- Integração com Open-Meteo
- Dados meteorológicos em tempo real (endpoint público)

### Autenticação (`/api/auth`)
- Login com PIN de administrador
- Geração de tokens JWT
- Refresh tokens

## Autenticação:
A maioria dos endpoints requer autenticação JWT. Use o endpoint `POST /api/auth/login` com o PIN de administrador para obter um token.

**Exemplo de uso:**
```
POST /api/auth/login
{
  ""pin"": ""1234""
}
```

O token retornado deve ser incluído no header `Authorization: Bearer {token}` para aceder aos endpoints protegidos.
            ".Trim(),
            Contact = new OpenApiContact
            {
                Name = "Ricardo Guimarães",
                Email = "ricardoguimaraes2021@github.com",
                Url = new Uri("https://github.com/ricardoguimaraes2021")
            },
            License = new OpenApiLicense
            {
                Name = "Academic Project",
                Url = new Uri("https://github.com/ricardoguimaraes2021/ISI-TP2-PoolTracker")
            }
        });

        // Tags para organizar os endpoints
        options.TagActionsBy(api =>
        {
            var controllerName = api.ActionDescriptor.RouteValues["controller"] ?? "Default";
            // Capitalizar primeira letra para melhor apresentação
            return new[] { char.ToUpper(controllerName[0]) + controllerName.Substring(1) };
        });
        options.DocInclusionPredicate((name, api) => true);

        // JWT Authentication
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header usando o esquema Bearer. 
Exemplo: 'Authorization: Bearer {token}'

Para obter um token, use o endpoint POST /api/auth/login com o PIN de administrador.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
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

        // XML Comments para documentação dos endpoints
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
        
        // Incluir XML comments dos projetos referenciados (Core) se existir
        var coreXmlFile = "PoolTracker.Core.xml";
        var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
        if (File.Exists(coreXmlPath))
        {
            options.IncludeXmlComments(coreXmlPath);
        }

        // Ordenar endpoints por nome do controller
        options.OrderActionsBy(apiDesc => apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Default");

        // Configurações adicionais
        options.UseInlineDefinitionsForEnums();
        
        // Ignorar propriedades obsoletas
        options.IgnoreObsoleteActions();
        options.IgnoreObsoleteProperties();
        
        // Melhorar a apresentação dos schemas
        options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    }
}

