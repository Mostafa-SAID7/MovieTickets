# MovieTickets 🎬

A comprehensive movie ticket booking system built with modern web technologies.

## 🚀 Overview

MovieTickets is a full-featured cinema booking platform that allows users to browse movies, select showtimes, choose seats, and book tickets seamlessly. The system provides an intuitive interface for customers while offering robust management capabilities for cinema operators.

## ✨ Features

### For Customers
- **Movie Catalog**: Browse current and upcoming movies with detailed information
- **Showtimes**: View available showtimes across different theaters
- **Seat Selection**: Interactive seat map for selecting preferred seats
- **Booking Management**: Easy ticket booking and confirmation process
- **User Authentication**: Secure login and registration system
- **Payment Integration**: Secure payment processing for ticket purchases
- **Booking History**: Track past and upcoming bookings

### For Administrators
- **Movie Management**: Add, edit, and remove movies from the catalog
- **Theater Management**: Configure theaters, screens, and seating layouts
- **Showtime Scheduling**: Create and manage movie showtimes
- **User Management**: Oversee user accounts and permissions
- **Analytics Dashboard**: Track booking statistics and revenue
- **Booking Overview**: Monitor all system bookings

## 🛠️ Technology Stack

### Backend
- **Framework**: .NET Core / ASP.NET Core
- **Database**: SQL Server with Entity Framework
- **Authentication**: JWT (JSON Web Tokens)
- **Architecture**: RESTful API design

### Frontend
- **Framework**: JavaScript (React/Angular/Vue.js - specify your choice)
- **Styling**: CSS3, Bootstrap/Material-UI
- **State Management**: (Redux/Vuex/etc. - specify if applicable)

### Database
- **Primary**: SQL Server
- **ORM**: Entity Framework Core
- **Migrations**: Code-First approach

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- [.NET 6.0 or later](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB acceptable for development)
- [Node.js](https://nodejs.org/) (for frontend dependencies)
- [Git](https://git-scm.com/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/Mostafa-SAID7/MovieTickets.git
cd MovieTickets
```

### 2. Database Setup
```bash
# Update connection string in appsettings.json
# Run Entity Framework migrations
dotnet ef database update
```

### 3. Backend Setup
```bash
# Navigate to the API project
cd MovieTickets.API

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### 4. Frontend Setup
```bash
# Navigate to the frontend directory
cd MovieTickets.Web

# Install dependencies
npm install

# Start development server
npm start
```

## 📁 Project Structure

```
MovieTickets/
├── MovieTickets.API/          # Backend API project
│   ├── Controllers/           # API controllers
│   ├── Models/               # Data models
│   ├── Services/             # Business logic services
│   ├── Data/                 # Database context and configurations
│   └── Program.cs            # Application entry point
├── MovieTickets.Web/         # Frontend application
│   ├── src/
│   │   ├── components/       # Reusable UI components
│   │   ├── pages/           # Application pages
│   │   ├── services/        # API service calls
│   │   └── utils/           # Utility functions
│   └── public/              # Static assets
├── MovieTickets.Data/        # Data access layer
├── MovieTickets.Core/        # Core business logic
└── MovieTickets.Tests/       # Unit and integration tests
```

## 🔧 Configuration

### Database Configuration
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MovieTicketsDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### JWT Configuration
Configure JWT settings in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "MovieTickets",
    "Audience": "MovieTickets",
    "ExpirationMinutes": 60
  }
}
```

## 🧪 Testing

Run the test suite:
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Database Schema

### Key Entities
- **Users**: Customer and admin user information
- **Movies**: Movie details and metadata
- **Theaters**: Theater and screen information
- **Showtimes**: Movie screening schedules
- **Bookings**: Customer booking records
- **Seats**: Seating arrangements and availability

## 🔐 Authentication & Authorization

The system implements JWT-based authentication with role-based authorization:
- **Customer Role**: Book tickets, view history, manage profile
- **Admin Role**: Full system management capabilities
- **Manager Role**: Theater-specific management permissions

## 🚀 API Endpoints

### Movies
- `GET /api/movies` - Get all movies
- `GET /api/movies/{id}` - Get movie by ID
- `POST /api/movies` - Create new movie (Admin)
- `PUT /api/movies/{id}` - Update movie (Admin)
- `DELETE /api/movies/{id}` - Delete movie (Admin)

### Bookings
- `GET /api/bookings` - Get user bookings
- `POST /api/bookings` - Create new booking
- `GET /api/bookings/{id}` - Get booking details
- `DELETE /api/bookings/{id}` - Cancel booking

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Mostafa Said**
- GitHub: [@Mostafa-SAID7](https://github.com/Mostafa-SAID7)
- LinkedIn: [Your LinkedIn Profile]
- Email: [Your Email]

## 🙏 Acknowledgments

- Thanks to all contributors who have helped with this project
- Inspiration from modern cinema booking platforms
- Built with modern web development best practices

## 📞 Support

If you have any questions or run into issues, please:
1. Check the [Issues](https://github.com/Mostafa-SAID7/MovieTickets/issues) page
2. Create a new issue if your problem isn't already addressed
3. Reach out via email for urgent matters

---

⭐ Star this repository if you find it helpful!
