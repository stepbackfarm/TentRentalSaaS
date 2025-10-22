# Tent Rental SaaS

Event tent rental platform with booking management, Stripe payments, and customer portal.

## Architecture

- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: React 19 + Vite + Tailwind CSS
- **Database**: PostgreSQL 16
- **Payments**: Stripe
- **Testing**: Playwright (E2E), xUnit (backend unit tests)

## Quick Start

### For Contractors (Docker-Only Setup)

**Prerequisites**: Docker Desktop

```bash
# One-time setup
npm run setup

# Start the full stack
npm run docker:full

# Visit http://localhost:5173
```

### For Development (Host-Based Setup)

**Prerequisites**: Docker, Node.js 20+, .NET 8 SDK

```bash
# One-time setup
npm run setup

# Start dev servers (fast hot-reload)
npm run dev:all

# Visit http://localhost:5173
```

## Development Workflows

### Available Commands

```bash
# Full Docker stack (database + API + frontend)
npm run docker:full

# Host-based development (recommended for AI agents)
npm run dev:all          # Start all services
npm run dev:db           # Database only
npm run dev:api          # API only (requires db)
npm run dev:web          # Frontend only

# Testing
npm run test:e2e         # Run Playwright E2E tests (headless)
npm run test:e2e:ui      # Playwright interactive UI mode
npm run test:e2e:headed  # Watch tests run in browser

# Cleanup
npm run docker:full:down # Stop and remove all containers + volumes
```

### Service URLs

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000
- **API Health**: http://localhost:5000/health
- **Swagger**: http://localhost:5000/swagger (Development only)
- **Database**: localhost:5433

## Testing

### End-to-End Tests (Playwright)

```bash
# Run all E2E tests
npm run test:e2e

# Interactive mode (recommended for development)
npm run test:e2e:ui

# Watch tests run in browser
npm run test:e2e:headed
```

**Playwright MCP Integration**: 

If using Playwright MCP for ad-hoc testing with AI agents:
- Set `PW_NO_SERVER=1` to reuse already-running dev servers
- Tests automatically start services if not running

### Backend Unit Tests

```bash
cd backend/tests
dotnet test
```

## Environment Variables

### Root `.env`
```env
POSTGRES_PORT=5433
API_PORT=5000
WEB_PORT=5173
ALLOWED_ORIGINS=http://localhost:5173
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

### Backend `.env` (for host-based dev)
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__Default=Host=localhost;Port=5433;Database=tent-rental-db;...
STRIPE_SECRET_KEY=sk_test_...
GOOGLE_MAPS_API_KEY=...
```

### Frontend `.env`
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

**Note**: Copy `.env.example` files to `.env` and update with your API keys.

## Project Structure

```
src/
├── backend/              # ASP.NET Core API
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── tests/            # xUnit tests
│   └── Dockerfile
├── frontend/             # React + Vite
│   ├── src/
│   │   ├── pages/
│   │   ├── components/
│   │   └── services/
│   └── Dockerfile
├── tests/
│   └── e2e/              # Playwright tests
├── docs/                 # Detailed documentation
├── docker-compose.yml    # Full stack
├── docker-compose.dev.yml # Dev overrides
├── playwright.config.ts
├── setup.sh              # Automated setup
└── package.json          # Root orchestration
```

## Troubleshooting

### Port Conflicts

If ports are already in use:
```bash
# Check what's using a port
lsof -i :5000  # or :5173, :5433

# Stop old containers
docker ps -a
docker rm -f <container-id>
```

### Database Issues

```bash
# Reset database
npm run docker:full:down  # Removes volume
npm run dev:db

# View logs
docker compose logs db

# Manual migration
cd backend
dotnet ef database update
```

### CORS Errors

Ensure `ALLOWED_ORIGINS` in backend includes `http://localhost:5173`.

### Playwright Browser Dependencies

```bash
# Install/update Playwright browsers
npx playwright install --with-deps
```

## Documentation

- [Detailed Setup Guide](docs/SETUP.md) - Step-by-step instructions
- [Project Plan](TentRentalSaaS-Project-Plan.md)
- [Implementation Plans](Implementation-Plan-Phase-*.md)

## Contributing

1. Create feature branch
2. Make changes with tests
3. Run tests locally: `npm run test:e2e`
4. Submit PR

## License

Proprietary
