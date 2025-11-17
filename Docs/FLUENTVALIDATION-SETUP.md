# FluentValidation - Configuração e Testes

## ✅ Configuração Concluída

### 1. Instalação dos Pacotes
- **FluentValidation 11.9.2** - Instalado em Gemona.Application
- **FluentValidation.AspNetCore 11.3.1** - Instalado em Gemona.API

### 2. Validators Criados (11 no total)

#### Categoria
- ✅ `CreateCategoriaRequestValidator` - Valida nome (3-50 caracteres, apenas letras) e URL opcional
- ✅ `UpdateCategoriaRequestValidator` - Mesmas validações do Create

#### Cliente
- ✅ `CreateClienteRequestValidator` - Validações completas:
  - Nome: 3-100 caracteres, apenas letras
  - Email: formato válido
  - Senha: mínimo 6 caracteres, deve conter maiúscula, minúscula e número
  - CPF: algoritmo completo de validação com multiplicadores
  - Data de Nascimento: mínimo 18 anos
  - Telefone: formato brasileiro (11) 99999-9999
  - Imagem: URL válida (opcional)
- ✅ `LoginClienteRequestValidator` - Email e senha obrigatórios

#### Profissional
- ✅ `CreateProfissionalRequestValidator` - Mesmas validações do Cliente (sem campo Especialidade)
- ✅ `LoginProfissionalRequestValidator` - Email e senha obrigatórios

#### Estabelecimento
- ✅ `CreateEstabelecimentoRequestValidator` - Validações completas:
  - Nome, Email, Telefone
  - CNPJ: algoritmo completo de validação
  - Endereço completo:
    - Rua, Número, Bairro, Cidade obrigatórios
    - Estado: 2 caracteres maiúsculos (UF)
    - CEP: formato 12345-678
  - Latitude: -90 a 90
  - Longitude: -180 a 180
  - URL da imagem (opcional)

#### Serviço
- ✅ `CreateServicoRequestValidator` - Validações:
  - Nome: 3-100 caracteres
  - Descrição: 10-1000 caracteres
  - Preço: maior que 0 e menor que 999.999,99
  - EstabelecimentoId e SubCategoriaId obrigatórios
  - URL da imagem (opcional)

#### Pedido
- ✅ `CreatePedidoRequestValidator` - Validações:
  - ClienteId e ServicoId obrigatórios
  - DataAgendamento: opcional, mas se informada deve ser futura
  - Observações: máximo 500 caracteres

#### Avaliação
- ✅ `CreateAvaliacaoRequestValidator` - Validações:
  - ClienteId e PedidoId obrigatórios
  - Nota: enum NotaAvaliacao (1-5)
  - Comentário: 10-500 caracteres (opcional)

#### SubCategoria
- ✅ `CreateSubCategoriaRequestValidator` - Validações:
  - Nome: 3-50 caracteres
  - CategoriaId obrigatória
  - URL da imagem (opcional)

### 3. Configuração no Program.cs

```csharp
// Adicionado no Program.cs:
using FluentValidation;
using FluentValidation.AspNetCore;

// Configuração FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Gemona.Application.Validators.Categoria.CreateCategoriaRequestValidator>();
```

### 4. Métodos Auxiliares Implementados

Todos os validators incluem métodos auxiliares para validações customizadas:

#### BeAValidUrl
Valida se a URL é válida e começa com http:// ou https://

#### BeAValidCpf
Implementa o algoritmo completo de validação de CPF:
- Remove caracteres especiais
- Verifica se tem 11 dígitos
- Verifica se não é sequência de números iguais
- Calcula dígitos verificadores usando multiplicadores
- Valida ambos os dígitos verificadores

#### BeAValidCnpj
Implementa o algoritmo completo de validação de CNPJ:
- Remove caracteres especiais
- Verifica se tem 14 dígitos
- Verifica se não é sequência de números iguais
- Calcula dígitos verificadores usando multiplicadores específicos do CNPJ
- Valida ambos os dígitos verificadores

