# Phase 1 Implementation Plan: The Core Booking Engine

This document provides a detailed, step-by-step task list for completing Phase 1 of the Tent Rental SaaS project. Each item is designed to be an actionable task for a developer or AI agent.

## Stage 1: Environment & Project Setup

- [ ] **Task 1.1: Install Prerequisites**
  - Verify that the .NET 8 SDK is installed (`dotnet --version`).
  - Verify that Git is installed (`git --version`).

- [ ] **Task 1.2: Create Backend Project**
  - Execute the following command to create the ASP.NET Core Web API project in a `backend` sub-directory:
    ```bash
    dotnet new webapi -n TentRentalSaaS.Api -o "./backend"
    ```

- [ ] **Task 1.3: Initialize Git Repository**
  - Navigate to the project root (`TentRentalSaaS`).
  - Initialize a new Git repository (`git init`).
  - Create a `.gitignore` file appropriate for a .NET project.
  - Perform the initial commit of the project files.

## Stage 2: Database & Data Models

- [ ] **Task 2.1: Add Entity Framework Core Packages**
  - Navigate to the `backend` directory.
  - Add the necessary NuGet packages for Entity Framework Core and the PostgreSQL provider:
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    ```

- [ ] **Task 2.2: Define C# Entity Models**
  - In the `backend` project, create a `Models` folder.
  - Create three files in the `Models` folder: `Booking.cs`, `Customer.cs`, and `LoginToken.cs`.
  - Implement the C# classes in these files exactly as defined in the `TentRentalSaaS-Project-Plan.md`.

- [ ] **Task 2.3: Create Database Context**
  - In the `backend` project, create a new file for the database context (e.g., `ApiDbContext.cs`).
  - Implement the `ApiDbContext` class, inheriting from `DbContext`.
  - Add `DbSet` properties for `Bookings`, `Customers`, and `LoginTokens`.

- [ ] **Task 2.4: Configure Database Connection**
  - In `appsettings.json`, add a connection string for the PostgreSQL database. Use a placeholder for the password.
  - In `Program.cs`, configure the `ApiDbContext` as a service and connect it to the PostgreSQL provider using the connection string.

- [ ] **Task 2.5: Generate Initial Migration**
  - From the `backend` directory, run the command to create the first database migration. This will generate code that creates the database schema based on your C# models.
    ```bash
    dotnet ef migrations add InitialCreate
    ```

- [ ] **Task 2.6: Apply Migration to Database**
  - Run the command to apply the migration, which will create the tables in your PostgreSQL database.
    ```bash
    dotnet ef database update
    ```

## Stage 3: API Endpoint Development (Controllers)

- [ ] **Task 3.1: Create Bookings Controller**
  - In the `Controllers` folder, create a new file named `BookingsController.cs`.
  - Create an empty API controller class inheriting from `ControllerBase` with the `[ApiController]` and `[Route("api/[controller]")]` attributes.

- [ ] **Task 3.2: Implement `GET /api/bookings/availability` Endpoint**
  - Create a method in `BookingsController` to handle `GET` requests to `/api/bookings/availability`.
  - This endpoint should accept `startDate` and `endDate` query parameters.
  - It will call a service to get the booked dates and return a list of unavailable dates in the given range.

- [ ] **Task 3.3: Implement `POST /api/bookings` Endpoint**
  - Create a method in `BookingsController` to handle `POST` requests to `/api/bookings`.
  - This endpoint will accept a Data Transfer Object (DTO) containing the booking request details (customer info, event date).
  - It will call a service to process the booking and payment.
  - It should return the created booking object on success.

## Stage 4: Business Logic (Services)

- [ ] **Task 4.1: Create Booking Service Interface & Class**
  - Create a `Services` folder.
  - Define an `IBookingService` interface with methods for `GetAvailability` and `CreateBooking`.
  - Create a `BookingService` class that implements the interface.
  - Register the service in `Program.cs` for dependency injection.

- [ ] **Task 4.2: Implement Availability Logic**
  - In `BookingService`, implement the `GetAvailability` method.
  - The method should query the database via the `ApiDbContext` to find all confirmed bookings within the requested date range.
  - It should return a list of dates that are already booked.

- [ ] **Task 4.3: Implement Booking Creation Logic**
  - In `BookingService`, implement the `CreateBooking` method.
  - This method should perform the following steps in a transaction:
    1.  Check for booking conflicts on the requested date.
    2.  Calculate the `DeliveryFee` (create a placeholder for now).
    3.  (Placeholder) Call the Stripe service to process the payment.
    4.  Create a new `Booking` entity and save it to the database via `ApiDbContext`.
    5.  Return the newly created `Booking`.

## Stage 5: Testing

- [ ] **Task 5.1: Set Up Test Project**
  - Create a new xUnit test project in the solution.
  - Reference the main `TentRentalSaaS.Api` project.

- [ ] **Task 5.2: Write Unit Tests for Booking Service**
  - Write unit tests for the `BookingService`.
  - Mock the `ApiDbContext` and any external dependencies.
  - Test the availability logic: ensure it correctly identifies booked dates.
  - Test the booking creation logic: ensure it prevents double booking.
