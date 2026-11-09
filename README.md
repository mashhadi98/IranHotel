# VisitorApp - Clean Architecture API with Standardized Response Envelopes

## Overview

VisitorApp is a comprehensive .NET 10 Web API project built with **Clean Architecture** principles, featuring a standardized response envelope system for consistent API responses. The project implements CQRS patterns, Repository patterns, and FastEndpoints with centralized error handling and validation.

## 🎯 Key Features

### ✨ **Standardized API Responses**
- **Envelope Response System**: All API responses wrapped in consistent format
- **Centralized Error Handling**: Domain exceptions automatically mapped to appropriate HTTP responses
- **Validation Error Structure**: Detailed validation errors with property-level feedback
- **Trace ID Correlation**: Automatic request correlation for debugging and monitoring

### 🏗️ **Architecture & Patterns**
- **Clean Architecture**: Separation of concerns with clear dependency boundaries
- **CQRS**: Command Query Responsibility Segregation with MediatR
- **Repository Pattern**: Generic repository with read/write separation
- **Domain Driven Design**: Rich domain models with business rule encapsulation
- **SOLID Principles**: Comprehensive implementation throughout the codebase

### 🔧 **Technical Stack**
- **.NET 10** with latest C# features
- **FastEndpoints** for high-performance API endpoints
- **Entity Framework Core** with Code First approach
- **AutoMapper** for object mapping
- **Serilog** for structured logging [[memory:8258441]]
- **FluentValidation** for request validation

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK or higher
- SQL Server or PostgreSQL
- Git
- Docker (optional)

### Installation
```bash
# Clone the repository
git clone <repository-url>
cd VisitorApp

# Restore packages
dotnet restore

# Update database
dotnet ef database update --project VisitorApp.Persistence --startup-project VisitorApp.API

# Run the application
cd VisitorApp.API
dotnet run
```

### Access Points
- **API Base URL**: `https://localhost:5001/api`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`

## 📋 API Response Format

### Standard Response Envelope

All API responses follow a consistent envelope structure:

#### Success Response
```json
{
  "success": true,
  "data": {
    "id": "123",
    "name": "Product Name",
    "price": 99.99
  },
  "message": "Success",
  "code": "SUCCESS",
  "traceId": "0HN7JKQTF5D8P:00000001",
  "errors": null,
  "timestamp": "2024-01-15T10:30:45.123Z"
}
```

#### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Product not found",
  "code": "NOT_FOUND",
  "traceId": "0HN7JKQTF5D8P:00000002",
  "errors": null,
  "timestamp": "2024-01-15T10:30:45.123Z"
}
```

#### Validation Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "code": "VALIDATION_ERROR",
  "traceId": "0HN7JKQTF5D8P:00000003",
  "errors": [
    {
      "property": "name",
      "message": "Name is required",
      "code": "REQUIRED",
      "attemptedValue": null
    }
  ],
  "timestamp": "2024-01-15T10:30:45.123Z"
}
```

### Standard Error Codes

| Code | Description | HTTP Status |
|------|-------------|-------------|
| `SUCCESS` | Operation completed successfully | 200 |
| `VALIDATION_ERROR` | Request validation failed | 400 |
| `NOT_FOUND` | Resource not found | 404 |
| `FORBIDDEN` | Access denied | 403 |
| `BUSINESS_RULE` | Business rule violation | 400 |
| `UNHANDLED` | Unexpected server error | 500 |

## 🏗️ Project Structure

```
VisitorApp/
├── 📂 VisitorApp.API/              # 🌐 Presentation Layer
│   ├── Common/                        # Shared components
│   │   ├── Endpoints/                 # Base endpoint classes
│   │   ├── Models/                    # API models and envelopes
│   │   ├── Middleware/                # Exception handling
│   │   ├── Services/                  # Response envelope factory
│   │   └── Configuration/             # Service extensions
│   └── Features/                      # Feature-based endpoints
│
├── 📂 VisitorApp.Application/       # 💼 Business Logic Layer
│   ├── Common/                        # CQRS infrastructure
│   └── Features/                      # Command/Query handlers
│
├── 📂 VisitorApp.Domain/            # 🎯 Core Business Layer
│   ├── Common/                        # Base entities and contracts
│   │   └── Exceptions/                # Domain-specific exceptions
│   ├── Features/                      # Domain entities
│   └── Shared/                        # Result patterns
│
├── 📂 VisitorApp.Infrastructure/    # 🔧 External Services Layer
│   └── Common/Repository/             # Repository implementations
│
└── 📂 VisitorApp.Persistence/       # 💾 Data Access Layer
    ├── Common/Context/                # Database context
    └── Features/                      # Entity configurations
