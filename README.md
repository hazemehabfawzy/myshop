# 🌌 TechVault — Premium Electronics & Repair Hub

**TechVault** is an enterprise-grade, full-stack stock management and repair service tracking platform. Built with a high-performance **ASP.NET Core Web API** backend and a sleek **React (Vite)** frontend, TechVault provides standard and admin dashboards to purchase flagship hardware, manage inventory levels in real-time, and coordinate technical service requests.

---

## 🛠️ Technology Stack

TechVault's enterprise stack comprises:

*   **Backend**: ASP.NET Core Web API (.NET 8)
*   **Frontend**: React (Vite, Vanilla CSS, Responsive Components)
*   **Database**: Microsoft SQL Server 2022
*   **Authentication**: Role-Based Authorization using JWT (JSON Web Tokens)
*   **Background Jobs**: Hangfire (Database automations and stock reports)
*   **Mapping**: AutoMapper (DTO architectural separation)
*   **Containerization**: Docker & Docker Compose

---

## 🌟 Key Features

*   **🛍️ Premium Inventory Management**: Catalog browsing with advanced search and real-time visual stock status indicators (In Stock, Low Stock, and Out of Stock badges).
*   **📊 Real-time Stock Control**: Safe partial updates with mapping guards protecting database references.
*   **🔐 Tiered Authentication**: Secure login and registration with token persistence for Administrator, Technician, and standard Customer roles.
*   **🛠️ Service Tracking**: Create and assign technical repair requests for hardware diagnostics.
*   **🛒 Shopping Cart & Orders**: Fully integrated checkout flow with transaction state-tracking and personal purchase logs.

---

## 🚀 Installation & Setup

Follow these simple steps to run TechVault locally on your machine using Docker:

### Prerequisites
*   **Docker Desktop** (running)
*   **Node.js** (v18.0.0 or higher)
*   **npm** (bundled with Node.js)

---

### Method A: Run with Docker Compose (Recommended)

1. **Navigate to the Project Directory**:
   ```cmd
   cd d:\projects\TechVault
   ```

2. **Launch the DB and API backend in Docker**:
   ```cmd
   docker compose up -d
   ```
   *   *The API backend will boot on:* `http://localhost:8080`
   *   *Explore Swagger documentation at:* `http://localhost:8080/swagger`

3. **Open the Frontend folder**:
   ```cmd
   cd techvault-frontend
   ```

4. **Install UI dependencies**:
   ```cmd
   npm install
   ```

5. **Start the Frontend development server**:
   ```cmd
   npm run dev
   ```
   *   *The Client will run on:* **`http://localhost:5173`**

---

### Method B: Run C# Backend Locally (Host Machine)

1. **Launch the SQL Server Container**:
   ```cmd
   docker compose up -d db
   ```

2. **Run C# Backend from Root folder**:
   ```cmd
   cd d:\projects\TechVault
   dotnet run
   ```

3. **Launch Frontend**:
   ```cmd
   cd techvault-frontend
   npm run dev
   ```

---

## 🔐 Seed Accounts

Use the following synchronized database accounts to test different roles:

| Role | Username | Password | Purpose / Permissions |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin` | `123456` | Full catalog edits, stock management, and service control |
| **Technician** | `tech` | `123456` | Diagnostic review and status updates on repairs |
| **Customer** | `hazem` | `123456` | Premium hardware purchase, cart, and repair tickets |

---

Developed with ❤️ for the TechVault Community.
