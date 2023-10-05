# TODO Notes application

Showcase WebApi built using ASP.NET Core.

## Features

- [x] NET 8
- [x] REPR endpoint layout using Minimal APIs
- [x] Vertical Slice architecture
- [x] Structured logging using Serilog
- [x] Metricts endpoint using prometheus-net
- [ ] Metrics endpoint using OTEL
- [x] Custom ASP.NET Core AuthenticationHandler implementation using DynamoDB
- [x] Policy based Authorization
- [x] Request validation using FluentValidation
- [x] Crud using DynamoDB
- [x] CRUD using EF Core (PostgreSQL)
- [x] CRUD using Dapper (PostgreSQL)
- [x] CRUD using Redis
- [x] Keyset pagination
- [x] Data seeding using hosted service
- [x] Idempotent POST requests
  - [x] InMemory
  - [x] DynamoDb
  - [x] Redis
- [x] Feature Flags middleware
- [x] Upstream service client using Refit, HTTP Client Factory
- [x] Upstream request resilience using Polly
- [ ] Polly V8
- [x] Separate OpenAPI documents per versioned API surface
