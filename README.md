# 🚀 ERP Lite

A lightweight, modular Enterprise Resource Planning system built with ASP.NET Core and Entity Framework Core, designed for medium-sized organizations. This project showcases Clean Architecture, CQRS, and modern .NET development practices.

> 📌 **Note:** This is currently a backend-only implementation with mock data for demonstration purposes.



## ✨ Features

- **Clean Architecture** - Organized into Core, Infrastructure, and API layers
- **Repository Pattern** - Abstraction over data access for improved testability
- **Entity Framework Core** - Modern ORM with SQL Server
- **JWT Authentication** - Secure API with token-based authentication
- **Role-Based Authorization** - Granular permission control

## 📋 Modules

- **📊 Human Resources** - Employee management, attendance tracking, leave management
- **📦 Inventory** - Product catalog, stock levels, purchase orders
- **💰 Expense Tracking** - Budget management, expense requests, approval workflows
- **📈 Reporting** - KPI visualization, customizable reports

## 🔧 Tech Stack

- **Backend**: ASP.NET Core 8.0, Entity Framework Core
- **Architecture**: Clean Architecture, Repository Pattern
- **Database**: SQL Server Express
- **Documentation**: Swagger/OpenAPI
- **Authentication**: JWT with refresh tokens
- **Validation**: FluentValidation

## 📥 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or higher
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)

## 🚀 Getting Started

### Clone the Repository

```bash
git clone https://github.com/YOUR-USERNAME/erp-lite.git
cd erp-lite
```

### Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Create a new database named `ERPLite`
3. Update the connection string in `appsettings.json` if necessary

### Configure and Run

1. Navigate to the API project directory:
   ```bash
   cd ERPLite.API
   ```

2. Update the database with migrations:
   ```bash
   dotnet ef database update -s . -p ../ERPLite.Infrastructure
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Access Swagger UI:
   ```
   https://localhost:7001/swagger/index.html
   ```
   (Port may vary; check the console output)

## 📂 Project Structure

```
ERPLite/
├── ERPLite.API/              # API Controllers, Middleware, Configuration
├── ERPLite.Core/             # Domain Entities, Interfaces, Business Logic
│   ├── Domain/               # Entities and Domain Logic
│   ├── Interfaces/           # Repository and Service Interfaces
│   └── Exceptions/           # Custom Exception Types
├── ERPLite.Infrastructure/   # Data Access, External Services Implementation
│   ├── Data/                 # DbContext, Migrations, Entity Configurations
│   ├── Repositories/         # Repository Implementations
│   ├── Services/             # Service Implementations
│   └── Auth/                 # Authentication Services
└── Tests/                    # Unit and Integration Tests
```

## 🧪 Mock Data

The system is pre-configured with mock data for demonstration purposes. Upon running the migrations, the database will be seeded with:

- Default admin user (Username: `admin`, Password: `Admin123!`)
- Sample departments, positions, and employees
- Test products and inventory items
- Demo purchase orders and expenses

## 🔐 Authentication

To get started with the API, obtain a JWT token by making a POST request to:

```
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin123!"
}
```

The response will include a JWT token and refresh token to use in subsequent requests via the Authorization header:

```
Authorization: Bearer your_jwt_token
```

## 🗺️ API Endpoints

Explore all available endpoints using the Swagger UI at `/swagger/index.html`

Main API groups:
- Auth: `/api/auth`
- Employees: `/api/employees`
- Departments: `/api/departments`
- Products: `/api/products`
- Inventory: `/api/inventory`
- Purchase Orders: `/api/purchase-orders`
- Expenses: `/api/expenses`

## 🔮 Roadmap

- [ ] React frontend integration
- [ ] Dockerization for easier deployment
- [ ] Extended reporting capabilities
- [ ] Internationalization (English/Arabic)
- [ ] Email notifications
- [ ] Mobile app integration

## 🤝 Contributing

Contributions are welcome! This project is intended to be a showcase but pull requests and suggestions are appreciated.

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add some amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Contact

Your Name - ferasAlkhodari51@gmail.com

Project Link: "@https://github.com/FerasAlhkodari/ERPLite.git "

---

⭐ Star this repository if you find it useful!
