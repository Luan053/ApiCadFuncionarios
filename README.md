# API Cadastro de Funcionários

API REST para gerenciamento de funcionários com autenticação JWT.

## Stack

- ASP.NET Core 7
- Entity Framework Core
- PostgreSQL
- BCrypt (hash de senhas)
- JWT (autenticação)

## Estrutura

```
/WebApi
  /Application     # Services e ViewModels
  /Controllers     # Endpoints (v1 e v2)
  /Domain          # Entidades e interfaces
  /Infrastructure  # DbContext e Repositories
```

## Executar

```bash
# Configurar conexão PostgreSQL em appsettings.json
# Aplicar migrations
dotnet ef database update

# Iniciar
dotnet run
```

## Endpoints

### Autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v1/auth/register` | Registrar usuário |
| POST | `/api/v1/auth/login` | Obter token JWT |

### Funcionários (requer autenticação)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/employee` | Listar (paginado) |
| GET | `/api/v1/employee/{id}` | Buscar por ID |
| POST | `/api/v1/employee` | Criar |
| DELETE | `/api/v1/employee/{id}/delete` | Remover |
| POST | `/api/v1/employee/{id}/download` | Baixar foto |

## Autenticação

Após login, usar header:
```
Authorization: Bearer <token>
```

## Decisões Técnicas

- **BCrypt** para hash de senhas
- **JWT via appsettings**
- **Versionamento de API** (v1/v2)
- **Repository Pattern** para acesso a dados por ser um projeto simples
- **Arquitetura em camadas** também por ser um projeto simples
