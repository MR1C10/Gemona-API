# 🎯 Gemona API

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=flat&logo=mysql&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-Blob_Storage-0078D4?style=flat&logo=microsoft-azure)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

API REST para gerenciamento de serviços profissionais, estabelecimentos e agendamentos. Sistema completo com autenticação JWT, upload de imagens, busca geolocalizada e validações robustas.

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Requisitos](#-requisitos)
- [Instalação](#-instalação)
- [Configuração](#-configuração)
- [Executando](#-executando)
- [Docker](#-docker)
- [Documentação](#-documentação)
- [Endpoints Principais](#-endpoints-principais)
- [Testes](#-testes)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

---

## 🎯 Sobre o Projeto

**Gemona** é uma API REST moderna desenvolvida em .NET 8 que conecta clientes a profissionais e estabelecimentos que prestam serviços. O sistema oferece:

- **Autenticação segura** com JWT e roles (Admin, Cliente, Profissional)
- **Upload de imagens** via Base64 para Azure Blob Storage
- **Busca geolocalizada** com cálculo de distância Haversine
- **Geocodificação automática** de CEPs brasileiros (ViaCEP + OpenCage)
- **Validações robustas** com FluentValidation
- **Exception handling global** com logs detalhados
- **Clean Architecture** com separação de responsabilidades

---

## ✨ Funcionalidades

### 🔐 Autenticação e Autorização
- [x] Registro de usuários (Cliente/Profissional)
- [x] Login com JWT tokens
- [x] Refresh tokens
- [x] Autorização baseada em roles (Admin, Cliente, Profissional)

### 👥 Gestão de Usuários
- [x] Perfis de Cliente e Profissional
- [x] Upload de foto de perfil (Base64 → Azure Blob)
- [x] Atualização de dados cadastrais
- [x] Value Objects para CPF/CNPJ (com validação)

### 🏢 Estabelecimentos e Serviços
- [x] CRUD completo de estabelecimentos
- [x] Gestão de serviços oferecidos
- [x] Horários de funcionamento
- [x] Categorias e subcategorias (seed data)
- [x] Upload de imagens de estabelecimentos/serviços

### 🗺️ Geolocalização
- [x] Busca de endereços por CEP (ViaCEP)
- [x] Obtenção automática de coordenadas GPS (OpenCage)
- [x] Busca de estabelecimentos por proximidade (raio em km)
- [x] Cálculo de distância com fórmula de Haversine
- [x] Filtros por cidade, estado, termo de busca

### ⭐ Avaliações
- [x] Sistema de avaliações com notas 1-5
- [x] Comentários com imagem opcional
- [x] Média de avaliações por estabelecimento

### 📦 Pedidos
- [x] Criação de pedidos de serviço
- [x] Histórico de status
- [x] Estados: Pendente, Confirmado, EmAndamento, Concluído, Cancelado

---

## 🛠️ Tecnologias

### Core
- **.NET 8.0** - Framework principal
- **C# 12** - Linguagem
- **ASP.NET Core Web API** - Framework web

### Banco de Dados
- **MySQL 8.0** - Banco de dados relacional
- **Entity Framework Core 8.0** - ORM
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL
- **Railway** - Hospedagem do banco (produção)

### Autenticação e Segurança
- **JWT Bearer Tokens** - Autenticação
- **BCrypt.Net** - Hash de senhas
- **ASP.NET Core Identity** - Gestão de usuários

### Validação
- **FluentValidation 11.9.2** - Validações de DTOs
- **FluentValidation.AspNetCore** - Integração com ASP.NET

### Cloud & Armazenamento
- **Azure Blob Storage** - Armazenamento de imagens
- **Azure.Storage.Blobs 12.19.1** - SDK do Azure

### APIs Externas
- **OpenCage Geocoding API** - Obtenção de coordenadas GPS
- **ViaCEP** - Busca de CEPs brasileiros

### Containerização
- **Docker** - Containerização
- **Docker Compose** - Orquestração multi-container

### Documentação
- **Swagger/OpenAPI** - Documentação interativa da API
- **Swashbuckle.AspNetCore** - Geração de Swagger

### Logging
- **ILogger nativo do .NET** - Sistema de logs

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

```
Gemona/
├── Gemona.API/                    # Camada de apresentação (Controllers, Middlewares)
│   ├── Controllers/               # Endpoints da API
│   ├── Middlewares/               # Exception handling, logging
│   ├── Extensions/                # Service registration
│   └── Properties/                # Launch settings
│
├── Gemona.Application/            # Camada de aplicação (Use Cases, DTOs, Validações)
│   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Request/               # DTOs de entrada
│   │   ├── Response/              # DTOs de saída
│   │   └── Shared/                # DTOs compartilhados (ImageUpload, ApiResponse)
│   ├── Services/                  # Lógica de negócio
│   ├── Validators/                # FluentValidation validators
│   ├── Interfaces/                # Contratos de serviços/repositórios
│   ├── Helpers/                   # GeoHelper (Haversine)
│   └── Exceptions/                # Custom exceptions
│
├── Gemona.Domain/                 # Camada de domínio (Entidades, Value Objects)
│   ├── Entities/                  # Entidades de negócio
│   ├── ValueObjects/              # CPF, CNPJ, CEP (com validação)
│   ├── Enums/                     # Enumerações
│   └── Constants/                 # Constantes do domínio
│
├── Gemona.Infrastructure/         # Camada de infraestrutura (Data Access, External Services)
│   ├── Data/                      # EF Core, Repositories
│   │   ├── Context/               # ApplicationDbContext
│   │   └── Repositories/          # Implementações de repositórios
│   ├── Configurations/            # EF Core Entity Configurations
│   ├── Migrations/                # EF Core Migrations
│   ├── Services/                  # JWT, Azure Blob Storage
│   ├── ExternalServices/          # APIs externas
│   │   ├── Azure/                 # BlobStorageService
│   │   ├── OpenCage/              # OpenCageGeocodingService
│   │   └── ViaCep/                # ViaCepService
│   └── Extensions/                # DI registration
│
├── Docs/                          # Documentação técnica
└── docker-compose.yml             # Orquestração Docker
```

### Princípios Aplicados
- ✅ **Separation of Concerns** - Cada camada tem responsabilidade única
- ✅ **Dependency Inversion** - Interfaces em Application, implementações em Infrastructure
- ✅ **Single Responsibility** - Classes focadas em uma única tarefa
- ✅ **Domain-Driven Design** - Value Objects, entidades ricas
- ✅ **Repository Pattern** - Abstração do acesso a dados
- ✅ **Service Layer** - Lógica de negócio isolada

---

## 📦 Requisitos

### Desenvolvimento Local
- **.NET 8.0 SDK** ou superior
- **MySQL 8.0** ou superior
- **Visual Studio 2022** / **VS Code** / **Rider**
- **Docker Desktop** (opcional, para rodar via container)

### Contas Externas (Free Tier)
- **Azure Storage Account** - Upload de imagens
- **OpenCage API Key** - Geocoding (2.500 req/dia grátis)
- **Railway / Azure / AWS** - Deploy do banco (opcional)

---

## 🚀 Instalação

### 1. Clone o repositório
```bash
git clone https://github.com/MR1C10/Gemona-API.git
cd Gemona-API
```

### 2. Restaure as dependências
```bash
dotnet restore
```

### 3. Configure o banco de dados

#### Opção A: MySQL Local
```bash
# Instale o MySQL 8.0
# Crie um banco de dados
mysql -u root -p
CREATE DATABASE gemona_db;
```

#### Opção B: Docker MySQL
```bash
docker run --name mysql-gemona -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=gemona_db -p 3306:3306 -d mysql:8.0
```

### 4. Configure as variáveis de ambiente

Crie `Gemona.API/appsettings.json` baseado em `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=gemona_db;User=root;Password=root;Port=3306"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_MINIMO_32_CARACTERES_AQUI",
    "Issuer": "GemonaAPI",
    "Audience": "GemonaClients",
    "ExpireDays": 7
  },
  "AzureStorage": {
    "ConnectionString": "SUA_CONNECTION_STRING_AZURE_STORAGE",
    "ContainerName": "images"
  },
  "OpenCage": {
    "ApiKey": "SUA_OPENCAGE_API_KEY"
  },
  "AllowedHosts": "*"
}
```

### 5. Execute as migrations
```bash
cd Gemona.API
dotnet ef database update
```

### 6. (Opcional) Execute o seed de categorias
O seed é executado automaticamente no startup da aplicação. Dados criados:
- 6 categorias principais
- 37 subcategorias

---

## ⚙️ Configuração

### Azure Blob Storage

1. Crie uma Storage Account no [Azure Portal](https://portal.azure.com)
2. Crie um container chamado `images` (acesso público: Blob)
3. Copie a Connection String
4. Configure em `appsettings.json`:

```json
"AzureStorage": {
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net",
  "ContainerName": "images"
}
```

### OpenCage API

1. Registre-se em [OpenCage](https://opencagedata.com/)
2. Gere uma API Key (2.500 requisições/dia grátis)
3. Configure em `appsettings.json`:

```json
"OpenCage": {
  "ApiKey": "sua_api_key_aqui"
}
```

### JWT

Gere uma chave secreta forte (mínimo 32 caracteres):

```bash
# PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))

# Bash
openssl rand -base64 32
```

---

## ▶️ Executando

### Desenvolvimento Local

```bash
cd Gemona.API
dotnet run --launch-profile https
```

A API estará disponível em:
- **HTTPS:** https://localhost:5269
- **HTTP:** http://localhost:5268
- **Swagger:** https://localhost:5269/swagger

### Watch Mode (Hot Reload)

```bash
dotnet watch run --launch-profile https
```

---

## 🐳 Docker

### Build da imagem

```bash
docker build -t gemona-api .
```

### Executar com Docker Compose

```bash
# Crie o arquivo .env (veja .env.example)
cp .env.example .env

# Edite o .env com suas credenciais

# Suba os containers
docker-compose up -d

# Logs
docker-compose logs -f

# Parar
docker-compose down
```

O docker-compose sobe:
- **API** na porta 8080
- **MySQL** na porta 3306

---

## 📚 Documentação

### Documentos Técnicos

A pasta `Docs/` contém documentação detalhada:

- **[ANÁLISE-PROJETO.md](Docs/ANÁLISE-PROJETO.md)** - Visão geral, decisões arquiteturais, roadmap
- **[EXCEPTION-HANDLING.md](Docs/EXCEPTION-HANDLING.md)** - Sistema de tratamento de erros
- **[FLUENTVALIDATION-SETUP.md](Docs/FLUENTVALIDATION-SETUP.md)** - Configuração de validações
- **[FUNCIONALIDADES.md](Docs/FUNCIONALIDADES.md)** - Lista completa de features
- **[IMAGE-UPLOAD.md](Docs/IMAGE-UPLOAD.md)** - Sistema de upload para Azure
- **[OPENCAGE-GEOCODING.md](Docs/OPENCAGE-GEOCODING.md)** - Integração com geocoding

### Swagger/OpenAPI

Acesse a documentação interativa em:

```
https://localhost:5269/swagger
```

Recursos do Swagger:
- ✅ Teste todos os endpoints diretamente
- ✅ Autenticação JWT integrada (botão "Authorize")
- ✅ Schemas completos de Request/Response
- ✅ Exemplos de uso

---

## 🔌 Endpoints Principais

### Autenticação
```http
POST   /api/auth/register/cliente      # Registro de cliente
POST   /api/auth/register/profissional # Registro de profissional
POST   /api/auth/login                 # Login
POST   /api/auth/refresh-token         # Renovar token
```

### Categorias
```http
GET    /api/categoria                  # Listar categorias
GET    /api/categoria/{id}             # Detalhes da categoria
POST   /api/categoria                  # Criar categoria (Admin)
PUT    /api/categoria/{id}             # Atualizar categoria (Admin)
DELETE /api/categoria/{id}             # Deletar categoria (Admin)
```

### Estabelecimentos
```http
GET    /api/estabelecimento            # Listar estabelecimentos
GET    /api/estabelecimento/{id}       # Detalhes do estabelecimento
GET    /api/estabelecimento/buscar     # Buscar (query params: cidade, termo, raio, lat, lng)
POST   /api/estabelecimento            # Criar estabelecimento
PUT    /api/estabelecimento/{id}       # Atualizar estabelecimento
DELETE /api/estabelecimento/{id}       # Deletar estabelecimento
```

### Endereços (Geocoding)
```http
POST   /api/endereco/buscar-por-cep    # Buscar endereço por CEP (ViaCEP + coordenadas)
```

### Imagens
```http
POST   /api/image/upload               # Upload de imagem (multipart/form-data)
GET    /api/image/{blobName}           # Download de imagem
DELETE /api/image/{blobName}           # Deletar imagem (Admin/Profissional)
```

### Exemplo de Requisição

#### Criar Profissional com Imagem
```json
POST /api/auth/register/profissional
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@email.com",
  "telefone": "11987654321",
  "cpf": "12345678901",
  "imagemPerfil": {
    "fileName": "perfil.jpg",
    "contentType": "image/jpeg",
    "base64Data": "/9j/4AAQSkZJRgABAQEAYABgAAD..."
  },
  "dataNascimento": "1990-05-15T00:00:00.000Z",
  "senha": "SenhaSegura123!"
}
```

#### Buscar Estabelecimentos Próximos
```http
GET /api/estabelecimento/buscar?latitude=-23.5505&longitude=-46.6333&raioKm=5
Authorization: Bearer {token}
```

---

## 🧪 Testes

### Executar testes unitários
```bash
dotnet test
```

### Executar com coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Siga os passos:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'feat: adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

### Padrão de Commits

Seguir o [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Documentação
- `refactor:` Refatoração de código
- `test:` Testes
- `chore:` Tarefas de manutenção

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👨‍💻 Autor

**Maurício Rodrigues**

- GitHub: [@MR1C10](https://github.com/MR1C10)
- Email: mauriciorcsouza1206@gmail.com

---

## 📊 Status do Projeto

```
✅ MVP Completo
✅ Documentação Completa
✅ Docker Ready
✅ Production Ready
🚀 V1.0.3 Released
```

---

<div align="center">
  
**⭐ Se este projeto foi útil, considere dar uma estrela! ⭐**

</div>
