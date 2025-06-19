# Prueba Técnica Backend - Microservicios Productos e Inventario

## 🛠 Tecnologías usadas

- **.NET 8**
- **Docker + Docker Compose**
- **SQLite**
- **XUnit + Moq**
- **Swagger**
- **GitHub CLI**
- **Herramientas de IA**: ChatGPT para apoyo en validaciones, pruebas, errores comunes, generación de documentación y mejora de calidad del código.

---

## 🚀 Instrucciones de instalación y ejecución

1. Clonar el repositorio:

```bash
git clone https://github.com/carsepe/PruebaMicroservicios.git
cd PruebaMicroservicios
```

2. Ejecutar los servicios con Docker:

```bash
docker-compose up --build
```

3. Acceder a Swagger para probar los endpoints:

- Producto: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Inventario: [http://localhost:5001/swagger](http://localhost:5001/swagger)

**⚠️ Importante:** Importante: Tanto Producto como Inventario están protegidos mediante autenticación básica por API Key.
Por lo tanto, es obligatorio incluir el siguiente encabezado en todas las solicitudes hacia cualquiera de los dos microservicios:

```http
X-API-KEY: pruebamicroservicios-KEY-2025-123456789
```

---

## 🧱 Arquitectura

Se utilizaron dos microservicios independientes:

- **Producto**: CRUD completo con validación de nombre único.
- **Inventario**: Gestión de stock, proceso de compra y consulta de compras históricas.

Cada uno tiene la estructura:

- `API`: Controladores y validación JSON:API.
- `Application`: Interfaces y DTOs.
- `Infrastructure`: Servicios, DbContext y lógica.
- `Domain`: Entidades.
- `Tests`: Pruebas unitarias e integración.

---

## ⚙️ Decisiones técnicas

- **SQLite**: Base de datos ligera para facilitar despliegue local.
- **Compra en Inventario**: Por principios DDD, se valida y actualiza el stock ahí mismo.
- **Comunicación HTTP**: El microservicio de Inventario consume Producto vía HttpClient.
- **Autenticación básica**: API key en encabezado HTTP (`X-API-KEY`) entre microservicios.
- **Eliminación lógica**: Mediante campo `EsActivo`.
- **Respuestas JSON:API**: Estandarizadas para todos los errores.
- **Resiliencia**: Manejo de timeout y reintentos con Polly.

---

## 🔁 Flujo de compra

```text
[ Cliente ]
     |
     v
[Inventario API] ---> llama a ---> [Producto API]
     |                                   |
     v                                   v
[Verifica existencia y stock]     [Verifica si producto existe y está activo]
     |
     v
[Actualiza stock, guarda histórico y responde en formato JSON:API]
```

---

## 📂 Endpoints principales

### Producto API

- `GET /productos`
- `GET /productos/{id}`
- `POST /productos`
- `PUT /productos/{id}`
- `PATCH /productos/{id}/estado`

### Inventario API

- `GET /inventarios`
- `GET /inventarios/{productoId}`
- `POST /inventarios`
- `PUT /inventarios`
- `PATCH /inventarios/{id}/estado`
- `POST /compras`
- `GET /compras/historico` 

---

## ✅ Pruebas realizadas

- **Unitarias** con XUnit:
  - Producto: creación, validación de nombre duplicado, actualización y cambio de estado.
  - Inventario: creación, validación de stock, flujo de compra completo.
- **Integración**:
  - Compra real entre servicios.
  - Validación de errores personalizados.
  - Guardado de histórico en compras.
- **Cobertura completa de casos positivos y negativos.**

---

## 🤖 Apoyo con herramientas de IA

Se utilizó **ChatGPT** para:

- Generar controladores y servicios con buenas prácticas.
- Aplicar formato de errores JSON:API.
- Crear pruebas unitarias e integración.
- Agilizar documentación técnica.
- Diseñar diagramas de arquitectura y comunicación entre servicios.
- Mejorar estructura y legibilidad del código.

---

## 📌 Consideraciones finales

- Proyecto dockerizado y funcional en local.
- Flujo de compra implementado, probado y validado.
- Microservicios separados, comunicados por HTTP.
- Validaciones claras, errores personalizados y pruebas completas.
- Seguridad básica entre servicios vía `X-API-KEY`.
