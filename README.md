# BookItEasy - Backend

### Project Description

BookItEasy is an online platform for booking services that allows users to browse and book services for specific dates and times. 
Additionally, users can create business accounts and add their own services, which can be booked by others.

The backend handles user authentication with JWT tokens, ensures secure communication with PostgreSQL as the database, and provides a RESTful API for interaction with the frontend.

Technologies used include .NET 8.0 for the API, JWT for secure authentication, and PostgreSQL as the database.

### System Requirements

- .NET SDK (version 8.0 or higher)
- PostgreSQL (version 12.0 or higher)
- Database management tool like pgAdmin or psql

### Setup Instructions

1. **Clone the repository:**
```bash
   git clone https://github.com/Daniruu/BookingService.git
   cd BookingService
```

2. **Set up the database:**
  - Install PostgreSQL and create a new database.
  - Configure the `appsettings.json` file to include your database connection information:
```json
{
  "ConntectionStrings": {
    "DefaultConnection": "Host=localhost;Database=BookingServiceDb;Username=myuser;Password=mypassword"
	},
  "Jwt": {
    "Key": "YourSuperSecretKeyHere",
    "Issuer": "https://your-domain.com",
    "Audience": "https://your-domain.com",
    "ExpireMinutes": "5"
  },
}
```

3. **Apply migrations:**
- Install Entity Framework tools:
```bash
dotnet tool install --global dotnet-ef
```
- Use Entity Framework tools to update the database schema:
```bash
  dotnet ef database update
```

4. **Run the application:** You can run the application using the following command:
```bash
  dotnet run
```

5. **Access the API:** The application will be available at:
- http://localhost:5022
- https://localhost:7143

### API Documentation
The application uses Swagger for API documentation. Once the application is running, you can access the API documentation at:
- Swagger UI: https://localhost:7143/swagger or http://localhost:5022/swagger
### Authentication
The application uses JSON Web Tokens (JWT) for secure authentication. After logging in, users receive a JWT, which must be included in the `Authorization` header of subsequent requests to access protected endpoints. 

### Technologies
- .NET 8.0
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- Google Cloud Storage
