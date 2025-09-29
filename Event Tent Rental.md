# Event Tent Rental

This document outlines the plan for renting out the large event tent.

## Service Definition

*   **The Tent:** 20x40 foot tent.
*   **The Service:** A full-service rental package that includes delivery, professional setup, and tear-down of the tent.

## Self-Service SaaS Portal

To streamline the customer experience and minimize administrative overhead, we will develop a small SaaS portal for the tent rental business. This portal will allow customers to manage the entire rental process themselves.

### Customer Journey

1.  **Discovery:** Customer finds the tent rental service through online search, social media, or local advertising.
2.  **Pricing & Availability:** Customer visits the website and uses a simple calculator to get a price quote based on their event date and location. They can see the tent's availability on a calendar.
3.  **Booking & Payment:** Customer selects their desired date, provides event details, and pays the rental fee and security deposit online.
4.  **Review & Sign Agreement:** Before the booking is finalized, the customer is presented with the rental agreement. They must check a box to electronically sign and agree to the terms and conditions.
5.  **Pre-Event:** Customer receives automated email reminders and confirmations. They can log in to the portal to view their booking details, and if needed, request a reschedule or cancellation.
5.  **Setup & Teardown:** The setup and teardown are automatically scheduled on our end based on the booking information. The customer is notified when the setup is complete.
6.  **Post-Event:** After the event, the customer receives a thank you email and a request for a review. The security deposit is automatically refunded after the tent is inspected.

### Portal Features

*   **Pricing Calculator:** A simple form where customers can enter their event date and location to get an instant price quote.
*   **Availability Calendar:** A real-time calendar that shows when the tent is available.
*   **Online Booking Engine:** A secure system for customers to book the tent and pay online.
*   **Customer Portal:** A secure, passwordless area where customers can manage their bookings. To log in, customers will enter their email address and receive a one-time use link that is valid for 12 hours. This link will grant them access to:
    *   View their booking details
    *   Request to reschedule or cancel their booking
    *   Contact support
*   **Automated Notifications:** Email and/or SMS notifications for booking confirmations, reminders, and other important updates.
*   **Digital Agreement & E-Signature:** The portal will generate a personalized rental agreement for each booking and present it to the customer for their review. The customer's agreement will be captured with a checkbox and a timestamp, and the signed agreement will be stored for both parties to access.
*   **FAQ/Support Section:** A searchable knowledge base with answers to common questions.

## Financials

*   **Rental Fee:** $400 for a weekend rental. This includes professional setup and tear-down.
*   **Refundable Security Deposit:** $100. This will be collected at the time of booking and fully refunded within 48 hours of the event, pending an inspection of the tent to ensure it is returned in the same condition it was delivered.
*   **Delivery Fee:** $2.00 per mile from Darlington, Indiana. This fee will be calculated based on the one-way trip mileage.  Total mileage driven is 4x the one-way mileage so the actual payment is $0.50 per mile but it's easier for customers to understand to quote in one-way trip mileage.
*   **Costs:** (e.g., maintenance, cleaning, insurance).

## Key Considerations

*   **Insurance & Liability:** A specific policy or rider will be needed to cover potential damage to the tent and any liability associated with its use.

## Technical Plan

The detailed technical plan for the development of the Self-Service SaaS Portal can be found in the [[TentRentalSaaS-Project-Plan]] document.
