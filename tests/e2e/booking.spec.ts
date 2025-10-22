import { test, expect } from '@playwright/test';

test('checkout page loads', async ({ page }) => {
  await page.goto('/checkout');
  // Basic check that page renders
  await expect(page.getByRole('heading', { name: /checkout/i })).toBeVisible();
});

test.skip('complete booking flow', async ({ page }) => {
  // TODO: Implement full booking flow once Stripe test mode is configured
  // Steps:
  // 1. Navigate to homepage
  // 2. Select tent type and dates
  // 3. Fill delivery address
  // 4. Proceed to checkout
  // 5. Complete Stripe payment (test mode)
  // 6. Verify confirmation page
});
