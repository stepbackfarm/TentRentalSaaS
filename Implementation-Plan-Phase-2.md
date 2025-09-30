# Phase 2 Implementation Plan: The Customer-Facing Frontend

This document provides a detailed, step-by-step task list for completing Phase 2 of the Tent Rental SaaS project. Each item is designed to be an actionable task for a developer or AI agent.

## Stage 1: Environment & Project Setup

- [x] **Task 1.1: Install Prerequisites**
  - Verify that Node.js and npm are installed (`node -v` and `npm -v`).

- [x] **Task 1.2: Create React Project**
  - In the project root (`TentRentalSaaS`), execute the following command to create the React frontend project in a `frontend` sub-directory using Vite:
    ```bash
    npm create vite@latest frontend -- --template react
    ```
  - When prompted, select `React` as the framework and `JavaScript` as the variant.

- [x] **Task 1.3: Install Initial Dependencies**
  - Navigate to the new `frontend` directory.
  - Install the initial set of required packages:
    ```bash
    npm install
    npm install axios react-router-dom
    ```
    *   `axios`: For making API calls to the C# backend.
    *   `react-router-dom`: For handling client-side routing.

- [x] **Task 1.4: Project Structure & Cleanup**
  - Remove the default boilerplate content (e.g., `App.css`, `App.jsx` initial content, assets).
  - Create a standard folder structure inside `frontend/src`:
    *   `components/`: For reusable UI components.
    *   `pages/`: For top-level page components (e.g., HomePage, ConfirmationPage).
    *   `services/`: For API communication logic.
    *   `hooks/`: For custom React hooks.

## Stage 2: API Service Integration

- [x] **Task 2.1: Create API Service Module**
  - In the `services/` folder, create a file named `api.js`.
  - Configure an `axios` instance with the base URL of the backend API (e.g., `http://localhost:5000/api`).

- [x] **Task 2.2: Implement Booking Service Functions**
  - In `api.js`, create and export functions to interact with the backend:
    *   `getAvailability(startDate, endDate)`: Makes a `GET` request to `/bookings/availability`.
    *   `createBooking(bookingData)`: Makes a `POST` request to `/bookings`.

## Stage 3: Core Component Development

- [x] **Task 3.1: Develop Availability Calendar Component**
  - In the `components/` folder, create a `Calendar.jsx` component.
  - Use a lightweight calendar library (e.g., `react-day-picker`) or build a simple calendar grid.
  - The component should fetch availability data using the `getAvailability` service function.
  - It should visually disable or style dates that are already booked.
  - It should allow the user to select a single date for their event.

- [x] **Task 3.2: Develop Booking Form Component**
  - In the `components/` folder, create a `BookingForm.jsx` component.
  - Create input fields for `CustomerName` and `CustomerEmail`.
  - The form should take the selected event date as a prop.

## Stage 4: Page Assembly & Routing

- [x] **Task 4.1: Create Home Page**
  - In the `pages/` folder, create `HomePage.jsx`.
  - Assemble the `Calendar` and `BookingForm` components on this page.
  - Manage the state for the selected date and pass it between the components.

- [x] **Task 4.2: Implement Stripe Payment Flow**
  - Integrate Stripe.js and React Stripe Elements (`@stripe/react-stripe-js`, `@stripe/stripe-js`).
  - Add the Stripe `Elements` provider to your app.
  - Add a credit card input field to the `BookingForm` using Stripe's `CardElement`.
  - On form submission, call the `createBooking` service function, passing the booking details and the Stripe payment method ID.

- [x] **Task 4.3: Create Confirmation Page**
  - In the `pages/` folder, create `ConfirmationPage.jsx`.
  - After a successful booking and payment, the user should be redirected to this page.
  - Display the booking details (date, customer name) as a confirmation message.

- [x] **Task 4.4: Set Up Client-Side Routing**
  - In `App.jsx` or a dedicated `Router.jsx` file, configure `react-router-dom` to handle the routes for the `HomePage` and `ConfirmationPage`.

## Stage 5: Styling & Responsive Design

- [x] **Task 5.1: Implement Basic Styling**
  - Use a simple CSS strategy (e.g., CSS Modules, a lightweight framework like Pico.css, or Tailwind CSS) to apply a clean and professional look to the application.

- [x] **Task 5.2: Ensure Mobile Responsiveness**
  - Use CSS media queries to ensure the layout adapts correctly to mobile screen sizes. The calendar and booking form must be usable on a phone.

## Stage 6: Deployment

- [x] **Task 6.1: Configure Vercel**
  - Create a new project on Vercel.
  - Connect the Vercel project to the Git repository.
  - Configure the build settings for a Vite/React project.

- [x] **Task 6.2: Set Environment Variables**
  - In the Vercel project settings, add an environment variable for the backend API URL (`VITE_API_BASE_URL`).

- [x] **Task 6.3: Deploy to Production**
  - Push the code to the `main` branch to trigger the first production deployment.
  - Verify that the live site is working correctly.

## Summary of Deployment Phase

Deployment of the frontend to Vercel and the backend to Google Cloud Run has been successfully completed. Initial 404 errors were resolved by correctly configuring the Vercel project's root directory and implementing a proxy rewrite rule in `frontend/vercel.json` to route API requests to the Google Cloud Run backend. The backend is running at `https://tentrentalsaas-api-975936251815.us-central1.run.app` and the frontend on Vercel is now successfully communicating with it.
