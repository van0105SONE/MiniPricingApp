# MiniPricing Web API

## Overview

**MiniPricing Web API** is a modular and scalable **ASP.NET Web API** designed for pricing calculations.  
It follows **SOLID principles**, **modular feature-based architecture**, and allows maintainable business logic.

---

## Features

- Modular architecture with feature-based separation
- ASP.NET Web API (.NET 9)
- Dependency Injection
- Global exception handling & logging (Serilog)
- JSON serialization with System.Text.Json
- Environment-based configuration
- Unit testing support with xUnit

---

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/) (optional, recommended)

---

### 1️⃣ Run with Docker

1. Build the Docker image:

```bash
docker build -t minipricingapp .
```

2. Run the container:

```bash
docker run -d -p 5000:8080 --name mini-pricing-api minipricingapp
```

3. Access the API at:

```
http://localhost:5000/swagger/index.html
```

---

### 2️⃣ Run with Docker Compose (recommended)


2. Build and run the container:

```bash
docker-compose up --build
```

3. Access the API DOCUMENT at:

```
http://localhost:5000/swagger/index.html
```

---

### 3️⃣ Run Locally (without Docker)

1. Restore dependencies:

```bash
dotnet restore
```

2. Run the API:

```bash
dotnet run --project MiniPricingApp
```

3. Access the API at:

```
http://localhost:5000
```

---

## API Documentation

Swagger and ReDoc documentation are available for exploring endpoints:

- Swagger UI:  
```
http://localhost:5000/swagger/index.html
```

- ReDoc UI:  
```
http://localhost:5000/api-docs/
```

> You can see request/response schema, test endpoints, and example requests directly in the browser.

---

## API Endpoints Overview

| HTTP Method | Endpoint                 | Description |
|------------|-------------------------|-------------|
| POST       | `/qoutes/price`         | Calculate price for a quote |
| POST       | `/qoutes/bulk`          | Upload CSV file for bulk quote creation |
| GET        | `/jobs/{id}`            | Get job status by Job Id |
| POST       | `/rules`                | Create a new pricing rule |
| PUT        | `/rules/{id}`           | Update an existing pricing rule |
| GET        | `/rules/all`            | Get all pricing rules |

---

## Project Structure

```
├── Middleware/
├── Modules/
│   ├── Health/
│   ├── Rules/
│   │   ├── Application/
│   │   │   ├── Dtos/
│   │   │   ├── Mappers/
│   │   │   ├── Services/
│   │   ├── Domains/
│   │   │   ├── Entities/
│   │   │   ├── Enums/
│   │   │   ├── Interface/
│   │   │   ├── Services/
│   │   │   ├── Validator/
│   │   ├── Infrastructure/
│   ├── Quotes/
│   │   ├── Services/
│   │   ├── Domains/
│   │   ├── Infrastructure/
├── Shared/
│   ├── Base/
│   ├── Exceptions/
│   └── Utilities/
├── Tests/
```

---

## Running Unit Tests

Run all tests using:

```bash
dotnet test
```

---

## Notes

- **CSV uploads**: Place uploaded files in the `UploadedCsv` folder (automatically created when using Docker Compose).  
- **Background jobs**: Bulk CSV processing runs via a **background task every 1 minute**. After uploading a CSV, wait at least 1 minute before checking job status.

---

## Technology Stack

- .NET 9
- ASP.NET Web API
- C#
- Serilog Logging
- xUnit for unit testing

---

## Author

- Vanlakhan INSYXIENGMAI
- chanthakhaninsyxiengmai@gmail.com