# 📸 Sistema de Upload de Imagens - Azure Blob Storage

**Data de Implementação:** 04 de Novembro de 2025  
**Versão:** 1.0.0  
**Status:** ✅ Implementado e Funcional

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Configuração](#configuração)
3. [Endpoints da API](#endpoints-da-api)
4. [Exemplos de Uso](#exemplos-de-uso)
5. [Validações](#validações)
6. [Estrutura do Código](#estrutura-do-código)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

Sistema completo de upload, download e gerenciamento de imagens usando **Azure Blob Storage**. Implementado com segurança, validações e integração perfeita com a API Gemona.

### ✅ **Funcionalidades:**

- ✅ **Upload de imagens** - Suporta JPG, PNG, WebP, GIF
- ✅ **Download de imagens** - Stream direto do Azure
- ✅ **Delete de imagens** - Com autorização (Admin/Profissional)
- ✅ **Obter URL pública** - Para exibição em front-end
- ✅ **Validações** - Tamanho, tipo, extensão
- ✅ **Logging** - Todas as operações registradas
- ✅ **Exception Handling** - Erros tratados pelo middleware global

---

## ⚙️ Configuração

### 📦 **Pacotes Instalados:**

```xml
<!-- Infrastructure -->
<PackageReference Include="Azure.Storage.Blobs" Version="12.19.1" />

<!-- Application -->
<PackageReference Include="Microsoft.AspNetCore.Http.Features" Version="5.0.17" />
```

### 🔐 **appsettings.json:**

```json
{
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=gemonastorage2025;AccountKey=***;EndpointSuffix=core.windows.net",
    "ContainerName": "images"
  }
}
```

**⚠️ IMPORTANTE:**  
- Nunca commite a connection string no Git
- Use **User Secrets** em desenvolvimento
- Use **Azure Key Vault** em produção

### 🏗️ **Arquitetura:**

```
┌─────────────────────────────────────────┐
│  Cliente (Front-end)                    │
│  - Formulário com input file            │
│  - Envia POST multipart/form-data       │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│  API Layer (ImageController)            │
│  - Validações de request                │
│  - Autorização                          │
│  - DTOs (Request/Response)              │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│  Infrastructure (BlobStorageService)    │
│  - Upload para Azure                    │
│  - Delete/Download                      │
│  - Geração de URL                       │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│  Azure Blob Storage                     │
│  - Região: Brazil South                │
│  - Container: images                    │
│  - Redundância: LRS                     │
└─────────────────────────────────────────┘
```

---

## 🌐 Endpoints da API

### 1. **POST /api/image/upload** - Upload de Imagem

**Descrição:** Faz upload de uma imagem para o Azure Blob Storage

**Autenticação:** Não requerida (pode adicionar se necessário)

**Content-Type:** `multipart/form-data`

**Request Body:**
```
FormData:
  Image: [arquivo]
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Imagem enviada com sucesso",
  "data": {
    "blobName": "a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg",
    "url": "https://gemonastorage2025.blob.core.windows.net/images/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg",
    "fileName": "foto.jpg",
    "size": 245678,
    "contentType": "image/jpeg",
    "uploadedAt": "2025-11-04T20:30:00Z"
  },
  "errors": null
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "A imagem excede o tamanho máximo permitido de 5MB",
  "data": null,
  "errors": ["A imagem excede o tamanho máximo permitido de 5MB"]
}
```

---

### 📦 **Upload via Base64 (Integrado nos DTOs)**

Todos os DTOs de criação/atualização que incluem imagem suportam upload via **Base64** através do objeto `ImageUploadDto`:

**Estrutura do ImageUploadDto:**
```json
{
  "fileName": "string",
  "contentType": "string",
  "base64Data": "string"
}
```

**Campos:**
- **`fileName`**: Nome do arquivo com extensão (ex: `"perfil.jpg"`, `"foto.png"`)
  - Usado para validar a extensão (`.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`)
- **`contentType`**: Tipo MIME da imagem
  - `"image/jpeg"` para `.jpg/.jpeg`
  - `"image/png"` para `.png`
  - `"image/webp"` para `.webp`
  - `"image/gif"` para `.gif`
- **`base64Data`**: Imagem codificada em Base64 (sem o prefixo `data:image/...;base64,`)

**Exemplo de uso em CreateProfissionalRequest:**
```json
{
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "telefone": "11987654321",
  "cpf": "12345678901",
  "imagemPerfil": {
    "fileName": "minha-foto.jpg",
    "contentType": "image/jpeg",
    "base64Data": "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAgGBgcGBQ..."
  },
  "dataNascimento": "1990-05-15T00:00:00.000Z",
  "senha": "SenhaSegura123!"
}
```

**Validações aplicadas:**
- ✅ Extensão válida (via `fileName`)
- ✅ Base64 válido (decodificação bem-sucedida)
- ✅ Imagem válida (headers verificados)
- ✅ Tamanho máximo: 5MB
- ✅ Campo **opcional** - pode criar entidade sem imagem

**Entities que suportam Base64:**
- `Profissional` → `imagemPerfil`
- `Cliente` → `imagemPerfil`
- `Estabelecimento` → `imagemEstabelecimento`
- `Servico` → `imagemServico`
- `Avaliacao` → `imagemComentario`

**Nota:** Ao enviar imagem via Base64, o upload para Azure é feito automaticamente e a URL é salva no banco de dados.

---

### 2. **DELETE /api/image/{blobName}** - Deletar Imagem

**Descrição:** Deleta uma imagem do Azure Blob Storage

**Autenticação:** Requerida (`Admin` ou `Profissional`)

**Authorization:** `Bearer {token}`

**URL Parameters:**
- `blobName`: Nome do blob (retornado no upload)

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Imagem deletada com sucesso",
  "data": true,
  "errors": null
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Imagem não encontrada",
  "data": null,
  "errors": ["Imagem com identificador 'xyz' não foi encontrada"]
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Não autorizado. Token inválido ou ausente."
}
```

---

### 3. **GET /api/image/{blobName}** - Download de Imagem

**Descrição:** Baixa uma imagem do Azure Blob Storage

**Autenticação:** Não requerida (`[AllowAnonymous]`)

**URL Parameters:**
- `blobName`: Nome do blob

**Response (200 OK):**
- **Content-Type:** `image/jpeg`, `image/png`, `image/gif`, etc.
- **Body:** Stream binário da imagem

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Imagem não encontrada",
  "data": null,
  "errors": ["Imagem com identificador 'xyz' não foi encontrada"]
}
```

**Uso em HTML:**
```html
<img src="https://api.gemona.com/api/image/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg" alt="Foto">
```

---

### 4. **GET /api/image/{blobName}/url** - Obter URL da Imagem

**Descrição:** Retorna a URL pública da imagem no Azure

**Autenticação:** Não requerida

**URL Parameters:**
- `blobName`: Nome do blob

**Response (200 OK):**
```json
{
  "success": true,
  "message": "URL obtida com sucesso",
  "data": "https://gemonastorage2025.blob.core.windows.net/images/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg",
  "errors": null
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Imagem não encontrada",
  "data": null,
  "errors": ["Imagem com identificador 'xyz' não foi encontrada"]
}
```

---

## 📘 Exemplos de Uso

### **1. Upload via cURL**

```bash
curl -X POST https://api.gemona.com/api/image/upload \
  -H "Content-Type: multipart/form-data" \
  -F "Image=@/path/to/image.jpg"
```

### **2. Upload via PowerShell**

```powershell
$filePath = "C:\Users\mauri\Pictures\foto.jpg"
$uri = "http://localhost:5268/api/image/upload"

$form = @{
    Image = Get-Item -Path $filePath
}

$response = Invoke-RestMethod -Uri $uri -Method Post -Form $form
$response | ConvertTo-Json -Depth 5
```

### **3. Upload via JavaScript (Fetch API)**

```javascript
async function uploadImage(file) {
    const formData = new FormData();
    formData.append('Image', file);

    const response = await fetch('https://api.gemona.com/api/image/upload', {
        method: 'POST',
        body: formData
    });

    const result = await response.json();
    console.log('Upload result:', result);
    
    if (result.success) {
        console.log('Image URL:', result.data.url);
        console.log('Blob Name:', result.data.blobName);
    }
}

// Uso com input file
document.getElementById('imageInput').addEventListener('change', (e) => {
    const file = e.target.files[0];
    uploadImage(file);
});
```

### **4. Upload via React**

```jsx
import { useState } from 'react';

function ImageUpload() {
    const [uploading, setUploading] = useState(false);
    const [imageUrl, setImageUrl] = useState('');

    const handleUpload = async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        setUploading(true);

        const formData = new FormData();
        formData.append('Image', file);

        try {
            const response = await fetch('https://api.gemona.com/api/image/upload', {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                setImageUrl(result.data.url);
                alert('Upload successful!');
            } else {
                alert('Upload failed: ' + result.message);
            }
        } catch (error) {
            alert('Error uploading image');
        } finally {
            setUploading(false);
        }
    };

    return (
        <div>
            <input 
                type="file" 
                onChange={handleUpload} 
                accept="image/jpeg,image/png,image/webp,image/gif"
                disabled={uploading}
            />
            {uploading && <p>Uploading...</p>}
            {imageUrl && <img src={imageUrl} alt="Uploaded" />}
        </div>
    );
}
```

### **5. Delete via cURL (com autenticação)**

```bash
curl -X DELETE https://api.gemona.com/api/image/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### **6. Exibir imagem em HTML**

```html
<!-- Opção 1: URL do Azure (direta) -->
<img src="https://gemonastorage2025.blob.core.windows.net/images/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg" 
     alt="Foto do estabelecimento">

<!-- Opção 2: Via API (passa pelo controller) -->
<img src="https://api.gemona.com/api/image/a1b2c3d4-5678-90ab-cdef-123456789abc_foto.jpg" 
     alt="Foto do estabelecimento">
```

---

## ✅ Validações

### **Tamanho do Arquivo:**
- **Máximo:** 5 MB (5.242.880 bytes)
- **Mínimo:** > 0 bytes (arquivo não pode estar vazio)

### **Tipos de Arquivo Permitidos:**

| Extensão | Content-Type | Suportado |
|----------|--------------|-----------|
| `.jpg`, `.jpeg` | `image/jpeg` | ✅ |
| `.png` | `image/png` | ✅ |
| `.webp` | `image/webp` | ✅ |
| `.gif` | `image/gif` | ✅ |
| `.bmp`, `.svg`, `.tiff` | - | ❌ |

### **Mensagens de Erro:**

- `"Nenhuma imagem foi enviada"` - Campo Image vazio
- `"A imagem excede o tamanho máximo permitido de 5MB"` - Arquivo muito grande
- `"Tipo de arquivo não permitido"` - Content-Type inválido
- `"Extensão de arquivo não permitida"` - Extensão não suportada

---

## 🏗️ Estrutura do Código

### **1. BlobStorageService (Infrastructure)**

```csharp
// Localização: Gemona.Infrastructure/Services/BlobStorageService.cs

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string blobName);
    Task<Stream> DownloadImageAsync(string blobName);
    Task<bool> BlobExistsAsync(string blobName);
    string GetBlobUrl(string blobName);
}
```

**Responsabilidades:**
- Comunicação com Azure Blob Storage
- Upload/Download/Delete de blobs
- Geração de URLs
- Logging de operações

---

### **2. ImageController (API)**

```csharp
// Localização: Gemona.API/Controllers/ImageController.cs

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    // 4 endpoints: Upload, Delete, Download, GetUrl
}
```

**Responsabilidades:**
- Receber requisições HTTP
- Validar requests
- Chamar BlobStorageService
- Retornar responses padronizadas

---

### **3. DTOs**

```csharp
// Request
public class ImageUploadRequest
{
    public IFormFile? Image { get; set; }
}

// Response
public class ImageUploadResponse
{
    public string BlobName { get; set; }
    public string Url { get; set; }
    public string FileName { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

---

### **4. Validator (FluentValidation)**

```csharp
// Localização: Gemona.Application/Validators/Image/ImageUploadRequestValidator.cs

public class ImageUploadRequestValidator : AbstractValidator<ImageUploadRequest>
{
    // Validações:
    // - Image não null
    // - Tamanho <= 5MB
    // - Content-Type permitido
    // - Extensão permitida
}
```

---

## 🔧 Troubleshooting

### ❌ **Erro: "Azure Storage connection string não configurada"**

**Causa:** Connection string ausente no `appsettings.json`

**Solução:**
```json
{
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "ContainerName": "images"
  }
}
```

---

### ❌ **Erro: "Unable to connect to Azure Storage"**

**Causa:** Connection string inválida ou conta de armazenamento inexistente

**Solução:**
1. Verificar connection string no portal Azure
2. Copiar novamente as chaves de acesso
3. Verificar se a conta de armazenamento está ativa

---

### ❌ **Erro: "The specified container does not exist"**

**Causa:** Container `images` não foi criado no Azure

**Solução:**
1. Acessar a conta de armazenamento no portal Azure
2. Ir em "Containers"
3. Criar container chamado `images`
4. Nível de acesso: **Privado**

---

### ❌ **Erro: 401 Unauthorized ao deletar**

**Causa:** Token JWT ausente ou inválido

**Solução:**
```bash
# Fazer login primeiro
curl -X POST https://api.gemona.com/api/auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@gemona.com","senha":"Admin@123"}'

# Copiar o token da resposta

# Usar o token no delete
curl -X DELETE https://api.gemona.com/api/image/{blobName} \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### ❌ **Erro: "A imagem excede o tamanho máximo"**

**Causa:** Arquivo maior que 5MB

**Solução:**
1. Comprimir a imagem antes do upload
2. Ou aumentar o limite no código (não recomendado para produção)

```csharp
// Para aumentar limite (em ImageController.cs):
const long maxFileSize = 10 * 1024 * 1024; // 10MB
```

---

### ❌ **Erro: CORS ao fazer upload do front-end**

**Causa:** Política CORS não permite origem do front-end

**Solução:**
```csharp
// No Program.cs, adicionar origem do front-end:
options.AddPolicy("Development", policy =>
{
    policy.WithOrigins(
        "http://localhost:3000",      // React
        "http://localhost:4200",      // Angular
        "http://localhost:5173",      // Vite
        "http://localhost:8080",      // Vue
        "https://seufrontend.com"     // Produção
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});
```

---

## 🚀 Próximas Melhorias

### **Sugestões de Features:**

1. **Redimensionamento automático**
   ```csharp
   // Criar thumbnail automático (300x300)
   // Usando ImageSharp ou SkiaSharp
   ```

2. **Múltiplos uploads**
   ```csharp
   public async Task<List<ImageUploadResponse>> UploadMultiple(List<IFormFile> images)
   ```

3. **Upload com progresso**
   ```javascript
   // Usando XMLHttpRequest ou Axios
   xhr.upload.onprogress = (e) => {
       const percent = (e.loaded / e.total) * 100;
       console.log(`Upload: ${percent}%`);
   };
   ```

4. **Integração com entidades**
   ```csharp
   // Adicionar campo ImageUrl nas entidades:
   public string? ImagemPerfilUrl { get; set; }
   public string? ImagemCapaUrl { get; set; }
   ```

5. **Soft delete de imagens**
   ```csharp
   // Mover para container "deleted" ao invés de deletar permanentemente
   ```

6. **CDN na frente do Blob Storage**
   ```
   Azure CDN → Azure Blob Storage
   - Cache distribuído globalmente
   - Latência reduzida
   ```

7. **Watermark automático**
   ```csharp
   // Adicionar marca d'água em imagens enviadas
   ```

8. **Compressão automática**
   ```csharp
   // Reduzir qualidade JPEG para economizar storage
   ```

---

## 📊 Estatísticas de Implementação

### **Arquivos Criados/Modificados:**

| Arquivo | Tipo | Linhas | Status |
|---------|------|--------|--------|
| `BlobStorageService.cs` | Service | 150 | ✅ Criado |
| `ImageController.cs` | Controller | 175 | ✅ Criado |
| `ImageUploadRequest.cs` | DTO | 8 | ✅ Criado |
| `ImageUploadResponse.cs` | DTO | 10 | ✅ Criado |
| `ImageUploadRequestValidator.cs` | Validator | 47 | ✅ Criado |
| `appsettings.json` | Config | +5 | ✅ Modificado |
| `ServiceCollectionExtensions.cs` | DI | +1 | ✅ Modificado |

**Total:** 7 arquivos, ~396 linhas de código

---

## ✅ Checklist de Implementação

- [x] Pacote Azure.Storage.Blobs instalado
- [x] BlobStorageService criado
- [x] Interface IBlobStorageService definida
- [x] ImageController criado com 4 endpoints
- [x] DTOs Request/Response criados
- [x] Validator com FluentValidation criado
- [x] Connection string configurada no appsettings.json
- [x] Serviço registrado no DI
- [x] Build bem-sucedido
- [x] Documentação completa criada

---

**Implementado por:** Mauricio Costa  
**Data:** 04 de Novembro de 2025  
**Projeto:** Gemona API - TCC  
**Versão da API:** 1.0.0
