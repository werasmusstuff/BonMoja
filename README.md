# Event Logger API

A serverless-ready backend API in C# for recording and retrieving user event logs. Metadata is stored in SQL Server, and flexible event details are stored in DynamoDB. The two are linked by a shared `eventId`.

---

## 🚀 Quick Start

---

### 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/products/docker-desktop/)
- [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html)

### 📦 Clone & Run

From the root folder of the repo, execute the following commands in order. 
The docker-compose comes with scripts to create the required objects in both the SQL and Dynamo.
```bash
docker network create eventlogger-network
docker-compose up -d
sam build     
sam local start-api --docker-network eventlogger-network
```
---
## 📌 API Endpoints
---
### `POST /events`

Records a new user event.

**Request Body:**

```json
{
  "userId": "12345",
  "eventType": "bet",
  "jsonDetails": {
    "ip": "192.168.1.1",
    "device": "Chrome"
  }
}
```

**Response Example:**

```json
{
  "eventId": "777be327-2a98-44ec-87c2-f50d78cb12e3",
  "sqlSuccess": true,
  "dynamoSuccess": true,
  "retryQueued": false
}
```

---

### `GET /events/{userId}`

Returns all events for a user, ordered by timestamp. Uses DynamoDB as the primary data source.

**Response Example:**

```json
[
  {
    "userId": "12345",
    "eventId": "123e4567-e89b-12d3-a456-426614174000",
    "timestamp": "2025-06-29T14:32:45.123Z",
    "jsonPayload": {
      "ip": "192.168.1.1",
      "device": "Chrome"
    }
  },
  {
    "userId": "12345",
    "eventId": "123e4567-e89b-12d3-a456-426614174001",
    "timestamp": "2025-06-29T14:35:10.456Z",
    "jsonPayload": {
      "ip": "192.168.1.2",
      "device": "Firefox"
    }
  }
]
```
---
## 📝 TODO / Tradeoffs due to Time Constraints

---

- **Expand Testing:**  
  Current test coverage is extremely limited. Additional tests are needed for input validation, concurrency, and edge cases.

- **Implement Pagination:**  
  The GET /events/{userId} endpoint currently returns all events without pagination. This needs to be implemented to handle large datasets efficiently.

- **Add Caching:**  
  Read-side caching is not yet implemented but would significantly improve performance and reduce costs.

- **Enhance Validation:**  
  Input validation is basic and should be improved to ensure data integrity and better error handling.

- **Retry Mechanism Implementation:**  
  The retry queue is currently a placeholder. A durable, persistent queue should be implemented outside the Lambda runtime to handle write failures and eventual consistency.

- **Error Handling and Logging:**  
  Error handling currently lacks comprehensive logging; adding structured logging will aid monitoring and troubleshooting.

- **Security and Monitoring:**  
  Further work is needed to secure API endpoints and add monitoring/alerting for production readiness.

- **Pagination & Rate Limiting:**  
  Adding rate limiting and pagination will help control resource usage and protect against abuse.

- **Deployment Considerations:**  
  Additional work is required for seamless deployment and scaling in actual AWS environments.

---
## 🛠️ Assumptions on Expected Usage Characteristics and Use Cases
---
The assignment did not provide explicit details about expected data volumes, concurrency levels, or specific use cases for the event logging API.  

In designing the overall solution, the following assumptions and priorities guided the implementation:

- **Scalability:** The system should be able to handle high throughput of event writes without bottlenecks.  
- **Cost Efficiency:** Cloud-native, serverless-friendly patterns were favored to minimize operational and infrastructure costs.  
- **Eventual Consistency Acceptable:** Given the nature of event logs, eventual consistency between SQL metadata and DynamoDB payloads was deemed acceptable.  
- **Flexibility and Extensibility:** The data model and API were designed to accommodate evolving requirements without major refactoring.

---

## 💡 Design Decisions & Rationale

---

### 📄 Document-First (Not SQL-First) Design

**Approach:** This solution intentionally follows a **Document-First** approach, using DynamoDB as the primary data source for reading events, rather than relying on SQL Server.

**Rationale:**
- Reading from DynamoDB via the `userId` partition key provides efficient and scalable access.
- By avoiding reads from SQL, we reduce infrastructure complexity, improve horizontal scalability, and minimize cloud costs.
- SQL Server is still used to store consistent, structured metadata for analytical or compliance purposes, but it is not the primary read source.

---

### 2. ⚡ Parallel Writes to SQL Server and DynamoDB

**Decision:** Metadata and payload are written in parallel.

**Rationale:**
- Reduces API response time by handling I/O-bound operations concurrently.
- Allows each store (SQL and DynamoDB) to operate independently.

**Trade-offs:**
- Risk of partial failure (e.g., one write succeeds, the other fails).
- Introduced a retry mechanism to address write inconsistencies.
- Must carefully handle exceptions to ensure retry logic triggers correctly.

---

### 3. 🔁 Retry Mechanism (Stubbed due to time constraints)

**Decision:** Introduced a basic retry mechanism interface for failed writes.

**Rationale:**
- Provides a foundation for guaranteed consistency between SQL and DynamoDB.
- Allows retries to be processed in the background without blocking the user.

**Future Improvement:**
- Implement a background worker (e.g., SQS + Lambda or ECS task) to process retry queue.
- Implement exponential backoff, error logging, and dead-letter queues to handle persistent failures without data loss.

---

### 5. 🕓 ISO 8601 Timestamps

**Decision:** Timestamps are stored using `DateTime.UtcNow.ToString("o")` (ISO 8601 format).

**Rationale:**
- Fully sortable and precise across systems.
- Supported by both SQL Server (`datetime2`) and DynamoDB (`String`).
- Avoids timezone ambiguity.

---

### 6. 🧬 Flexible JSON Payload in DynamoDB

**Decision:** `JsonDetails` is stored as raw JSON in a `JsonPayload` attribute.

**Rationale:**
- Supports dynamic event structures (e.g., IP address, device info, location, etc.).
- Allows schema to evolve without migrations or code changes.

**Trade-offs:**
- No validation upfront.
- May need transformation when querying or analyzing data.

---

### 8. 💸 Cost Awareness

**Decision:** Minimize SQL usage during read operations.

**Rationale:**
- Reading from DynamoDB is typically cheaper and faster at scale.
- Helps reduce cloud hosting costs when running on AWS.

---
## 📊 Architecture & Sequence Diagrams

---

### 🧱 1. System Architecture Overview

![System Architecture Overview](./docs/overview.png)

---

### ✍️ 2. Write Sequence (POST /events)

![Write Sequence](./docs/writes.png)

---

### 📥 3. Read Sequence (GET /events/{userId})

![Read Sequence](./docs/reads.png)

