# Library Management gRPC API (.NET 8)

A layered, containerized library API built with .NET 8, gRPC/Protocol Buffers and EF Core/SQLite. The edge API communicates with a separate internal gRPC service; the internal service owns business rules and persistence.

## Projects

- `Library.Contracts` — protobuf contract shared by both processes.
- `Library.Api` — external gRPC gateway; forwards calls to the internal service, propagates expected gRPC statuses, applies deadlines/cancellation and bounded retry for transient `Unavailable` responses.
- `Library.Service` — business logic, validation, analytics, EF Core and SQLite persistence.
- `Library.UnitTests` — warm-up exercise tests and edge cases.
- `Library.IntegrationTests` — SQLite schema, persistence, relationships and uniqueness constraints.

## Requirements

- .NET SDK 8.0+
- Docker Engine and Docker Compose (for containerized execution)
- Optional: `grpcurl` for manual gRPC calls

## Run with Docker

From the repository root:

```bash
docker compose up --build
```

The API listens on `localhost:8081` using HTTP/2 cleartext (h2c), suitable for local development. The internal service is only exposed on the Compose network. SQLite data is persisted in the `library-data` volume. Sample books and borrowers are inserted on first start.

Stop the stack with `docker compose down`. To remove the database volume too, use `docker compose down -v` (this deletes persisted data).

## Run tests

```bash
dotnet restore Library.sln
dotnet test Library.sln --configuration Release
```

The integration tests use a temporary in-memory SQLite database and do not require Docker. To exercise the complete containerized flow, start Compose and use the protobuf contract with `grpcurl` (reflection is intentionally not enabled):

```bash
grpcurl -plaintext -import-path src/Library.Contracts/Protos -proto library.proto localhost:8081 library.v1.LibraryRpc/Health

grpcurl -plaintext -import-path src/Library.Contracts/Protos -proto library.proto \
  -d '{"title":"Refactoring","author":"Martin Fowler","pages":448,"copies":2}' \
  localhost:8081 library.v1.LibraryRpc/CreateBook
```

## RPC operations

| RPC | Purpose |
|---|---|
| `CreateBook`, `GetBook`, `ListBooks` | Book catalogue and paginated listing |
| `RegisterBorrower` | Register a borrower; email is unique |
| `BorrowBook`, `ReturnBook` | Lending lifecycle and inventory counts |
| `GetMostBorrowedBooks` | Rank books by borrow events in a time window |
| `GetMostActiveBorrowers` | Rank borrowers by borrow events in a time window |
| `GetReadingPace` | Estimate pages/day using total book pages divided by elapsed borrow/return days |
| `GetRelatedBooks` | Find books borrowed by borrowers who also borrowed a specified book |
| `Health` | Application-level RPC health probe |

## Error contract

The service uses standard gRPC status codes: `InvalidArgument` for invalid input, `NotFound` for missing records, `AlreadyExists` for duplicate borrower email, `FailedPrecondition` for unavailable copies or repeated returns, `Internal` for unexpected backend exceptions, and `Unavailable` for unexpected gateway/internal-service connectivity failures. Detailed exception text is not sent to clients; errors are logged server-side. Calls propagate cancellation and deadlines.

## Warm-up tasks

`Library.Service.Utilities.BookExercises` contains the power-of-two check, title reversal, title replication, and odd IDs from 1 through 99. Replication rejects negative counts and caps output at one million characters to avoid accidental excessive allocation.

## Design notes and production hardening

- Keep API and service independently deployable; only the API port is published by Compose.
- The sample uses SQLite for a zero-configuration demonstration. For multi-replica production deployment, use a server database (e.g. PostgreSQL or SQL Server), schema migrations, and concurrency-safe inventory updates/transactions. SQLite is not intended as a shared multi-replica database.
- The current startup uses `EnsureCreated` for assignment simplicity; production deployment should use reviewed EF Core migrations and a controlled migration job.
- The API's local h2c endpoint is not encrypted. Terminate TLS at a trusted ingress/reverse proxy or configure TLS for any non-local environment.
- Authentication was explicitly out of scope in the assignment. Add authentication, authorization, rate limiting, secrets management and audit policies before public exposure.
- Reading pace is an estimate assuming continuous reading. A minimum duration of one hour avoids division by zero for same-day loans. The metric does not claim to measure actual reading behavior.
- Analytics include borrow events whose `BorrowedAt` is within the inclusive `[from, to]` interval. A missing `from` defaults to the earliest representable timestamp, a missing `to` to now, and `limit` is bounded to 1–100.

## System smoke test

After `docker compose up --build`, run `tests/Library.SystemTests/run-smoke.sh` (requires `grpcurl`). It exercises the public gateway, internal gRPC hop, database persistence, borrower registration, borrow and return workflow. The script uses a fixed test email and is intended for a fresh/demo database; remove that borrower row or use a new email before rerunning against a persistent volume.
