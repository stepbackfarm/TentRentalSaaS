-- Analyze corrupted bookings and calculate required refunds
-- Run this BEFORE applying the migration to identify impacted bookings

-- 1. Identify bookings with suspicious delivery fees
SELECT 
    "Id",
    "CustomerId",
    "EventDate",
    "CreatedDate",
    "DeliveryFee",
    "RentalFee",
    "SecurityDeposit", 
    "TotalPrice",
    "StripePaymentIntentId",
    -- Calculate what the correct delivery fee should be (assuming corrupted value is meters)
    GREATEST(
        ROUND(("DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    ) as "CorrectedDeliveryFee",
    -- Calculate what the correct total should be
    "RentalFee" + "SecurityDeposit" + GREATEST(
        ROUND(("DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    ) as "CorrectedTotalPrice",
    -- Calculate the overcharge amount
    "TotalPrice" - ("RentalFee" + "SecurityDeposit" + GREATEST(
        ROUND(("DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    )) as "OverchargeAmount"
FROM "Bookings"
WHERE "DeliveryFee" > 1000
ORDER BY "CreatedDate" DESC;

-- 2. Summary of corruption impact
SELECT 
    COUNT(*) as "AffectedBookings",
    SUM("TotalPrice") as "TotalChargedAmount",
    SUM("RentalFee" + "SecurityDeposit" + GREATEST(
        ROUND(("DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    )) as "CorrectTotalAmount",
    SUM("TotalPrice" - ("RentalFee" + "SecurityDeposit" + GREATEST(
        ROUND(("DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    ))) as "TotalOverchargeAmount"
FROM "Bookings"
WHERE "DeliveryFee" > 1000;

-- 3. Get customer details for refund processing
SELECT DISTINCT
    c."CustomerId",
    c."FirstName",
    c."LastName", 
    c."Email",
    COUNT(b."Id") as "AffectedBookings",
    SUM(b."TotalPrice" - (b."RentalFee" + b."SecurityDeposit" + GREATEST(
        ROUND((b."DeliveryFee" / 1609.344) * 2.00, 2),
        5.00
    ))) as "TotalRefundDue"
FROM "Customers" c
JOIN "Bookings" b ON c."CustomerId" = b."CustomerId"
WHERE b."DeliveryFee" > 1000
GROUP BY c."CustomerId", c."FirstName", c."LastName", c."Email"
ORDER BY "TotalRefundDue" DESC;

-- 4. Check for any bookings with reasonable delivery fees (should remain unchanged)
SELECT 
    COUNT(*) as "UnaffectedBookings",
    AVG("DeliveryFee") as "AverageDeliveryFee",
    MIN("DeliveryFee") as "MinDeliveryFee",
    MAX("DeliveryFee") as "MaxDeliveryFee"
FROM "Bookings"
WHERE "DeliveryFee" <= 1000;