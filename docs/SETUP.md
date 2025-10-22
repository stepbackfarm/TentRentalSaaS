# Detailed Setup Guide

Complete guide for setting up the TentRental SaaS development environment.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Initial Setup](#initial-setup)
3. [Development Workflows](#development-workflows)
4. [Playwright MCP Integration](#playwright-mcp-integration)
5. [Configuration](#configuration)
6. [Troubleshooting](#troubleshooting)

## Prerequisites

### For All Developers

- **Docker Desktop** (or Docker Engine + Docker Compose plugin)
  - Linux Mint: `sudo apt install docker.io docker-compose-v2`
  - macOS: Download from docker.com
  - Windows: Download Docker Desktop

### For Host-Based Development (Recommended for AI Agents)

- **Node.js 20+** and npm
  - Linux: `curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash - && sudo apt install -y nodejs`
  - macOS: `brew install node@20`
  - Windows: Download from nodejs.org

- **.NET 8 SDK**
  - Linux: Follow https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu
  - macOS: `brew install dotnet@8`
  - Windows: Download from microsoft.com/net

### Verify Prerequisites

```bash
docker --version          # Should show Docker version
docker compose version    # Should show Compose version
node --version            # Should show v20.x.x
npm --version             # Should show 10.x.x or higher
dotnet --version          # Should show 8.0.x
```

## Initial Setup

### Step 1: Clone and Navigate

```bash
cd /path/to/TentRentalSaaS/src
```

### Step 2: Run Setup Script

```bash
npm run setup
```

**What the setup script does:**

1. ✅ Checks prerequisites (Docker, Node, .NET)
2. ✅ Creates .env files from templates
3. ✅ Installs root dev dependencies (Playwright, concurrently, cross-env)
4. ✅ Installs Playwright browsers with system dependencies
5. ✅ Installs frontend npm dependencies
6. ✅ Starts PostgreSQL database in Docker
7. ✅ Installs dotnet-ef tool globally (if not present)
8. ✅ Applies EF Core database migrations
9. ✅ Smoke tests the backend health endpoint

**Duration**: 3-5 minutes (first time), 1-2 minutes (subsequent runs)

### Step 3: Configure Environment Variables

#### Root `.env`

The setup script creates this from `.env.example`. Update if needed:

```env
POSTGRES_PORT=5433
API_PORT=5000
WEB_PORT=5173
ALLOWED_ORIGINS=http://localhost:5173
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_51SDGz3...
```

#### Backend `.env`

Located at `backend/.env`:

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:5000
ConnectionStrings__Default=Host=localhost;Port=5433;Database=tent-rental-db;Username=postgres;***REMOVED***
ALLOWED_ORIGINS=http://localhost:5173
STRIPE_SECRET_KEY=sk_test_... # Replace with your test key
GOOGLE_MAPS_API_KEY=...       # Replace with your API key
```

#### Frontend `.env`

Located at `frontend/.env`:

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

**Important**: The `.env` files are gitignored. Keep your keys secure.

## Development Workflows

### Workflow 1: Full Docker Stack

**Best for**: Contractors, CI/CD, production-like testing

```bash
npm run docker:full
```

- Starts all services in Docker with hot-reload
- Database, API, and frontend all containerized
- Code changes auto-reload via volume mounts

**Access**:
- Frontend: http://localhost:5173
- API: http://localhost:5000
- Health check: http://localhost:5000/health

**Stop**: `Ctrl+C` then `npm run docker:full:down` to clean up

### Workflow 2: Host-Based Development

**Best for**: AI agent development, fastest iteration

```bash
npm run dev:all
```

- Database runs in Docker
- API and frontend run natively on host
- Fastest hot-reload and debugging

**What runs**:
- `concurrently` starts API and frontend in parallel
- API: `dotnet watch` (hot reload)
- Frontend: `vite` dev server (HMR)

**Individual services**:
```bash
npm run dev:db    # Database only
npm run dev:api   # API only (requires database)
npm run dev:web   # Frontend only
```

### Workflow 3: Manual Docker

For fine-grained control:

```bash
# Start just the database
docker compose up -d db

# Start API and web without hot-reload (prod-like)
docker compose up api web

# With dev overrides (hot-reload)
docker compose -f docker-compose.yml -f docker-compose.dev.yml up
```

## Playwright MCP Integration

If you're using the Playwright MCP for AI agent testing:

### Option 1: Auto-Start Services (Default)

```bash
npm run test:e2e
```

Playwright will automatically start `dev:all` and run tests.

### Option 2: Reuse Running Services

If you already have `npm run dev:all` or `npm run docker:full` running:

```bash
PW_NO_SERVER=1 npm run test:e2e
```

This skips the webServer startup and uses your existing services.

### Option 3: Interactive UI Mode

```bash
npm run test:e2e:ui
```

Opens Playwright's interactive test runner. Great for:
- Writing new tests
- Debugging failures
- Inspecting element selectors
- Time-travel debugging

### Option 4: Headed Mode

```bash
npm run test:e2e:headed
```

Watch tests run in real browsers. Useful for visual debugging.

### MCP Ad-Hoc Testing Workflow

1. Start services: `npm run dev:all`
2. Use Playwright MCP to run ad-hoc tests against `http://localhost:5173`
3. MCP can directly control the browser and inspect elements
4. When done, stop services with `Ctrl+C`

## Configuration

### Adjusting Paths

If your project structure differs from the default, update these files:

**`setup.sh`**:
```bash
BACKEND_DIR="backend"              # Path to backend
FRONTEND_DIR="frontend"            # Path to frontend
API_PROJECT="${BACKEND_DIR}/TentRentalSaaS.Api.csproj"  # .csproj path
```

**`package.json` (root)**:
```json
{
  "scripts": {
    "dev:api": "dotnet watch --project backend/TentRentalSaaS.Api.csproj"
  }
}
```

### Port Configuration

Ports are defined in root `.env`:

```env
POSTGRES_PORT=5433  # Host port for database
API_PORT=5000       # Host port for API
WEB_PORT=5173       # Host port for frontend
```

Change if you have conflicts. Remember to update:
- `VITE_API_BASE_URL` in frontend `.env`
- `ALLOWED_ORIGINS` in backend config

### Database Configuration

**Host-based development**: Uses `localhost:5433`

`backend/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=tent-rental-db;Username=postgres;***REMOVED***"
  }
}
```

**Docker-based**: Uses `db:5432` (Docker network)

Configured via environment variable in docker-compose.yml.

## Troubleshooting

### Setup Script Fails

**"Missing prerequisite: docker"**
- Install Docker: `sudo apt install docker.io` (Linux)
- Ensure Docker daemon is running: `sudo systemctl start docker`
- Add user to docker group: `sudo usermod -aG docker $USER` then logout/login

**"Database did not become healthy"**
```bash
# Check if port 5433 is already in use
lsof -i :5433

# Check database logs
docker compose logs db

# Stop conflicting containers
docker ps -a
docker stop <container-id>
```

**"Backend health check failed"**
```bash
# View backend logs
cat /tmp/api-setup.log

# Common issues:
# - Port 5000 already in use
# - Database not ready
# - Missing environment variables
```

### Runtime Issues

**CORS Errors in Browser Console**

Ensure backend `ALLOWED_ORIGINS` includes `http://localhost:5173`:

```bash
# In backend/.env or as environment variable
ALLOWED_ORIGINS=http://localhost:5173
```

**Frontend Can't Reach API**

Check `VITE_API_BASE_URL`:
```bash
# In frontend/.env
VITE_API_BASE_URL=http://localhost:5000/api
```

Restart frontend: `Ctrl+C` then `npm run dev:web`

**Database Connection Errors**

```bash
# Check database is running
docker ps | grep tentrental-db

# Restart database
docker compose restart db

# Check connection string in backend/.env or appsettings.Development.json
# For host-based: Host=localhost;Port=5433
# For Docker: Host=db;Port=5432
```

**Port Already in Use**

```bash
# Find what's using the port
lsof -i :5000  # Replace with your port

# Kill the process
kill -9 <PID>

# Or use different ports in .env
```

**Playwright Browsers Missing**

```bash
# Reinstall browsers
npx playwright install --with-deps

# On Linux, may need system dependencies
sudo npx playwright install-deps
```

**Hot Reload Not Working**

**Docker**: Ensure you're using `docker-compose.dev.yml`:
```bash
npm run docker:full  # Uses both compose files
```

**Host-based**: Check file watchers:
```bash
# Linux: Increase inotify watches
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

### Testing Issues

**Tests Timeout Waiting for Server**

If `webServer` in `playwright.config.ts` times out:
```bash
# Start services manually first
npm run dev:all

# Then run tests without auto-start
PW_NO_SERVER=1 npm run test:e2e
```

**Tests Fail Due to Stripe/Auth**

Some flows require real API keys. For testing:
- Use Stripe test mode keys
- Set `TEST_MODE=true` in backend `.env` to bypass OAuth
- Mock external services as needed

### Clean Slate Reset

If everything is broken:

```bash
# Stop all containers
docker compose down -v --remove-orphans

# Remove node_modules
rm -rf node_modules frontend/node_modules

# Re-run setup
npm run setup
```

## Platform-Specific Notes

### Linux Mint / Ubuntu

- Use `apt` package manager
- May need `sudo` for Docker commands if not in docker group
- Ensure Docker service is enabled: `sudo systemctl enable docker`

### macOS

- Use Homebrew for package installation
- Docker Desktop handles Docker daemon
- May need to adjust file sharing in Docker Desktop settings

### Windows (WSL2 Recommended)

- Use WSL2 with Ubuntu for best experience
- Docker Desktop integrates with WSL2
- All Linux instructions apply within WSL2
- Avoid mixing Windows and WSL2 file systems (use WSL2 filesystem)

## Additional Resources

- [Playwright Documentation](https://playwright.dev)
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/aspnet/core)
- [Vite Documentation](https://vitejs.dev)
- [Docker Compose Documentation](https://docs.docker.com/compose)

## Getting Help

1. Check this guide's Troubleshooting section
2. Review logs:
   - Backend: `/tmp/api-setup.log` or console output
   - Database: `docker compose logs db`
   - Frontend: Terminal where `npm run dev:web` runs
3. Verify all `.env` files are configured correctly
4. Ensure all prerequisites are installed and up to date