## 🧪 Como Testar

### 1. Usando o arquivo Validators-Test.http

O arquivo `Validators-Test.http` contém diversos casos de teste para cada validator:

- **Categoria**: nome vazio, curto, com números, URL inválida, válido
- **Cliente**: CPF inválido, senha fraca, menor de idade, telefone inválido
- **Estabelecimento**: CNPJ inválido, estado inválido, CEP inválido, latitude fora do range
- **Serviço**: preço negativo, preço muito alto
- **Pedido**: data passada
- **Avaliação**: comentário curto, nota inválida

### 2. Testando via Swagger

1. Acesse: http://localhost:5268/swagger
2. Tente criar recursos com dados inválidos
3. Observe as mensagens de erro em português

### 3. Exemplos de Resposta de Validação

Quando um validator falha, a API retorna um response 400 Bad Request com os erros:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": [
      "Nome deve ter no mínimo 3 caracteres"
    ],
    "Cpf": [
      "CPF inválido"
    ],
    "Senha": [
      "Senha deve conter pelo menos uma letra maiúscula"
    ]
  }
}
```

## 📊 Validações Implementadas por Categoria

### Validações de Formato
- ✅ CPF (algoritmo completo)
- ✅ CNPJ (algoritmo completo)
- ✅ Email
- ✅ Telefone brasileiro
- ✅ CEP
- ✅ UF (2 caracteres)
- ✅ URL HTTP/HTTPS

### Validações de Negócio
- ✅ Idade mínima (18 anos)
- ✅ Data de agendamento futura
- ✅ Preço positivo e dentro do range
- ✅ Coordenadas geográficas válidas

### Validações de Tamanho
- ✅ Nome: 3-100 caracteres
- ✅ Descrição: 10-1000 caracteres
- ✅ Comentário: 10-500 caracteres
- ✅ Observações: máximo 500 caracteres

### Validações de Segurança
- ✅ Senha forte (maiúscula + minúscula + número)
- ✅ Tamanho mínimo de senha (6 caracteres)

## 🎯 Próximos Passos (Sprint 1)

Conforme o roadmap em ANÁLISE-PROJETO.md:

1. ✅ **Validações com FluentValidation** - CONCLUÍDO
2. ⏳ **Exception Handling Middleware** - Próximo
3. ⏳ **Custom Exceptions**
4. ⏳ **Serilog (logging)**
5. ⏳ **AutoMapper (configuração)**

## 📝 Observações Importantes

1. **Validações Automáticas**: Com `AddFluentValidationAutoValidation()`, as validações são executadas automaticamente antes dos controllers
2. **Mensagens em Português**: Todas as mensagens de erro estão em português brasileiro
3. **Performance**: As validações são executadas de forma eficiente antes de qualquer lógica de negócio
4. **Extensibilidade**: É fácil adicionar novos validators ou regras aos existentes

## 🔧 Correções Realizadas

Durante a implementação, os seguintes ajustes foram feitos nos validators:

1. **SubCategoria**: Corrigido nome da propriedade de `ImagemSubCategoriaUrl` para `ImagemSubcategoriaUrl` (minúsculo)
2. **Profissional**: Removida validação do campo `Especialidade` (não existe no DTO)
3. **Estabelecimento**: 
   - Corrigido campo `Logradouro` para `Rua`
   - Removido `.HasValue` de Latitude/Longitude (são decimal, não nullable)
4. **Pedido**: Removida validação de `ValorTotal` (não existe no DTO)

## ✅ Status Final

- **Compilação**: ✅ Sucesso
- **Aplicação**: ✅ Rodando em http://localhost:5268
- **Validators**: ✅ 11 validators criados e configurados
- **Testes**: ⏳ Arquivo de testes HTTP criado (pronto para testar)
