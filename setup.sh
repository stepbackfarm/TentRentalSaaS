#!/usr/bin/env bash
set -euo pipefail

# Configurable paths (adjust if your structure differs)
BACKEND_DIR="backend"
FRONTEND_DIR="frontend"
API_PROJECT="${BACKEND_DIR}/TentRentalSaaS.Api.csproj"
API_PORT="${API_PORT:-5000}"

echo "======================================"
echo "TentRental SaaS - Development Setup"
echo "======================================"
echo

echo "Checking prerequisites..."
need() { 
  command -v "$1" >/dev/null 2>&1 || { 
    echo "❌ Missing prerequisite: $1"
    echo "   Please install $1 and try again."
    exit 1
  }
  echo "✓ $1 found"
}

need docker
need node
need npm
need dotnet

if ! docker-compose version >/dev/null 2>&1; then
  echo "❌ docker-compose not found."
  echo "   Please install docker-compose: sudo apt install docker-compose"
  exit 1
fi
echo "✓ docker-compose found"
echo

echo "Creating .env files from templates..."
copy_env() { 
  local src=$1 dst=$2
  [ -f "$src" ] || { echo "⚠️  Template $src not found, skipping"; return 0; }
  if [ ! -f "$dst" ]; then 
    cp "$src" "$dst"
    echo "✓ Created $dst"
  else 
    echo "→ $dst already exists, skipping"
  fi
}

copy_env ".env.example" ".env"
copy_env "${BACKEND_DIR}/.env.example" "${BACKEND_DIR}/.env"
copy_env "${FRONTEND_DIR}/.env.example" "${FRONTEND_DIR}/.env"
echo

echo "Installing root dev dependencies (Playwright, concurrently, cross-env)..."
if [ ! -f package.json ]; then 
  echo '{ "name":"tentrental-root","private":true }' > package.json
fi
npm i -D @playwright/test concurrently cross-env
echo

echo "Installing Playwright browsers..."
npx playwright install --with-deps
echo

echo "Installing frontend dependencies..."
pushd "${FRONTEND_DIR}" >/dev/null
if [ -f package-lock.json ]; then 
  npm ci
else 
  npm i
fi
popd >/dev/null
echo

echo "Starting database..."
docker-compose up -d db
echo

echo -n "Waiting for database to be healthy"
for i in {1..60}; do
  status=$(docker ps --filter "name=tentrental-db" --format "{{.Status}}" 2>/dev/null || true)
  if echo "$status" | grep -qi "(healthy)"; then 
    echo
    echo "✓ Database is healthy"
    break
  fi
  echo -n "."
  sleep 2
  if [ $i -eq 60 ]; then
    echo
    echo "❌ Database did not become healthy in time"
    echo "   Check logs with: docker compose logs db"
    exit 1
  fi
done
echo

echo "Checking for dotnet-ef tool..."
if ! dotnet tool list -g | grep -q 'dotnet-ef'; then
  echo "Installing dotnet-ef globally..."
  dotnet tool install -g dotnet-ef
  export PATH="$PATH:$HOME/.dotnet/tools"
fi
echo "✓ dotnet-ef is available"
echo

echo "Applying EF Core migrations..."
export ASPNETCORE_ENVIRONMENT=Development
if [ -f "${API_PROJECT}" ]; then
  dotnet ef database update --project "${API_PROJECT}"
  echo "✓ Migrations applied"
else
  echo "❌ Could not find ${API_PROJECT}"
  echo "   Please update BACKEND_DIR and API_PROJECT in setup.sh"
  exit 1
fi
echo

echo "Smoke testing backend health endpoint..."
echo "Starting backend temporarily..."
set +e  # Temporarily disable exit on error for background process
(ASPNETCORE_ENVIRONMENT=Development dotnet run --project "${API_PROJECT}" >/tmp/api-setup.log 2>&1 &)
API_PID=$!
set -e  # Re-enable exit on error
sleep 8

if curl -sf "http://localhost:${API_PORT}/health" >/dev/null 2>&1; then
  echo "✓ Backend health check passed"
  kill $API_PID 2>/dev/null || true
  wait $API_PID 2>/dev/null || true
else
  echo "❌ Backend health check failed"
  echo "   See /tmp/api-setup.log for details"
  kill $API_PID 2>/dev/null || true
  exit 1
fi
echo

cat <<'EONEXT'
======================================
✅ Setup complete!
======================================

Next steps:

📦 Full Docker stack (recommended for contractors):
   npm run docker:full

🚀 Host-based development (recommended for AI agents):
   npm run dev:all
   
   Then visit: http://localhost:5173

🧪 Run E2E tests:
   npm run test:e2e              # Headless
   npm run test:e2e:ui            # Interactive UI mode
   npm run test:e2e:headed        # Watch tests run in browser

🔧 Individual services:
   npm run dev:db                 # Database only
   npm run dev:api                # API only (requires db)
   npm run dev:web                # Frontend only

📝 Tips:
   - Playwright MCP: Set PW_NO_SERVER=1 to reuse running servers
   - Update .env files with your Stripe and Google API keys
   - Database data persists in Docker volume 'pgdata'

EONEXT
