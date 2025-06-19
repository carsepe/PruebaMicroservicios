# Prueba Técnica Backend - Microservicios Productos e Inventario

## 🛠 Tecnologías usadas

- **.NET 8**
- **Docker + Docker Compose**
- **SQLite**
- **XUnit + Moq**
- **Swagger**
- **GitHub CLI**
- **Herramientas de IA**: ChatGPT para apoyo en validaciones, pruebas, errores comunes y estructura del proyecto.

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

---

## 🧱 Arquitectura

Se utilizaron dos microservicios independientes:

- **Producto**: CRUD completo con validación de nombre único.
- **Inventario**: Gestión de stock y proceso de compra con validaciones.

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
- **Eliminación lógica**: Mediante campo `EsActivo`.
- **Respuestas JSON:API**: Estandarizadas para todos los errores.

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
[Actualiza stock y responde en formato JSON:API]
```

---

## ✅ Pruebas realizadas

- **Unitarias** con XUnit:
  - Producto: creación, validación de nombre duplicado, actualización y cambio de estado.
  - Inventario: creación, validación de stock, flujo de compra completo.
- **Integración**:
  - Compra real entre servicios.
  - Validación de errores personalizados.
- **Cobertura completa de casos positivos y negativos.**

---

## 🤖 Apoyo con herramientas de IA

Se utilizó **ChatGPT** para:

- Generar controladores y servicios con buenas prácticas.
- Aplicar formato de errores JSON:API.
- Generar validaciones de negocio.
- Crear pruebas unitarias e integración.
- Optimizar contenido del `README.md`.

---

## 📌 Consideraciones finales

- Proyecto dockerizado y funcional en local.
- Todo el flujo de compra implementado, probado y validado.
- Microservicios separados, comunicados por HTTP.
- Validaciones claras, errores personalizados y pruebas completas.
- Se cumplieron todos los puntos exigidos en la prueba para nivel **semi senior**.