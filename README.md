# Lautus Informática - Backend
[![Ask DeepWiki](https://devin.ai/assets/askdeepwiki.png)](https://deepwiki.com/diogolpievan/lautus-informatica-backend)

This repository contains the backend for Lautus Informática, a management system for a computer repair and services business. Developed with .NET 8 and ASP.NET Core, it provides a robust RESTful API to handle operations such as user management, service orders, inventory control, and activity logging. The system is designed to interact with a MySQL database, utilizing both Entity Framework Core for ORM and Dapper for optimized stored procedure execution.

## Features

*   **Authentication & Authorization:** Secure user login and registration using JWT (JSON Web Tokens). Role-based access control distinguishes between `Admin` and `Client` users.
*   **User Management:** Full CRUD (Create, Read, Update, Delete) operations for users. Admins can manage all users, while clients can manage their own profiles. Includes features for password changes and user locking after multiple failed login attempts.
*   **Service Order Management:** Comprehensive tracking of service orders from creation to completion. Each order includes details like equipment, problem description, service price, and status (e.g., Pending, In Progress, Completed).
*   **Inventory (Item) Management:** Manage a catalog of products and parts. Includes CRUD operations for items, stock quantity adjustments, and categorization (Hardware, Software, etc.).
*   **Used Item Tracking:** Associate items from inventory with specific service orders. The system automatically deducts used items from the stock.
*   **Auditing and Logging:** All critical database operations (creations, updates, deletions) are logged for security and auditing purposes, recording which user performed the action.
*   **Custom Exception Handling:** A global middleware ensures consistent, informative error responses for a better developer experience.

## Technology Stack

*   **Framework:** .NET 8, ASP.NET Core 8 Web API
*   **Database:** MySQL
*   **Data Access:**
    *   Entity Framework Core 9 (for ORM and migrations)
    *   Dapper (for high-performance stored procedure execution)
*   **Authentication:** JWT (JSON Web Tokens)
*   **API Documentation:** Swashbuckle (Swagger)
*   **Containerization:** Docker

## Project Architecture

The application is structured following a clean, layered architecture to ensure separation of concerns and maintainability.

*   **Controllers (`/Controllers`):** Handle incoming HTTP requests, validate input, and delegate business logic to the service layer.
*   **Services (`/Services`):** Encapsulate the core business logic of the application. They coordinate with repositories to perform operations.
*   **Repositories (`/Repositories`):** Abstract the data access layer, interacting directly with the database using both EF Core and Dapper.
*   **Models (`/Models`):** Define the core domain entities (e.g., `User`, `Item`, `ServiceOrder`).
*   **DTOs (`/DTOs`):** Data Transfer Objects used to shape the data sent to and from the API, preventing over-posting and separating API contracts from domain models.
*   **Migrations (`/Migrations`):** EF Core migrations define the database schema and include SQL scripts to create necessary stored procedures.
*   **Middleware (`/Middlewares`):** Custom middleware for global exception handling.

## Database Schema and Stored Procedures

The database is built around several key entities:
*   `Users`: Stores client and admin information, including credentials and lock status.
*   `Items`: Manages the inventory stock.
*   `ServiceOrders`: Tracks service requests, linked to a client (`User`).
*   `UsedItems`: A join table connecting `ServiceOrders` and `Items` to track parts used.
*   `Logs`: Records all CUD (Create, Update, Delete) operations with user and timestamp details.

The system heavily relies on MySQL Stored Procedures for complex and transactional database operations to enhance security and performance. Key procedures include:
*   `sp_CreateUser`, `sp_UpdateUser`, `sp_ExcluirUsuario` (Delete User)
*   `sp_ValidaLogin`, `sp_TrocarSenha` (Change Password), `sp_DesbloquearUsuario` (Unlock User)
*   `sp_CreateItem`, `sp_UpdateItem`, `sp_DeleteItem`, `sp_AdjustStock`
*   `sp_CreateServiceOrder`, `sp_UpdateServiceOrder`, `sp_DeleteServiceOrder`
*   `sp_CreateUsedItem`, `sp_UpdateUsedItem`, `sp_DeleteUsedItem`

## Getting Started

### Prerequisites
*   .NET 8 SDK
*   MySQL Server
*   A code editor like Visual Studio or VS Code
*   Docker (Optional, for containerized deployment)

### 1. Clone the Repository
```bash
git clone https://github.com/diogolpievan/lautus-informatica-backend.git
cd lautus-informatica-backend
```

### 2. Configure the Database
1.  Ensure your MySQL server is running.
2.  Create a new database named `lautus_informatica`.
    ```sql
    CREATE DATABASE lautus_informatica;
    ```
3.  Open the `LautusInformatica/appsettings.json` file and update the `DefaultConnection` string with your MySQL credentials (user, password, server, port).
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "server=localhost;port=3306;database=lautus_informatica;user=root;password=your_password;Charset=utf8mb4"
    }
    ```

### 3. Apply Migrations
The migrations will set up all tables and stored procedures automatically. Navigate to the project directory and run the following command:
```bash
cd LautusInformatica
dotnet ef database update
```
This will also create a default super admin user with the following credentials:
*   **Email:** `admin@lautus.com`
*   **Password:** `123456`

### 4. Run the Application
You can now run the application from the `LautusInformatica` directory:
```bash
dotnet run
```
The API will be available at `http://localhost:5170`.

### 5. Explore the API
Once the application is running, you can explore and test the API endpoints using the Swagger UI at:
`http://localhost:5170/swagger`

## API Endpoints Overview
All endpoints are available under the `/api/` prefix. Most require an `Admin` role, except for public endpoints like login/register.

*   **Authentication:** `POST /api/auth/login`, `POST /api/auth/register`
*   **Users:** `GET`, `POST`, `PUT`, `DELETE` at `/api/users`
*   **Items (Inventory):** `GET`, `POST`, `PUT`, `DELETE`, `PATCH` at `/api/items`
*   **Service Orders:** `GET`, `POST`, `PUT`, `DELETE`, `PATCH` at `/api/service-orders`
*   **Used Items:** `GET`, `POST`, `PUT`, `DELETE` at `/api/service-orders/{serviceOrderId}/used-items`
*   **Logs:** `GET /api/logs`
