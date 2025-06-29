# Backend developer take-home assignment

## Context

You are joining a product engineering team responsible for building and maintaining a high-throughput backend system that processes user interactions and stores data in both relational (SQL Server) and non-relational (DynamoDB) databases. The team follows modern cloud-native principles using AWS and serverless technologies.

This exercise simulates a common feature task where you’ll build a small backend API to manage and retrieve "event logs" for user actions in an application. The system should store structured metadata in SQL Server and flexible JSON blobs in DynamoDB.

---

## Requirements

Your task is to build a minimal backend API that meets the following requirements:

### 1. API design

* Create a RESTful API with the following endpoints:

  * `POST /events`: Record a new user event.
  * `GET /events/{userId}`: Return a list of all events for a given user.

### 2. Data modeling

* Store event metadata (e.g., `userId`, `eventType`, `timestamp`) in SQL Server.
* Store flexible event details (e.g., nested JSON blobs) in DynamoDB.
* Link the data across both databases via a shared `eventId`.

### 3. Serverless local simulation

* Configure the project to run locally using:

  * `Amazon.Lambda.AspNetCoreServer`
  * AWS SAM CLI to simulate deployment locally (no actual AWS deployment needed)
  * Provide a valid `template.yaml` SAM template

### 4. Infrastructure

* Use the provided `docker-compose.yml` to spin up local SQL Server and DynamoDB Local instances.

### 5. Testing

* Write unit tests for your API logic.
* Include a test suite that covers:

  * Happy path behavior
  * Input validation and edge cases
  * Error scenarios (e.g., database failures)

### 6. API documentation

* Include documentation of the API (e.g., using Swagger/OpenAPI or Markdown).
* Must include:

  * Endpoint URLs
  * HTTP methods
  * Request/response schemas
  * Status codes and error formats

---

## Deliverables

Please submit the following:

1. **GitHub repository**

   * Your solution code (e.g., `Program.cs`, `Startup.cs`, controller classes, data models, SAM template)
   * A valid `docker-compose.yml` file
   * A working `template.yaml` for AWS SAM
   * A test project with test files and instructions for running tests
   * API documentation files (Swagger config or Markdown)

2. **README.md**

   * Clear instructions on how to run your app and test suite locally using SAM and Docker
   * Overview of your technical decisions

3. **SOLUTION.md**

   * A brief explanation of your approach, any tradeoffs, and areas for future improvement

4. **Demo video (5–10 minutes)**

   * Record yourself demoing your solution using Loom or similar
   * Walk through your API endpoints, code structure, and key decisions

---

## Evaluation criteria

| Area              | What we look for                                                 |
| ----------------- | ---------------------------------------------------------------- |
| Functionality     | Meets the core requirements, correct handling of input/output    |
| Code quality      | Clean, idiomatic C#, good use of design patterns                 |
| Data modeling     | Correct use of SQL vs. NoSQL, sound data structure choices       |
| Architecture      | Logical separation of concerns, scalable design                  |
| Testing           | Comprehensive test coverage, meaningful assertions, maintainable |
| API documentation | Clear, accurate documentation useful to front-end developers     |
| Documentation     | Clear setup instructions and rationale for decisions             |

---

## Time estimate

We expect this exercise to take approximately 3–4 hours. Please document any tradeoffs you made to stay within this time frame.

