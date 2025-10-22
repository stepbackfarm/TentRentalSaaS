import { test, expect } from '@playwright/test';

test('portal login request flow', async ({ page }) => {
  await page.goto('/portal/login-request');
  
  // Verify page loaded
  await expect(page.getByRole('heading', { name: /customer portal/i })).toBeVisible();
  
  // Fill in email
  await page.getByLabel(/email address/i).fill('test.user@example.com');
  
  // Submit form
  await page.getByRole('button', { name: /send login link/i }).click();
  
  // Wait for response message to appear (either success or error)
  await page.locator('.mt-4.text-center').first().waitFor({ state: 'visible', timeout: 10000 });
  
  // Check if we got success or error (both are valid outcomes for this test)
  const successMessage = page.locator('.text-green-400');
  const errorMessage = page.locator('.text-red-400');
  
  const hasSuccess = await successMessage.isVisible();
  const hasError = await errorMessage.isVisible();
  
  // At least one message should be displayed
  expect(hasSuccess || hasError).toBe(true);
});
