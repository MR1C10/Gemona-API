# Exception Handling - Implementação Completa

## O que foi implementado

### 1. Custom Exceptions (3 classes)

#### 📁 `Gemona.Application/Exceptions/`

**NotFoundException.cs**
```csharp
// Uso: Quando um recurso não é encontrado
throw new NotFoundException("Categoria", 123);
// Retorna: HTTP 404 - "Categoria com ID '123' não foi encontrado(a)."
```

**BusinessException.cs**
```csharp
// Uso: Para regras de negócio violadas
throw new BusinessException("Não é possível excluir categoria com subcategorias ativas");
// Retorna: HTTP 400 - Mensagem customizada
```

**UnauthorizedException.cs**
```csharp
// Uso: Quando usuário não tem permissão
throw new UnauthorizedException();
// Retorna: HTTP 401 - "Você não tem permissão para acessar este recurso."
```

### 2. Global Exception Handler Middleware

#### 📁 `Gemona.API/Middlewares/GlobalExceptionHandlerMiddleware.cs`

**Funcionalidades:**
- ✅ Captura TODAS as exceções não tratadas da aplicação
- ✅ Retorna respostas JSON padronizadas
- ✅ Status codes corretos por tipo de exceção
- ✅ Mensagens em português
- ✅ TraceId para rastreamento
- ✅ Em desenvolvimento: mostra detalhes técnicos
- ✅ Em produção: oculta detalhes sensíveis

**Mapeamento de Exceções:**

| Exceção | Status Code | Uso |
|---------|-------------|-----|
| NotFoundException | 404 Not Found | Recurso não existe |
| UnauthorizedException | 401 Unauthorized | Sem permissão |
| BusinessException | 400 Bad Request | Regra de negócio violada |
| ArgumentException | 400 Bad Request | Argumento inválido |
| Outras exceções | 500 Internal Server Error | Erros inesperados |

### 3. Formato de Resposta de Erro

#### Estrutura JSON Padronizada:

**Desenvolvimento:**
```json
{
  "success": false,
  "message": "Categoria com ID '999' não foi encontrado(a).",
  "errors": [
    "Categoria com ID '999' não foi encontrado(a)."
  ],
  "traceId": "0HN7S7KQVJG8Q:00000001",
  "details": {
    "type": "NotFoundException",
    "stackTrace": "...",
    "innerException": null
  }
}
```

**Produção:**
```json
{
  "success": false,
  "message": "Ocorreu um erro interno no servidor.",
  "errors": [
    "Um erro inesperado ocorreu. Por favor, tente novamente mais tarde."
  ],
  "traceId": "0HN7S7KQVJG8Q:00000001"
}
```

### 4. Configuração no Program.cs

```csharp
// Deve ser o PRIMEIRO middleware no pipeline
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
```

**Ordem correta dos middlewares:**
1. GlobalExceptionHandlerMiddleware ← PRIMEIRO
2. Swagger (Development)
3. CORS
4. HTTPS Redirection
5. Authentication
6. Authorization
7. MapControllers

### 5. Endpoints de Teste

#### 📁 `TestController.cs` - Novos endpoints para testar:

```http
GET /api/test/exception/notfound      → 404
GET /api/test/exception/business      → 400
GET /api/test/exception/unauthorized  → 401
GET /api/test/exception/argument      → 400
GET /api/test/exception/generic       → 500
```

#### 📁 `Exception-Test.http` - Arquivo de testes pronto

## 🧪 Como Testar

### 1. Via Arquivo HTTP (Recomendado)
Abra `Exception-Test.http` e execute cada request para ver as respostas.

### 2. Via Swagger
1. Acesse: http://localhost:5268/swagger
2. Vá em `Test` controller
3. Execute os endpoints `exception/*`

### 3. Via Browser
```
http://localhost:5268/api/test/exception/notfound
```

## 📝 Exemplo de Uso nos Services

### Antes (sem custom exceptions):
```csharp
var categoria = await _categoriaRepository.GetByIdAsync(id);
if (categoria == null)
{
    return ApiResponse<CategoriaResponse?>.ErrorResult("Categoria não encontrada");
}
```

### Depois (com custom exceptions):
```csharp
var categoria = await _categoriaRepository.GetByIdAsync(id);
if (categoria == null)
{
    throw new NotFoundException("Categoria", id);
}
```

**Vantagens:**
- ✅ Código mais limpo (sem try-catch em todo lugar)
- ✅ Status code correto automático (404)
- ✅ Resposta padronizada
- ✅ Logging automático
- ✅ TraceId para rastreamento

## 🎯 Próximos Passos

### Aplicar nos Services existentes:
1. Atualizar todos os `GetByIdAsync` para usar `NotFoundException`
2. Adicionar `BusinessException` em regras de negócio
3. Usar `UnauthorizedException` em validações de permissão

### Exemplo de lugares para aplicar:

**CategoriaService:**
```csharp
// ✅ Já atualizado no GetByIdAsync
if (categoria == null) throw new NotFoundException("Categoria", id);
```

**ClienteService:**
```csharp
// Ao tentar atualizar cliente que não existe
if (cliente == null) throw new NotFoundException("Cliente", id);

// Ao tentar criar cliente com email duplicado
if (await EmailJaExiste(email))
    throw new BusinessException("Email já cadastrado no sistema");
```

**PedidoService:**
```csharp
// Ao tentar cancelar pedido já concluído
if (pedido.Status == StatusPedido.Concluido)
    throw new BusinessException("Não é possível cancelar um pedido já concluído");
```

## ✅ Benefícios Implementados

1. **Segurança**: Não vaza stack traces em produção
2. **Consistência**: Todas as respostas de erro seguem o mesmo padrão
3. **Rastreabilidade**: TraceId em cada resposta
4. **Logging**: Todas as exceções são logadas automaticamente
5. **Manutenibilidade**: Código mais limpo sem try-catch repetitivos
6. **Developer Experience**: Mensagens claras em português
7. **Debugging**: Detalhes técnicos em ambiente de desenvolvimento

## 📊 Status

- ✅ Custom Exceptions criadas (3)
- ✅ Middleware implementado
- ✅ Configurado no Program.cs
- ✅ Endpoints de teste criados
- ✅ Arquivo de teste HTTP pronto
- ✅ Exemplo aplicado em CategoriaService
- ⏳ Pendente: aplicar em todos os services (tarefa futura)

## 🔍 Observações

- O middleware está configurado para ser o **primeiro** no pipeline
- Em **desenvolvimento**: mostra detalhes técnicos completos
- Em **produção**: oculta informações sensíveis
- Todas as exceções são **logadas** automaticamente via ILogger nativo
- O `TraceId` pode ser usado para correlacionar logs

---

**Sistema de Exception Handling concluído!** ✅
