# LuxeLiving - Full-Stack eCommerce Platform

A sophisticated, production-ready eCommerce application built with modern technologies and clean architecture principles. This comprehensive platform provides a complete online shopping experience with advanced features for both customers and administrators.

## 🚀 Features

### Customer Features
- **User Authentication & Authorization**
  - JWT-based secure authentication
  - Email verification and password reset
  - Role-based access control
  - Refresh token management

- **Product Discovery**
  - Advanced search with filters and sorting
  - Real-time search autocomplete
  - Category-based browsing
  - Recently viewed products tracking

- **Shopping Experience**
  - Interactive shopping cart with real-time updates
  - Wishlist functionality
  - Secure checkout with Stripe integration
  - Order tracking and history

- **User Account Management**
  - Profile management
  - Order history and status tracking
  - Email notifications

### Admin Features
- **Product Management**
  - CRUD operations for products and categories
  - Inventory management
  - Product image handling

- **Order Management**
  - Order status updates
  - Order fulfillment tracking
  - Customer order history

- **User Management**
  - User role management
  - Account administration

## 🛠️ Tech Stack

### Backend (.NET 8)
- **Framework**: ASP.NET Core 8.0
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Host)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT with refresh tokens
- **Payment**: Stripe integration
- **Email**: SMTP email service
- **Patterns**: Repository pattern, Dependency Injection, CQRS principles

### Frontend (Angular 18)
- **Framework**: Angular 18 with standalone components
- **UI**: Bootstrap 5 with custom styling
- **Icons**: FontAwesome & Feather Icons
- **HTTP**: Angular HTTP Client with interceptors
- **State Management**: RxJS observables
- **Notifications**: NGX-Toastr
- **Payment**: Stripe.js integration

## 🏗️ Architecture

### Backend Architecture
```
├── eCommerce.Domain/          # Core business entities and interfaces
├── eCommerce.Application/     # Business logic and DTOs
├── eCommerce.Infrastructure/  # Data access and external services
└── eCommerce.Host/           # API controllers and configuration
```

### Key Design Patterns
- **Clean Architecture**: Separation of concerns with dependency inversion
- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic encapsulation
- **DTO Pattern**: Data transfer optimization
- **Dependency Injection**: Loose coupling and testability

## 🚦 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js (v18+)
- SQL Server (LocalDB or full instance)
- Angular CLI (`npm install -g @angular/cli`)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AngularNet-Ecommerce.git
   cd AngularNet-Ecommerce
   ```

2. **Configure the database**
   ```bash
   cd Backend/eCommerce.Host
   cp appsettings.example.json appsettings.json
   ```
   Update the connection string in `appsettings.json`

3. **Set up configuration**
   Configure the following in `appsettings.json`:
   - Database connection string
   - JWT secret key (minimum 32 characters)
   - Email SMTP settings
   - Stripe API keys

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Start the backend**
   ```bash
   dotnet run
   ```
   API will be available at `https://localhost:7139`

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd Frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   ng serve
   ```
   Application will be available at `http://localhost:4200`

## 📁 Project Structure

### Backend Structure
```
Backend/
├── eCommerce.Domain/
│   ├── Entities/              # Domain entities (User, Product, Order, etc.)
│   └── Interface/             # Domain interfaces
├── eCommerce.Application/
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Services/              # Business logic services
│   ├── Mapping/               # AutoMapper profiles
│   └── Configuration/         # Application settings
├── eCommerce.Infrastructure/
│   ├── Data/                  # DbContext and configurations
│   ├── Repositories/          # Data access implementations
│   ├── Services/              # External service implementations
│   └── Migrations/            # Database migrations
└── eCommerce.Host/
    ├── Controllers/           # API endpoints
    └── Program.cs             # Application entry point
```

### Frontend Structure
```
Frontend/src/app/
├── Core/
│   ├── Guards/                # Route guards (auth, admin)
│   ├── Interceptors/          # HTTP interceptors
│   ├── Models/                # TypeScript interfaces
│   └── Services/              # Angular services
└── Shared/
    └── Components/            # Reusable UI components
        ├── auth/              # Authentication components
        ├── product/           # Product-related components
        ├── cart/              # Shopping cart components
        ├── checkout/          # Checkout and payment
        └── admin/             # Admin panel components
```

## 🔧 Configuration

### Environment Variables
Create `appsettings.json` from `appsettings.example.json` and configure:

- **Database**: SQL Server connection string
- **JWT**: Secret key and token settings
- **Email**: SMTP server configuration
- **Stripe**: Payment processing keys
- **CORS**: Frontend URL configuration

### Database Schema
The application uses a comprehensive eCommerce schema including:
- Users and authentication
- Products and categories
- Shopping carts and cart items
- Orders and order items
- Wishlists and recently viewed items

## 🔐 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: BCrypt password encryption
- **CORS Configuration**: Controlled cross-origin requests
- **Input Validation**: Comprehensive data validation
- **SQL Injection Protection**: Entity Framework parameterized queries
- **XSS Protection**: Angular's built-in sanitization

## 🧪 Testing

### Backend Testing
```bash
cd Backend
dotnet test
```

### Frontend Testing
```bash
cd Frontend
ng test
```

## 📦 Deployment

### Backend Deployment
1. Publish the application:
   ```bash
   dotnet publish -c Release
   ```

2. Configure production settings in `appsettings.Production.json`

3. Deploy to your preferred hosting platform (Azure, AWS, etc.)

### Frontend Deployment
1. Build for production:
   ```bash
   ng build --prod
   ```

2. Deploy the `dist/` folder to your web server

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with modern web development best practices
- Implements clean architecture principles
- Uses industry-standard security practices
- Designed for scalability and maintainability

## 📞 Support

For support, email support@luxeliving.com or create an issue in this repository.

---

**Note**: This is a comprehensive eCommerce platform suitable for real-world applications with proper configuration and deployment. The architecture follows industry best practices and is designed for scalability and maintainability.
