# 🛠️ TooliRent API

## TooliRent is a REST-based API for managing tool rentals in a makerspace environment.  
It is built with **ASP.NET Core**, **Entity Framework Core**, and follows modern software architecture and design patterns.  
Even though this is a school project, it has been developed with the goal of resembling a real-world application as closely as possible, given my current knowledge level.
The project also attempts to follow a **feature-first approach** (sometimes called vertical slice architecture), where functionality is grouped by features (e.g. Bookings, Loans, Tools) instead of only being separated by technical layers. This helps keep related code together and makes the project easier to maintain and scale.

---

## 📖 Table of Contents
- [Features](#-features)
- [Architecture](#-architecture)
- [Technical Requirements](#-technical-requirements)
- [API Endpoints](#-api-endpoints)
- [Roadmap](#-roadmap)
- [Getting Started](#-getting-started)
- [Author](#-author)
- [License](#-licence)
---

## 🚀 Features

### Authentication & Authorization
- ✅ Register as a **Member**
- ✅ Login and receive a JWT token
- ✅ Refresh token flow
- ⏳ Logout *(Coming Soon)*
- Roles:
  - **Member**
  - **Admin**

### Tools
- ✅ List all available tools
- ✅ Filter tools by category, status, and availability
- ⏳ View detailed information about specific tools *(Coming Soon)*

### Bookings
- ✅ Create a booking (one or multiple tools for a period)
- ✅ View personal bookings and their status
- ✅ Cancel bookings *(rule: cannot cancel if already started or expired)*

### Loans (Pickup / Return)
- ✅ Mark tool as picked up
- ✅ Mark tool as returned
- ✅ Handle overdue returns

### Admin Functions
- ⏳ CRUD operations for tools *(Coming Soon)*
- ⏳ CRUD operations for tool categories *(Coming Soon)*
- ⏳ Manage users (activate/deactivate) *(Coming Soon)*
- ⏳ Rental and usage statistics *(Coming Soon)*

---

## 🏗️ Architecture

The project is based on **N-tier architecture** with clear separation of concerns:

- **Presentation Layer** → API Controllers  
- **Application Layer** → Services  
- **Domain Layer** → Entities & Interfaces  
- **Infrastructure Layer** → Repositories & DbContext  

**Design patterns used:**
- **Service Pattern** for business logic
- **Repository Pattern** for data access

---

## 📦 Technical Requirements

- **DTO & Mapping**
  - AutoMapper for Entity <-> DTO mapping
  - Separation between domain entities and external contracts

- **Validation**
  - FluentValidation for incoming DTOs
  - Domain rules validated separately

- **Authentication**
  - JWT Bearer authentication
  - Role-based authorization (**Member** & **Admin**)

- **Database**
  - Relational database (SQL Server)
  - Code-first with EF Core migrations
  - Seed data for development and testing

---

## 🌐 API Endpoints

### 🔐 Authentication
| Method | Endpoint        | Description                          | Status       |
|--------|-----------------|--------------------------------------|--------------|
| POST   | `/auth/register`| Register a new member                | ✅ Implemented |
| POST   | `/auth/login`   | Login with email & password, returns JWT token | ✅ Implemented |
| POST   | `/auth/refresh` | Refresh JWT token                    | ✅ Implemented |
| POST   | `/auth/logout`  | Logout user                          | ⏳ Coming Soon |

### 🛠️ Tools
| Method | Endpoint        | Description                          | Status       |
|--------|-----------------|--------------------------------------|--------------|
| GET    | `/tools`        | List tools (with optional filters)   | ✅ Implemented |
| GET    | `/tools/{id}`   | Get details of a specific tool       | ✅ Implemented |

### 📅 Bookings
| Method | Endpoint         | Description                          | Status       |
|--------|------------------|--------------------------------------|--------------|
| POST   | `/bookings`      | Create a booking                     | ✅ Implemented |
| GET    | `/bookings/me`   | List bookings for the logged-in user | ✅ Implemented |
| DELETE | `/bookings/{id}` | Cancel a booking                     | ✅ Implemented |

### 📦 Loans (Pickup / Return)
| Method | Endpoint         | Description                          | Status       |
|--------|------------------|--------------------------------------|--------------|
| POST   | `/loans/pickup`  | Mark tool as picked up               | ✅ Implemented |
| POST   | `/loans/return`  | Mark tool as returned                | ✅ Implemented |

### 👑 Admin
| Method | Endpoint                   | Description                          | Status       |
|--------|----------------------------|--------------------------------------|--------------|
| POST   | `/admin/tools`             | CRUD operations for tools            | ⏳ Coming Soon |
| POST   | `/admin/categories`        | CRUD operations for tool categories  | ⏳ Coming Soon |
| PATCH  | `/admin/users/{id}/activate`| Activate/deactivate a user           | ⏳ Coming Soon |
| GET    | `/admin/statistics`        | Rental and usage statistics          | ⏳ Coming Soon | 

---

## 📊 Roadmap

Planned improvements and extra features:  
- ⏳ **Docker Compose** (API + DB packaged for easy startup)  
- ⏳ **Global Exception Middleware** (RFC 7807 standardized error responses)  
- ⏳ **Rate Limiting** (protect sensitive endpoints like `/auth/login`)  
- ⏳ **Integration Tests** (WebApplicationFactory + InMemory DB)  
- ⏳ **Response Caching** (optimize frequent GET requests)  

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server
- IDE of choice

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/JonssonF/TooliRent.git
   cd TooliRent

---

## 👨‍💻 Author
- 🌐[GitHub – Fredrik Jonsson](https://github.com/JonssonF)
📍 System Development Student at *Campus Varberg* (2023–2026)

---

## 📄 License

MIT License – free to use, modify and distribute.
