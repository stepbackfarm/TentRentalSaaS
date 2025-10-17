const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();

  const consoleErrors = [];
  page.on('console', msg => {
    if (msg.type() === 'error') {
      console.log(`Playwright Console Error: ${msg.text()}`);
      consoleErrors.push(msg.text());
    }
  });

  try {
    await page.goto('https://tent-rental-hh1bx2kh8-davids-projects-15ffe845.vercel.app/portal/login-request');

    await page.fill('input[type="email"]', 'test@example.com');

    await page.click('button[type="submit"]');

    const successMessage = page.locator('text=If an account with this email exists, a login link has been sent.');
    const errorMessage = page.locator('text=An error occurred. Please try again.');

    try {
        await Promise.race([
            successMessage.waitFor({ state: 'visible', timeout: 10000 }),
            errorMessage.waitFor({ state: 'visible', timeout: 10000 })
        ]);
    } catch (e) {
        console.log("Test inconclusive: Timed out waiting for success or error message.");
    }


    if (await errorMessage.isVisible()) {
        console.log('Test Result: FAILED. The error message "An error occurred. Please try again." was displayed.');
    } else if (await successMessage.isVisible()) {
        console.log('Test Result: PASSED. The success message was displayed.');
    } else {
        console.log('Test Result: INCONCLUSIVE. Neither success nor error message was found.');
    }

    if (consoleErrors.length > 0) {
        console.log(`Found ${consoleErrors.length} console error(s).`);
    } else {
        console.log("No console errors found.");
    }

  } catch (e) {
    console.error('An exception occurred during the Playwright test:', e);
  } finally {
    await browser.close();
  }
})();
