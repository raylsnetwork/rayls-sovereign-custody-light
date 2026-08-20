<div align="center">

# Rayls Custody HSM API

**HSM-backed wallet custody API for the Rayls network — wallet management, EVM transactions, and token operations, with AWS KMS or local keystore key handling.**

[![License: Apache 2.0][license-badge]][license-url]
[![.NET][dotnet-badge]][dotnet-url]
[![Docker][docker-badge]][docker-url]

[![Discord][discord-badge]][discord-url]
[![X][x-badge]][x-url]
[![LinkedIn][linkedin-badge]][linkedin-url]
[![YouTube][youtube-badge]][youtube-url]

[Overview](#overview) | [Local development](#local-development-docker-compose) | [Environment variables](#environment-variables) | [API](#api-endpoints) | [License](#license)

</div>

## Overview

The Rayls Custody HSM API is a .NET 8.0 web application that provides:

- **Wallet Management**: Create and manage cryptocurrency wallets with HSM-backed security
- **Transaction Processing**: Handle cryptocurrency transactions with multi-signature support
- **Token Operations**: Manage ERC-20 tokens and smart contract interactions
- **Authentication**: Secure API authentication with JWT tokens
- **AWS Integration**: AWS KMS integration for cryptographic key management
- **Database**: PostgreSQL with Entity Framework Core

## Architecture

```
├── Rayls.Custody.HSM.API/          # Web API controllers and configuration
│   ├── Controllers/                 # AuthController, TokenController, TransactionController, WalletController
│   ├── Middlewares/                 # Error handling middleware
│   ├── Properties/                  # Launch settings
│   └── Startup*.cs                  # App, AutoMapper, Aws, Http, Swagger configuration
├── Rayls.Custody.HSM.DTO/          # Data Transfer Objects and client providers
│   ├── ClientProviders/             # HTTP client providers for external APIs
│   ├── Configuration/               # API configuration classes
│   ├── Const/                       # Constants (Routes, Errors)
│   ├── Enums/                       # Enumerations (OperationType, WalletType)
│   ├── Errors/                      # Error response types (ApiError, HSMApiErrorDTO, HSMIntegrationException)
│   └── Helpers/                     # Validation extensions and Swagger filters
├── Rayls.Custody.HSM.Repository/   # Data access layer and AWS KMS integration
│   ├── PostgreSql/                  # Entity Framework DbContext (RaylzDbContext)
│   └── *Repository.cs               # Token, Transaction, Wallet, AwsKmsKey repositories
└── Rayls.Custody.HSM.Service/      # Business logic and service implementations
    ├── Interface/                   # Service and repository interfaces
    ├── SmartContractConsoles/       # ERC20 smart contract interactions
    ├── ValidationException/         # Custom validation exceptions
    └── *Service.cs                  # Auth, Token, Transaction, Wallet services
```

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- PostgreSQL (handled by Docker Compose in development)

### Dependencies

- **Runtime**: .NET 8.0
- **Database**: PostgreSQL with Entity Framework Core 8
- **Cloud**: AWS KMS (optional, set `USES_AWS=false` for keystore mode)
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Blockchain**: Nethereum 4.16 (EVM/Web3)
- **Monitoring**: Datadog

All dependencies are available on the public [NuGet](https://api.nuget.org/v3/index.json) feed — no private feed or access token required.

## Local Development (Docker Compose)

### 1. Clone the Repository

```bash
git clone https://github.com/raylsnetwork/rayls-sovereign-custody-light
cd rayls-sovereign-custody-light
```

### 2. Configure Environment

```bash
cp .env.example .env
# Edit .env with your values (see Environment Variables section)
```

### 3. Start the Stack

```bash
docker compose -f docker-compose.dev.yml up
```

This starts:
- **API** on `http://localhost:5032` with hot-reload (`dotnet watch`)
- **PostgreSQL** on `localhost:5433`

API endpoints:
- Swagger UI: `http://localhost:5032/swagger`
- ReDoc: `http://localhost:5032/redoc`

## Docker Build (Production)

```bash
# Build image
docker build -t rayls-custody-hsm-api:latest .

# Run container
docker run -p 5000:5000 \
  -e POSTGRESQL_SERVER=<host> \
  -e POSTGRESQL_PORT=5432 \
  -e POSTGRESQL_USERNAME=postgres \
  -e POSTGRESQL_PASSWORD=<password> \
  -e POSTGRESQL_DBNAME=raylzdb \
  -e AUTH_API_KEY=<key> \
  -e AUTH_JWT_SECRET=<secret> \
  -e USES_AWS=false \
  rayls-custody-hsm-api:latest
```

> No `NPM_PERSONAL_ACCESS_TOKEN` or Azure DevOps credentials needed — all packages are on public NuGet.

## Environment Variables

| Variable | Description | Example |
|---|---|---|
| `POSTGRESQL_SERVER` | PostgreSQL host | `localhost` |
| `POSTGRESQL_PORT` | PostgreSQL port | `5432` |
| `POSTGRESQL_USERNAME` | Database user | `postgres` |
| `POSTGRESQL_PASSWORD` | Database password | `secret` |
| `POSTGRESQL_DBNAME` | Database name | `raylzdb` |
| `AUTH_API_KEY` | API key for authentication | `your-api-key` |
| `AUTH_JWT_SECRET` | Secret for signing JWT tokens | `your-256-bit-secret` |
| `USES_AWS` | Enable AWS KMS (`true`/`false`) | `false` |
| `KMS_MAX_WALLETS` | Max wallets via AWS KMS | `200` |
| `KEYSTORE_MAX_WALLETS` | Max wallets via keystore | `20` |
| `EVM_JSON_RPC` | EVM JSON-RPC endpoint | `http://node:8545` |
| `EVM_JSON_RPC_CHAIN_ID` | EVM chain ID | `600001` |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `LOC`, `DEV`, `STG` |

## API Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/auth` | Get JWT token | No |
| `POST` | `/api/wallet` | Create wallet(s) | JWT |
| `GET` | `/api/wallet` | List wallets (paginated) | JWT |
| `GET` | `/api/wallet/{id}` | Get wallet by ID | JWT |
| `GET` | `/api/wallet/address/{address}` | Get wallet by address | JWT |
| `PATCH` | `/api/wallet/{id}/external/{external_id}` | Associate external ID | JWT |
| `POST` | `/api/token` | Register token | JWT |
| `GET` | `/api/token` | List tokens (paginated) | JWT |
| `GET` | `/api/token/{id}` | Get token by ID | JWT |
| `GET` | `/api/token/{id}/balance/{walletId}` | Get token balance | JWT |
| `POST` | `/api/transaction` | Create EVM transaction | JWT |
| `POST` | `/api/transaction/view` | View transaction | JWT |
| `POST` | `/api/transaction/transfer_token` | Transfer token | JWT |
| `POST` | `/api/transaction/transfer_token_bridge` | Bridge token transfer | JWT |
| `GET` | `/api/transaction/{hash}` | Get transaction by hash | JWT |
| `GET` | `/api/transaction/receipt/{hash}` | Get transaction receipt | JWT |

## Troubleshooting

### PostgreSQL connection error

```bash
# Check database is running
docker compose -f docker-compose.dev.yml ps

# Test connectivity
psql -h localhost -p 5433 -U postgres -d raylzdb
```

### AWS KMS access denied

Set `USES_AWS=false` to use the local keystore instead of AWS KMS, or configure AWS credentials:

```bash
export AWS_ACCESS_KEY_ID=your_access_key
export AWS_SECRET_ACCESS_KEY=your_secret_key
export AWS_DEFAULT_REGION=eu-west-2
```

### Port already in use

```bash
# Check what's on port 5032
sudo lsof -i :5032
# Or change the port in docker-compose.dev.yml
```

## Security Considerations

- Never commit secrets or `.env` files to version control
- Use environment variables for all sensitive configuration
- Use HTTPS in production
- Rotate `AUTH_JWT_SECRET` and `AUTH_API_KEY` regularly

## Deployment

```bash
# Build production image
docker build -t rayls-custody-hsm-api:production .

# Tag and push to registry
docker tag rayls-custody-hsm-api:production your-registry.com/rayls-custody-hsm-api:latest
docker push your-registry.com/rayls-custody-hsm-api:latest
```

## Contributing

We are not accepting external contributions at this time — see [CONTRIBUTING.md](./CONTRIBUTING.md). Please also read our [Code of Conduct](./CODE_OF_CONDUCT.md).

## Security

To report a security vulnerability, see [SECURITY.md](./SECURITY.md) — please do not open a public issue.

## License

Licensed under the Apache License, Version 2.0 — see [LICENSE](./LICENSE).

Copyright 2026 Rayls Core Ltd.

Dependencies are pulled from the public NuGet feed and keep their own licenses. Note that
`Dinamo.Hsm` is a proprietary vendor SDK; using the Dinamo HSM integration requires your own
license from the vendor.

[license-badge]: https://img.shields.io/badge/License-Apache_2.0-blue.svg
[license-url]: ./LICENSE
[dotnet-badge]: https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com
[docker-badge]: https://img.shields.io/badge/Docker-supported-2496ED?logo=docker&logoColor=white
[docker-url]: https://www.docker.com
[discord-badge]: https://img.shields.io/badge/Discord-join%20chat-5865F2?logo=discord&logoColor=white
[discord-url]: https://discord.gg/6THZ96357r
[x-badge]: https://img.shields.io/badge/X-%40RaylsLabs-000000?logo=x&logoColor=white
[x-url]: https://x.com/RaylsLabs
[linkedin-badge]: https://img.shields.io/badge/LinkedIn-Rayls-0A66C2?logo=linkedin&logoColor=white
[linkedin-url]: https://www.linkedin.com/company/rayls/
[youtube-badge]: https://img.shields.io/badge/YouTube-Rayls-FF0000?logo=youtube&logoColor=white
[youtube-url]: https://www.youtube.com/@Rayls_blockchain
