# RiteSwipe - Real-time Task Marketplace

RiteSwipe is a Tinder-like interface for connecting task posters with workers. The platform facilitates real-time task matching, secure payments through escrow, and a comprehensive review system.

## Tech Stack

### Backend
- ASP.NET 8 Web API
- SQL Server
- SignalR for real-time communication
- Entity Framework Core
- JWT Authentication
- FluentValidation
- Clean Architecture

### Frontend
- React with TypeScript
- Redux for state management
- @microsoft/signalr for real-time updates
- Material-UI components
- React Router
- Axios for HTTP requests

## Features

1. **Real-time Task Matching**
   - Tinder-like swipe interface
   - Instant notifications for matches
   - Real-time task updates

2. **Secure Payments**
   - Escrow system
   - Real-time payment status updates
   - Dispute resolution system

3. **User Management**
   - JWT authentication
   - Profile management
   - Skill tracking
   - Rating system

4. **Task Management**
   - Create and manage tasks
   - Apply for tasks
   - Real-time application updates
   - Task status tracking

5. **Reviews and Ratings**
   - Two-way review system
   - Rating aggregation
   - Real-time review notifications

## Project Structure

The solution follows Clean Architecture principles:

- **RiteSwipe.Domain**: Core entities and business rules
- **RiteSwipe.Application**: Application services and interfaces
- **RiteSwipe.Infrastructure**: External concerns (database, SignalR, etc.)
- **RiteSwipe.Api**: API controllers and SignalR hubs
- **RiteSwipe.Web**: React frontend application
- **RiteSwipe.Tests**: Unit and integration tests

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Node.js and npm
- Docker (optional)

### Development Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/riteswipe.git
   cd riteswipe
   ```

2. Set up environment variables:
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

3. Backend Setup:
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run --project RiteSwipe.Api
   ```

4. Frontend Setup:
   ```bash
   cd RiteSwipe.Web
   npm install
   npm start
   ```

### Docker Setup

1. Build and run using Docker Compose:
   ```bash
   docker-compose up -d
   ```

2. Access the application:
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Frontend: http://localhost:3000

## Testing

Run backend tests:
```bash
dotnet test
```

Run frontend tests:
```bash
cd RiteSwipe.Web
npm test
```

## Architecture

The application follows Clean Architecture principles with:

- **Domain-Driven Design**: Rich domain model with business rules
- **CQRS Pattern**: Separate command and query responsibilities
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **SignalR Integration**: Real-time communication
- **JWT Authentication**: Secure API access
- **FluentValidation**: Request validation
- **Swagger**: API documentation

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
