# SmartOps Backend

ASP.NET Core Web API for the SmartOps IT operations and incident management platform. The backend exposes HTTP endpoints for the Angular frontend and uses PostgreSQL for persistent data.

## Features and Modules

- Authentication and JWT bearer validation
- User and role-related services
- Team and team membership services
- Incident management and assignment
- Incident comments and history
- Dashboard metrics
- SLA policies
- Notifications
- Audit logs
- Health checks
- SignalR notifications hub
- Request logging, exception handling, and rate limiting

Actual endpoint behavior and access depend on the controllers, DTOs, configured policies, and database data in this repository.

## Technology

- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer authentication
- FluentValidation
- Serilog
- SignalR

## Project Structure

```text
SmartOps/
├── SmartOps.API/             # Controllers, middleware, hubs, startup
├── SmartOps.Application/     # DTOs, services, interfaces, validators
├── SmartOps.Domain/          # Domain entities, enums, common definitions
└── SmartOps.Infrastructure/  # EF Core, PostgreSQL, repositories, identity
```

## Prerequisites

- .NET SDK compatible with the project's target framework
- PostgreSQL
- A database client such as pgAdmin (optional)

Check the SDK:

```bash
dotnet --version
```

## Setup

### 1. Configure the database

Create a PostgreSQL database for SmartOps. In `SmartOps.API/appsettings.json` or an appropriate local configuration provider, set the existing connection string key:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SmartOpsDb;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD"
  }
}
```

Replace the example values with your local database settings. The actual database name, username, and password must match your PostgreSQL setup.

### 2. Configure JWT

The API expects these configuration keys:

```json
{
  "Jwt": {
    "SecretKey": "REPLACE_WITH_A_LONG_RANDOM_SECRET",
    "Issuer": "YOUR_CONFIGURED_ISSUER",
    "Audience": "YOUR_CONFIGURED_AUDIENCE"
  }
}
```

The values must match the settings used by the project's token service. Do not commit real secrets. Use .NET User Secrets or environment variables for local development where possible.

### 3. Restore and build

Run from the solution directory, or specify the solution/project path:

```powershell
dotnet restore
dotnet build
```

### 4. Run the API

```powershell
cd SmartOps.API
dotnet run --launch-profile http
```

Use the URL and port printed by the terminal. The local development setup has used `http://localhost:5181`, but the selected launch profile may differ.

### 5. Check health

If the API is running on port 5181:

```text
GET http://localhost:5181/health
```

In Development, OpenAPI may be available at:

```text
http://localhost:5181/openapi/v1.json
```

Availability depends on the environment and launch configuration.

## Configuration Reference

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection |
| `Jwt:SecretKey` | JWT signing secret |
| `Jwt:Issuer` | Expected token issuer |
| `Jwt:Audience` | Expected token audience |

The application also configures the Angular CORS policy for local development origins. If the frontend runs on a different origin, update the allowed origins in a deliberate deployment-specific configuration.

## API Routes

Routes below are examples of endpoints referenced by the frontend or shown in the authentication controller. Confirm exact routes, DTOs, and permissions in the controller code.

| Area | Method and route | Purpose |
|---|---|---|
| Auth | `POST /api/Auth/login` | Login |
| Auth | `POST /api/Auth/register` | Register |
| Auth | `GET /api/Auth/me` | Current authenticated user |
| Auth | `POST /api/Auth/refresh` | Refresh access token |
| Auth | `POST /api/Auth/logout` | Revoke refresh token |
| Dashboard | `GET /api/Dashboard/summary` | Dashboard summary |
| Dashboard | `GET /api/Dashboard/monthly-trend` | Monthly incident trend |
| Dashboard | `GET /api/Dashboard/team-workload` | Team workload |
| Users | `/api/User` | User endpoints |
| Teams | `/api/Team` | Team endpoints |
| Incidents | `/api/Incident` | Incident endpoints |
| Notifications | `/api/Notification` | Notification endpoints |
| Audit | `/api/AuditLog` | Audit-log endpoints |
| SLA | `/api/SLAPolicy` | SLA policy endpoints |
| Health | `GET /health` | Health check |
| SignalR | `/hubs/notifications` | Notifications hub |

## Authentication and Authorization

- JWT Bearer authentication validates protected requests.
- Authorization policies in the application include roles such as Administrator, Team Lead, Support Agent, Engineer, and Manager.
- Frontend navigation restrictions are not sufficient for security. The API must enforce authorization at the endpoint and data-query levels.
- If users should see only assigned incidents or team-scoped data, enforce those rules in backend services/repositories.

## Database Seeding

The API startup invokes `DbSeeder.SeedAsync`. Review the seeder and its configuration to understand which records and development accounts are created. Do not assume any example login credentials are valid unless they are explicitly configured in your environment.

## Troubleshooting

### 404 Not Found
- Verify the request path and HTTP method.
- Check controller route attributes.
- Ensure `builder.Services.AddControllers()` is registered and `app.MapControllers()` is called.
- Verify the request reaches the expected backend port.

### 401 Unauthorized
- Check the bearer token and expiry.
- Verify JWT secret, issuer, audience, and token claims.

### 403 Forbidden
- Check the authenticated user's roles and the endpoint's authorization policy.

### PostgreSQL connection error
- Ensure PostgreSQL is running.
- Check the connection string and database permissions.
- Verify the configured database exists.

### Build or startup failure
- Run `dotnet restore` and `dotnet build`.
- Read the first relevant exception in the terminal and application logs.
- Check required configuration values before restarting.

## Security

- Never commit database credentials, JWT secrets, refresh tokens, or production configuration secrets.
- Use HTTPS in deployed environments.
- Use a strong random JWT signing key.
- Review seed accounts before production deployment.
- Back up PostgreSQL and test database restoration.
- Apply authorization and data isolation in backend code, not only in the frontend.

