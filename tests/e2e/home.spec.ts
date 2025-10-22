import { test, expect } from '@playwright/test';

test('homepage loads', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveTitle(/Tent/i);
  // Check that main navigation exists
  await expect(page.getByRole('link', { name: /gallery/i })).toBeVisible();
});

test('homepage has FAQ link', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('link', { name: /faq/i })).toBeVisible();
});
