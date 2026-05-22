# ShopFlow

> Full-stack marketplace: a .NET 8 API, a Next.js 14 web store, and a React Native (Expo) mobile app, organized as a monorepo.

![status](https://img.shields.io/badge/status-in%20development-yellow)
![license](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)

## Features

- 🛒 Product catalog with search and category filtering
- 🔐 JWT authentication with access roles (customer and administrator)
- 🧺 Per-user persistent shopping cart
- 📦 Order creation from the cart, with stock control
- 💳 Checkout integrated with Stripe (test mode)
- 🖼️ Product image upload via Cloudinary
- 🛠️ Admin panel for product management

## Stack

| Layer | Technologies |
|-------|--------------|
| Backend | ASP.NET Core 8, Entity Framework Core, PostgreSQL, JWT, Serilog, xUnit, Testcontainers |
| Web | Next.js 14 (App Router), TypeScript, Tailwind CSS, shadcn/ui, TanStack Query, Zustand, Jest, Playwright |
| Mobile | Expo SDK 51, TypeScript, Expo Router, React Query, Zustand, Jest |
| Infrastructure | Docker, GitHub Actions, Vercel, Railway/Fly.io, Supabase, Cloudinary, Sentry |

## Monorepo structure

```
shopflow/
├── apps/
│   ├── api/      # ASP.NET Core 8 (ShopFlow.Api + ShopFlow.Tests)
│   ├── web/      # Next.js 14
│   └── mobile/   # React Native + Expo
├── docs/
│   └── adrs/     # Architecture Decision Records
├── .github/workflows/
├── docker-compose.yml
└── .env.example
```

## Prerequisites

- .NET SDK 8.0+
- Node.js 20+
- Docker + Docker Compose (for local PostgreSQL and integration tests)
- Expo CLI / Expo Go app on your phone (for the mobile app)

## Getting started

### Backend (API)

```bash
# Start the local PostgreSQL (or point to a managed Postgres instead)
cp .env.example .env   # fill in the variables
docker compose up -d postgres

# Configure local settings (kept outside the repository)
cd apps/api/ShopFlow.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=...;Port=5432;Database=...;Username=...;Password=..."
dotnet user-secrets set "Jwt:Secret" "$(openssl rand -base64 64)"

# (optional) create an admin user via the development seed
dotnet user-secrets set "Seed:AdminEmail" "admin@example.local"
dotnet user-secrets set "Seed:AdminPassword" "$(openssl rand -base64 24)"

# Apply migrations and run
dotnet ef database update
dotnet run
```

The API runs at `http://localhost:5000` with Swagger at `/swagger`.

### Web

```bash
cd apps/web
npm install
npm run dev
```

### Mobile

```bash
cd apps/mobile
npm install
npx expo start
```

## API

Interactive documentation is available via Swagger at `/swagger` while the API is running.

| Resource | Routes | Access |
|----------|--------|--------|
| Authentication | `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me` | public / authenticated |
| Products | `GET /api/products`, `GET /api/products/{id}` | public |
| Products (management) | `POST`, `PUT`, `DELETE /api/products` | administrator |
| Cart | `GET /api/cart`, `POST /api/cart/items`, `PUT`/`DELETE /api/cart/items/{id}` | authenticated |
| Orders | `POST /api/orders`, `GET /api/orders`, `GET /api/orders/{id}` | authenticated |
| Health check | `GET /health` | public |

Authentication uses a JWT token in the `Authorization: Bearer <token>` header. Errors follow the `ProblemDetails` format (RFC 7807).

## Roadmap

- [x] REST API — authentication, catalog, cart, and orders
- [ ] Automated test coverage and continuous integration
- [ ] Web store (Next.js)
- [ ] Mobile app (Expo)
- [ ] Stripe checkout
- [ ] Production deployment

## License

[MIT](./LICENSE)
