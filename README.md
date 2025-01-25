# API de Gerenciamento de Funcionários

API RESTful para gerenciamento de funcionários, desenvolvida com ASP.NET Core 7.0, implementando boas práticas de desenvolvimento e padrões de projeto.

## 🚀 Funcionalidades

- Autenticação e autorização com JWT
- CRUD completo de funcionários
- Upload e download de fotos
- Paginação de resultados
- Refresh tokens
- Swagger/OpenAPI
- Logs estruturados
- Tratamento de erros global

## 📋 Pré-requisitos

- .NET 7.0 SDK
- PostgreSQL

## 🔧 Configuração

1. Clone o repositório:

```bash
git clone https://github.com/seu-usuario/ApiCadFuncionarios.git
```

2. Configure o banco de dados no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

3. Configure a chave JWT no `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-aqui",
    "Issuer": "seu-issuer",
    "Audience": "sua-audience",
    "ExpiryInMinutes": 60
  }
}
```

4. Execute as migrations:

```bash
dotnet ef database update
```

## 🚀 Executando a API

1. Navegue até a pasta do projeto:

```bash
cd ApiCadFuncionarios/WebApi
```

2. Execute o projeto:

```bash
dotnet run
```

A API estará disponível em `https://localhost:5001` e `http://localhost:5000`

## 📚 Documentação da API

A documentação completa está disponível através do Swagger UI em:

- Desenvolvimento: `https://localhost:5001/swagger`
- Produção: `{sua-url}/swagger`

### Principais Endpoints

#### Autenticação

- POST `/api/v1/auth/register` - Registra um novo usuário
- POST `/api/v1/auth/login` - Realiza login
- POST `/api/v1/auth/refresh-token` - Renova o token de acesso
- POST `/api/v1/auth/revoke` - Revoga o refresh token

#### Funcionários

- GET `/api/v1/employee` - Lista funcionários (paginado)
- GET `/api/v1/employee/{id}` - Obtém um funcionário específico
- POST `/api/v1/employee` - Adiciona um novo funcionário
- PUT `/api/v1/employee/{id}` - Atualiza um funcionário
- DELETE `/api/v1/employee/{id}` - Remove um funcionário
- GET `/api/v1/employee/{id}/photo` - Download da foto do funcionário

## 🔒 Segurança

- Autenticação via JWT Bearer Token
- Refresh Tokens para renovação segura
- Senhas hasheadas com BCrypt
- Validação de uploads de arquivos
- CORS configurável
- Rate Limiting

## 📁 Estrutura do Projeto

```
WebApi/
├── Application/         # Camada de aplicação (ViewModels, Services)
├── Controllers/         # Controllers da API
├── Domain/             # Modelos de domínio e interfaces
├── Infrastructure/     # Implementações de repositório e contexto
├── Migrations/         # Migrations do Entity Framework
└── SwaggerConfig/      # Configurações do Swagger
```

## ⚙️ Tecnologias Utilizadas

- ASP.NET Core 7.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- AutoMapper
- Swagger/OpenAPI
- BCrypt.NET

## 🔍 Boas Práticas Implementadas

- Repository Pattern
- Unit of Work
- SOLID Principles
- Clean Architecture
- Dependency Injection
- Error Handling Global
- Logging Estruturado
- API Versioning
