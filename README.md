# EazyAssessment

A simple user management application built with React and ASP.NET Core Web API. The application supports user registration, JWT authentication, profile management, and profile picture uploads using Azure Blob Storage.

## Demo

Application: https://eazyassessment-api-cmhafzhfewdhewbw.southeastasia-01.azurewebsites.net/

## Tech Stack

### Frontend
- React (Vite)
- TypeScript
- React Router
- Fetch API

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core
- JWT Authentication
- BCrypt.Net

### Database & Storage
- Azure SQL Database
- Azure Blob Storage

### Hosting
- Azure App Service

---

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- Node.js 20+
- Azure SQL Database
- Azure Storage Account (Blob Storage)

### Backend

1. Clone the repository.

2. Create an `appsettings.Development.json` file in the `EazyAssessment.Server` project.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<YOUR_CONNECTION_STRING>"
  },
  "Jwt": {
    "Key": "<YOUR_JWT_SECRET>",
    "Issuer": "EazyAssessmentApi",
    "Audience": "EazyAssessmentClient"
  },
  "BlobStorage": {
    "ConnectionString": "<YOUR_BLOB_CONNECTION_STRING>",
    "ContainerName": "images"
  }
}
```

3. Apply the database migrations.

```bash
dotnet ef database update
```

4. Run the backend.

```bash
dotnet run
```

---

### Frontend

1. Navigate to the client project.

```bash
npm install
```

2. Start the development server.

```bash
npm run dev
```
