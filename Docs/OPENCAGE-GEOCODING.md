# 🌍 OpenCage Geocoding - Documentação

## 📋 Visão Geral

O projeto Gemona utiliza a API do **OpenCage Geocoding** para obter coordenadas geográficas (latitude/longitude) a partir de endereços (CEP, rua, cidade, etc.) e vice-versa.

---

## 🔑 Configuração

### **API Key**
- Provedor: [OpenCage Data](https://opencagedata.com/)
- Limite Free: **2.500 requisições/dia**
- Protocolo: HTTPS
- Não requer cartão de crédito

### **appsettings.json**
```json
{
  "OpenCage": {
    "ApiKey": "b730f750896e4a74bf8bd350e5315d02"
  }
}
```

---

## 🏗️ Arquitetura

### **Localização dos Arquivos**

```
Gemona.Infrastructure/ExternalServices/OpenCage/
├── OpenCageGeocodingService.cs       # Implementação do serviço
└── Models/
    └── OpenCageModels.cs              # DTOs da API

Gemona.Application/
├── Interfaces/Services/
│   └── IGeocodingService.cs           # Interface do serviço
└── Helpers/
    └── GeoHelper.cs                   # Cálculo de distância (Haversine)
```

---

## 🔌 Interface `IGeocodingService`

### **Métodos Disponíveis**

#### 1. `BuscarPorCepAsync(string cep)`
Busca endereço completo e coordenadas a partir de um CEP brasileiro.

**Entrada:**
```json
"01310-100"
```

**Saída:**
```json
{
  "rua": "Avenida Paulista",
  "numero": "",
  "bairro": "Bela Vista",
  "cidade": "São Paulo",
  "estado": "São Paulo",
  "cep": "01310-100",
  "latitude": -23.561414,
  "longitude": -46.655882
}
```

#### 2. `BuscarCoordenadasAsync(string endereco)`
Busca apenas as coordenadas de um endereço completo.

**Entrada:**
```json
"Avenida Paulista, 1578, Bela Vista, São Paulo, SP, Brasil"
```

**Saída:**
```json
{
  "latitude": -23.561414,
  "longitude": -46.655882
}
```

---

## 📡 Endpoints da API

### **POST /api/endereco/buscar-por-cep**
Busca dados completos do endereço por CEP (incluindo coordenadas).

**Request:**
```json
{
  "cep": "01310-100"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Endereço encontrado com sucesso",
  "data": {
    "rua": "Avenida Paulista",
    "numero": "",
    "bairro": "Bela Vista",
    "cidade": "São Paulo",
    "estado": "São Paulo",
    "cep": "01310-100",
    "latitude": -23.561414,
    "longitude": -46.655882,
    "enderecoCompleto": "Avenida Paulista,  - Bela Vista, São Paulo/São Paulo"
  }
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "CEP não encontrado",
  "data": null
}
```

---

## 🤖 Integração Automática

### **ClienteService.CreateAsync**
Ao criar um cliente, se `Latitude` e `Longitude` não forem fornecidos (= 0), o sistema **automaticamente** busca as coordenadas usando o endereço completo.

```csharp
// Exemplo de Request
{
  "nome": "João Silva",
  "cpf": "12345678900",
  "email": "joao@email.com",
  "senha": "senha123",
  "rua": "Avenida Paulista",
  "numero": "1578",
  "bairro": "Bela Vista",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01310-100",
  "latitude": 0,      // ← Não fornecido
  "longitude": 0      // ← Não fornecido
}

// Sistema automaticamente busca coordenadas via OpenCage
// Resultado: latitude: -23.561414, longitude: -46.655882
```

### **EstabelecimentoService.CreateAsync**
Mesma lógica aplicada ao criar estabelecimentos.

---

## 📐 Cálculo de Distância (Haversine)

### **GeoHelper.CalcularDistancia()**

Calcula a distância entre dois pontos geográficos usando a **Fórmula de Haversine**.

**Implementação:**
```csharp
using Gemona.Application.Helpers;

var distanciaKm = GeoHelper.CalcularDistancia(
    lat1: -23.561414m,  // Ponto A
    lon1: -46.655882m,
    lat2: -23.550520m,  // Ponto B
    lon2: -46.633309m
);

// Resultado: ~2.5 km
```

### **Uso em GetEstabelecimentosProximosAsync**

```csharp
GET /api/estabelecimento/proximos?latitude=-23.561414&longitude=-46.655882&raioKm=5

// Retorna estabelecimentos dentro de 5km do ponto fornecido
// Ordenados por distância (mais próximo primeiro)
```

**Fluxo:**
1. Busca todos os estabelecimentos no banco
2. Para cada um, calcula distância usando Haversine
3. Filtra os que estão dentro do raio
4. Ordena por distância crescente

---

## 🔍 Estrutura dos Models

### **OpenCageResponse**
```csharp
{
  "results": [
    {
      "formatted": "Avenida Paulista, 1578, Bela Vista, São Paulo, SP, Brasil",
      "geometry": {
        "lat": -23.561414,
        "lng": -46.655882
      },
      "components": {
        "road": "Avenida Paulista",
        "house_number": "1578",
        "neighbourhood": "Bela Vista",
        "city": "São Paulo",
        "state": "São Paulo",
        "postcode": "01310-100",
        "country": "Brasil"
      }
    }
  ],
  "status": {
    "code": 200,
    "message": "OK"
  }
}
```

---

## ⚠️ Tratamento de Erros

### **Casos Tratados:**

1. **CEP não encontrado**
   - Retorna `null`
   - Log: `CEP {Cep} não encontrado`

2. **Erro na API (status != 200)**
   - Retorna `null`
   - Log: `Erro ao buscar CEP {Cep}: {StatusCode}`

3. **Exceção inesperada**
   - Retorna `null`
   - Log: `Erro ao buscar CEP {Cep}` + Exception

4. **API Key inválida/ausente**
   - `InvalidOperationException` no startup
   - Mensagem: "OpenCage ApiKey não configurada"

---

## 📊 Limitações

| Item | Limite |
|------|--------|
| Requisições/dia | 2.500 (Free) |
| Rate Limit | 1 req/segundo |
| Cache recomendado | Sim (opcional) |
| HTTPS | Obrigatório |

---

## 🧪 Testes Manuais

### **Teste 1: Buscar CEP**
```bash
POST http://localhost:5269/api/endereco/buscar-por-cep
Content-Type: application/json

{
  "cep": "01310-100"
}
```

### **Teste 2: Estabelecimentos Próximos**
```bash
GET http://localhost:5269/api/estabelecimento/proximos?latitude=-23.561414&longitude=-46.655882&raioKm=10
```

### **Teste 3: Criar Cliente (Geocoding Automático)**
```bash
POST http://localhost:5269/api/cliente
Content-Type: application/json

{
  "nome": "Teste",
  "cpf": "12345678900",
  "email": "teste@email.com",
  "senha": "senha123",
  "dataNascimento": "1990-01-01",
  "telefone": "11999999999",
  "rua": "Avenida Paulista",
  "numero": "1578",
  "bairro": "Bela Vista",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01310100",
  "latitude": 0,
  "longitude": 0
}
```

---

## 🚀 Melhorias Futuras (Opcional)

- ✅ Cache de coordenadas por CEP (Redis)
- ✅ Fallback para outros provedores (Google Maps, Nominatim)
- ✅ Rate limiting interno (1 req/s)
- ✅ Batch geocoding (múltiplos endereços)
- ✅ Validação de CEP antes de chamar API

---

## 📚 Referências

- [OpenCage API Docs](https://opencagedata.com/api)
- [Haversine Formula](https://en.wikipedia.org/wiki/Haversine_formula)
- [CEP Format (Brasil)](https://en.wikipedia.org/wiki/C%C3%B3digo_de_Endere%C3%A7amento_Postal)

---

**Última atualização:** 10 de novembro de 2025
