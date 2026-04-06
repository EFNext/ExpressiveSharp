#!/usr/bin/env bash
set -euo pipefail

# Run integration tests against real database containers via Testcontainers.
#
# Usage:
#   ./test-containers.sh                  # All providers (SqlServer, Postgres, PomeloMySql)
#   ./test-containers.sh SqlServer        # SQL Server only
#   ./test-containers.sh Postgres         # PostgreSQL only
#   ./test-containers.sh PomeloMySql      # MySQL (Pomelo) only
#   ./test-containers.sh Cosmos           # Cosmos DB emulator only
#   ./test-containers.sh All              # All providers including Cosmos

PROJECT="tests/ExpressiveSharp.EntityFrameworkCore.IntegrationTests/ExpressiveSharp.EntityFrameworkCore.IntegrationTests.csproj"
CONFIG="Release"

database="${1:-All}"

# Validate argument
case "$database" in
    SqlServer|Postgres|PomeloMySql|Cosmos|All) ;;
    *)
        echo "Unknown database: $database"
        echo "Valid options: SqlServer, Postgres, PomeloMySql, Cosmos, All"
        exit 1
        ;;
esac

echo "==> Building with TestDatabase=$database..."
dotnet build "$PROJECT" -c "$CONFIG" -p:TestDatabase="$database" --verbosity quiet

echo "==> Running tests with TestDatabase=$database..."
dotnet test --project tests/ExpressiveSharp.EntityFrameworkCore.IntegrationTests \
    --no-build -c "$CONFIG" \
    -p:TestDatabase="$database"
