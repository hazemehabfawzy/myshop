# 🚀 TechVault Electronic Shop API

**TechVault** is a premium, enterprise-grade ASP.NET Core 10 Web API designed for a modern electronics retailer and repair specialist. It provides a robust backend infrastructure for managing high-end hardware sales, repair services, and background maintenance tasks.

---

## 🌟 Key Features

- **🛍️ Comprehensive Catalog**: Manage multi-level categories, products, and tags for a seamless browsing experience.
- **🛠️ Service Requests**: Specialized system for tracking repair services and custom PC builds.
- **🔐 Secure Authentication**: Role-based access control (Admin, Technician, Customer) powered by JWT and BCrypt hashing.
- **📦 Containerized Workflow**: Fully Dockerized environment for consistent deployment across any system.
- **⏰ Background Automation**: Leverages Hangfire for automated stock reporting and order monitoring.
- **📊 Optimized Mapping**: Efficient data transfer using AutoMapper for clean architectural separation.

---

## 🛠️ Technology Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 10 (ASP.NET Core) |
| **ORM** | Entity Framework Core 10 |
| **Database** | Microsoft SQL Server 2022 |
| **Authentication** | JWT Bearer Tokens |
| **Background Jobs** | Hangfire |
| **API Docs** | Swagger / OpenAPI |
| **Containerization** | Docker & Docker Compose |

---

## 🚀 Quick Start (Docker)

The fastest way to get TechVault up and running is using Docker Compose.

1. **Clone the repository**
2. **Launch the stack**:
   ```bash
   docker compose up --build
   ```
3. **Explore the API**:
   - **Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - **Hangfire Dashboard**: [http://localhost:8080/hangfire](http://localhost:8080/hangfire)

---

## 🛡️ Seed Accounts

Use these pre-configured accounts to test the different role permissions:

| Role | Username | Password | Purpose |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `Admin123!` | Full system management |
| **Technician** | `tech1` | `Tech123!` | Management of service requests |
| **Customer** | `john` | `Customer123!` | Browsing and ordering |

---

## 📐 System Architecture

The project follows a **Service-Oriented Architecture (SOA)**:
- **Controllers**: Thin entry points handling HTTP requests/responses.
- **Services**: Encapsulated business logic and validation.
- **Data**: Entity Framework Core context and automated migrations for Docker.
- **Models/DTOs**: Clear separation between database entities and API contracts.

---

## 🔒 Security Best Practices

We prioritize security by implementing:
- **BCrypt Password Hashing**: Slow hashing to mitigate brute-force attacks.
- **JWT Scope Management**: Granular permissions via Role-based Authorization.
- **SQL Injection Protection**: Fully parameterized queries via EF Core.
- **Environment Isolation**: Secure configuration management via Docker environment variables.

---

## 📡 API Endpoints Summary

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Public | Create a new customer account |
| `POST` | `/api/auth/login` | Public | Authenticated login & JWT issuance |
| `GET` | `/api/products` | Public | List all active inventory |
| `POST` | `/api/products` | Admin | Add new stock to the catalog |
| `POST` | `/api/orders` | Customer | Submit a new purchase order |
| `GET` | `/api/orders/my` | User | View personal purchase history |
| `PUT` | `/api/service-requests/{id}/status` | Tech | Update status of a repair |

---

Developed with ❤️ for the TechVault Community.

