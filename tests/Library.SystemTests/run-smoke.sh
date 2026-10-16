#!/usr/bin/env bash
set -euo pipefail
# Run after `docker compose up --build`; requires grpcurl.
TARGET="${TARGET:-localhost:8081}"
PROTO_DIR="$(cd "$(dirname "$0")/../../src/Library.Contracts/Protos" && pwd)"
PROTO="$PROTO_DIR/library.proto"
call() { local method="$1"; shift; grpcurl -plaintext -import-path "$PROTO_DIR" -proto "$PROTO" "$@" "$TARGET" "$method"; }
call library.v1.LibraryRpc/Health
created="$(call library.v1.LibraryRpc/CreateBook -d '{"title":"System Test Book","author":"Test Author","pages":120,"copies":1}')"
echo "$created"
book_id="$(printf '%s' "$created" | sed -n 's/.*"id": *"\([^"]*\)".*/\1/p')"
test -n "$book_id"
borrower="$(call library.v1.LibraryRpc/RegisterBorrower -d '{"name":"System Test Borrower","email":"system-test@example.com"}')"
echo "$borrower"
borrower_id="$(printf '%s' "$borrower" | sed -n 's/.*"id": *"\([^"]*\)".*/\1/p')"
test -n "$borrower_id"
loan="$(call library.v1.LibraryRpc/BorrowBook -d "{\"bookId\":\"$book_id\",\"borrowerId\":\"$borrower_id\"}")"
echo "$loan"
loan_id="$(printf '%s' "$loan" | sed -n 's/.*"id": *"\([^"]*\)".*/\1/p')"
test -n "$loan_id"
call library.v1.LibraryRpc/ReturnBook -d "{\"loanId\":\"$loan_id\"}"
echo "System smoke flow passed."