```

## 🔧 Configuration

### appsettings.json
```json
{
  "EnvelopeResponse": {
    "IsEnabled": true,
    "EnableBackwardCompatibility": false,
    "EnableDetailedErrors": false,
    "DefaultSuccessMessage": "Success",
    "DefaultSuccessCode": "SUCCESS"
  }
}
```

### Service Registration
```csharp
// Program.cs
builder.Services.AddResponseEnvelopeServices();
builder.Services.AddEnvelopeServicesWithMigration(builder.Configuration);

// Pipeline configuration
app.UseEnvelopeResponsePipeline();
```

## 🧪 API Routes

```text
POST  /api/orders
GET   /api/orders/{id}
GET   /api/customers/{customerId}/orders?page=1&pageSize=20
PATCH /api/orders/{id}/status
```

Identity endpoints (login, registration, refresh token, current user, users, and roles) are also available under `/api`.

## 📚 Documentation

Comprehensive documentation is available in the `docs/` folder:

- [**Architecture Overview**](./docs/01-Architecture-Overview.md) - Complete system architecture
- [**Code Standards**](./docs/02-Code-Standards.md) - Coding guidelines and best practices  
- [**Project Structure**](./docs/03-Project-Structure.md) - Detailed file organization
- [**Development Guidelines**](./docs/04-Development-Guidelines.md) - Development workflows
- [**API Response Contract**](./docs/API-Response-Contract.md) - Complete API response documentation
- [**Quick Reference**](./docs/Quick-Reference.md) - Common tasks and patterns

## 🎯 Key Benefits

### For Developers
- **Consistent Experience**: All endpoints follow the same response pattern
- **Rich Error Information**: Detailed validation and business rule violations
- **Debugging Support**: Automatic trace ID correlation
- **Type Safety**: Strong typing throughout the response pipeline
- **Easy Testing**: Predictable response structure for all scenarios

### For Frontend/Client Developers
- **Predictable Responses**: Same envelope structure for success and error
- **Rich Error Handling**: Detailed error information for user feedback
- **Request Correlation**: Trace IDs for debugging and support
- **TypeScript Support**: Easy to generate type definitions

### For Operations
- **Request Tracing**: Full request correlation across logs
- **Centralized Error Handling**: All errors processed consistently
- **Monitoring Ready**: Structured logging with correlation
- **Health Checks**: Built-in health monitoring endpoints

## 🔄 Migration from Legacy APIs

The system includes comprehensive migration support:

```csharp
// Feature toggle
"EnvelopeResponse": {
  "IsEnabled": true,
  "EnableBackwardCompatibility": true,
}

// Legacy endpoint marker
[LegacyResponseFormat]
public class LegacyEndpoint : EndpointBase<Request, Response>
{
    // Automatically handled during migration
}

// Convert legacy responses
var envelope = legacyResponse.ToEnvelope(envelopeFactory);
```

## 🚀 Performance Considerations

- **Minimal Overhead**: ~100-200 bytes per response
- **Optimized Serialization**: Configured JSON serialization
- **Async Throughout**: Full async/await implementation
- **EF Core Optimizations**: AsNoTracking, projection, pagination
- **Memory Efficient**: Proper resource disposal and lifecycle management

## 🔒 Security Features

- **Input Validation**: Comprehensive request validation
- **Error Sanitization**: No sensitive information in error messages
- **Trace ID Security**: Safe correlation identifiers
- **Authorization**: Role-based access control
- **HTTPS Enforcement**: Production security headers

## 🐛 Debugging & Troubleshooting

### Using Trace IDs
Every response includes a `traceId` for correlation:

```bash
# Find logs for specific request
grep "0HN7JKQTF5D8P:00000001" logs/app.log

# Correlate across services
GET /api/orders/{id}
# Response: { "traceId": "ABC123", ... }
# Logs: [ABC123] Processing request...
```

### Common Issues
1. **Missing Envelope**: Ensure endpoint inherits from `EnvelopeEndpointBase`
2. **Raw Responses Wrapped**: Implement `IRawResponse` interface
3. **Validation Not Working**: Check `FluentValidation` configuration
4. **Missing Trace Correlation**: Verify logging middleware registration

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the coding standards in [docs/02-Code-Standards.md](./docs/02-Code-Standards.md)
4. Write comprehensive tests
5. Update documentation
6. Create a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Support

For questions, issues, or contributions:
- **Issues**: Create a GitHub issue
- **Documentation**: Check the `docs/` folder
- **Examples**: See `VisitorApp.API/Features/Examples/`

**Built with ❤️ using Clean Architecture and modern .NET practices** 