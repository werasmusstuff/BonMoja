# 📘 SOLUTION.md

## 🏗️ Architecture Overview

This solution implements a minimal, scalable backend API for recording and retrieving user event logs. It leverages a serverless architecture, using AWS SAM to simulate local Lambda-based hosting. The API is written in C# (.NET 8) and separates concerns between structured and unstructured data:

- **Structured Metadata**: Stored in SQL Server (e.g., userId, eventType, timestamp)
- **Flexible Payload**: Stored as JSON in DynamoDB, keyed by `UserId` and indexed by `EventId`
- **Service Composition**: Follows a clean architecture with dependency injection, separation of concerns, and testable interfaces.

### Key Components
- **.NET Web API** running under AWS Lambda (Amazon.Lambda.AspNetCoreServer)
- **SQL Server** via Docker Compose for structured metadata
- **DynamoDB Local** for fast, flexible document storage
- **Retry mechanism** to reconcile failed writes (placeholder implementation)
- **Read cache (planned)** to enhance performance and reduce DynamoDB costs

### 🧩 Dependency Injection and Loose Coupling

- The application uses **constructor-based dependency injection** throughout.
- All core components (e.g., repositories, services, time providers, retry mechanisms) are abstracted behind interfaces, following **SOLID** principles.
- This promotes:
  - ✅ **Testability** — easier mocking and unit testing
  - 🔄 **Extensibility** — swap implementations (e.g., switch from DynamoDB to S3, or SQL Server to RDS) without changing the core logic
  - ♻️ **Separation of Concerns** — cleanly decouples infrastructure from business logic

---

## ⚖️ Trade-Offs Made

### 1. DynamoDB as the Primary Read Store
- **Pros**:
  - Enables fast lookup via `UserId` index
  - Reduces load on SQL Server
  - Flexible JSON allows schema evolution
- **Cons**:
  - Requires maintaining consistency manually

### 2. Parallel Writes
- **Pros**:
  - Improves latency
  - Allows independent scalability of write paths
- **Cons**:
  - Risk of partial failures (e.g., SQL succeeds, Dynamo fails)
  - Retry logic must compensate for inconsistencies

### 3. Serverless and Container-First Design
- **Pros**:
  - Easy local simulation with SAM and Docker
  - Cloud-ready without major changes
- **Cons**:
  - Adds complexity for developers unfamiliar with serverless

---

## 🔐 Security & Monitoring Considerations

### Not Implemented (but recommended in a production deployment):
- **Authentication/Authorization** for endpoints
- **Audit logging**
- **Structured logging** (e.g., Serilog + AWS CloudWatch)
- **Metrics and Tracing**: Use of AWS X-Ray, CloudWatch Metrics
- **Validation Improvements**: Use FluentValidation or custom model binding logic

---

## 💰 Cost Strategies

- **DynamoDB**: Chosen for its pay-per-request billing and scalability. Using `UserId` as partition key ensures even distribution and performance.
- **Avoid SQL Reads**: By using DynamoDB for reads, we avoid heavier SQL costs and licensing concerns.
- **Retry Queue (Planned)**: Moves reconciliation to async background processing, freeing up request time.
- **Cache Layer (Planned)**: Reduces reads to Dynamo and helps with hot-path queries.

---

## ✅ Future Enhancements

- Implement persistent retry queue (e.g., SQS FIFO or Dynamo-backed queue)
- Introduce response caching
- Add pagination to `/events/{userId}`
- Expand test coverage (integration tests, retry logic, validation)
- Security hardening (auth + rate limiting)

---

This solution is built with extensibility and cloud-readiness in mind, keeping a balance between simplicity and future scaling needs.