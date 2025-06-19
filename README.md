# Prueba Técnica Backend - Microservicios (.NET 8)

## 🧱 Arquitectura

Este proyecto está compuesto por **dos microservicios** desarrollados con .NET 8 y arquitectura por capas:

- **Producto.API**: CRUD de productos.
- **Inventario.API**: CRUD de inventarios y flujo de compras.

Cada microservicio contiene sus propias capas: API, Application, Domain, Infrastructure, Tests.

## 📦 Flujo de Compra

El flujo de compra se encuentra en el microservicio de **Inventario** por las siguientes razones:

- El stock pertenece al dominio de Inventario.
- Se requiere consumir el microservicio de Producto para validar existencia y estado del producto.
- Centralizar la lógica de consumo y validación garantiza la cohesión del flujo de negocio.

## 🔧 Cómo levantar el proyecto

### Requisitos

- Docker
- Docker Compose

### Comando

```bash
docker-compose up --build
