# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 1. Codebase Overview

This repository contains a full-stack application for tent rentals, consisting of an ASP.NET Core Web API backend and a React frontend.

### Backend (`backend/`)
- Developed with **ASP.NET Core 8 Web API**.
- Uses **Entity Framework Core** with **PostgreSQL** for data persistence.
- Key logical divisions:
    - `Models/`: Database entity definitions (e.g., `Booking`, `Customer`).
    - `DTOs/`: Data Transfer Objects for API request/response bodies.
    - `Services/`: Contains business logic and orchestrates data access (e.g., `BookingService`). This layer is injected via dependency injection into controllers.
    - `Controllers/`: Exposes RESTful API endpoints.
    - `Migrations/`: Entity Framework Core database migration scripts.

### Frontend (`frontend/`)
- Developed with **React** and **Vite**.
- Uses `axios` for API communication with the backend.
- Employs `react-router-dom` for client-side routing.
- Current project structure includes:
    - `src/components/`: Reusable UI components.
    - `src/pages/`: Top-level page components and their composition.
    - `src/services/`: API client logic (e.g., `api.js` for backend calls).
    - `src/hooks/`: Custom React hooks for shared logic.
- Integrates **Stripe.js** for payment processing.

## 2. Commonly Used Commands

### Backend
- **Build**: `dotnet build backend/TentRentalSaaS.Api.csproj`
- **Run**: `dotnet run --project backend/TentRentalSaaS.Api.csproj`
- **Run Tests**: `dotnet test backend/tests/` (assuming `backend/tests/` is the test project directory)
- **Add Migration**: `dotnet ef migrations add [MigrationName] --project backend/TentRentalSaaS.Api.csproj --startup-project backend/TentRentalSaaS.Api.csproj`
- **Update Database**: `dotnet ef database update --project backend/TentRentalSaaS.Api.csproj --startup-project backend/TentRentalSaaS.Api.csproj`

### Frontend
- **Install Dependencies**: `npm install --prefix frontend`
- **Run Development Server**: `npm run dev --prefix frontend`
- **Build for Production**: `npm run build --prefix frontend`
- **Run Tests**: `npm test --prefix frontend` (if configured)

### Project-wide
- **Run both Backend and Frontend (concurrently)**: It is recommended to run the backend and frontend development servers in separate terminal sessions.
    1.  In one terminal: `dotnet run --project backend/TentRentalSaaS.Api.csproj`
    2.  In another terminal: `npm run dev --prefix frontend`

## 3. High-Level Architecture and Structure

The application follows a **client-server architecture**.

- The **Frontend (React)** serves as the user interface, handling all client-side interactions, form submissions, and data display. It communicates exclusively with the backend API.
- The **Backend (ASP.NET Core Web API)** provides a RESTful API for managing bookings, customer data, and payments. It encapsulates all business logic and interacts with the PostgreSQL database via Entity Framework Core.
- **Data Flow**: Frontend sends requests to Backend API → Backend processes logic, interacts with PostgreSQL via EF Core → Backend returns data/status to Frontend.
- **Third-Party Integrations**: Stripe is integrated into both the frontend (for collecting payment details securely) and potentially the backend (for processing charges and managing subscriptions).
- **Authentication**: The system currently outlines a passwordless login approach for the customer portal, involving one-time links sent via email.
