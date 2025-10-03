### Google Cloud Database Setup Guide

This checklist will walk you through creating a new database in Google Cloud, configuring it, and connecting it to your deployed backend service.

**1. Choose and Create the Database Instance**
*   [ ] **Navigate to Cloud SQL:** In the Google Cloud Console, go to the "SQL" section.
*   [ ] **Create Instance:** Click "Create Instance".
*   [ ] **Choose a Database Engine:** Select PostgreSQL, as your project is configured for it.
*   [ ] **Set Instance ID and Root Password:** Provide a unique name for your database instance and set a secure password for the root user. **Save this password securely.**
*   [ ] **Choose Region and Zone:** Select the same region where your backend is deployed to minimize latency and enable private IP connectivity.

**2. Configure Network Connectivity**

Choose one of the following two options for connecting your backend to the database.

#### Option A: Private IP (Recommended & More Secure)
*Use this method if your backend is running in the same Google Cloud project and VPC network.*

*   [ ] **Enable Private IP:** Under the "Connectivity" section during instance creation, select **Private IP**.
*   [ ] **Select VPC Network:** Choose the VPC network that your backend service is using.
*   [ ] **Configure Private Service Access:** If prompted, you may need to enable the Service Networking API and configure a private service access connection. The Cloud Console will guide you through this one-time setup.
*   [ ] **Note the Private IP:** Once the instance is created, find and copy the **Private IP address** from the instance's overview page.

#### Option B: Public IP (Simpler, Less Secure)
*Use this method for initial testing or if your backend is hosted outside of Google Cloud.*

*   [ ] **Enable Public IP:** Under the "Connectivity" section, select **Public IP**.
*   [ ] **Authorize Your Backend's IP:** For security, add the IP address of your deployed backend service to the "Authorized networks" list. This acts as a firewall. You can add your local IP for testing purposes.
*   [ ] **Note the Public IP:** Once the instance is created, find and copy the **Public IP address** from the instance's overview page.

**3. Create the Database**
*   [ ] **Create a Database:** Once the instance is running, navigate to the "Databases" tab for that instance and click "Create database". Give it the name `tent-rental-db` to match your project's current configuration.

**4. Connect Your Backend**
*   [ ] **Construct the Connection String:** Use the IP address you noted in Step 2 (either private or public) to create your connection string.
    `Server=<YOUR_INSTANCE_IP_ADDRESS>;Database=tent-rental-db;User Id=<YOUR_ROOT_USER>;Password=<YOUR_ROOT_PASSWORD>;`
*   [ ] **Update Backend Configuration:** Update the `DefaultConnection` connection string. **It is highly recommended to use environment variables or a secret manager for your production deployment instead of `appsettings.json`.**
*   [ ] **Redeploy Backend:** Redeploy your backend application with the new connection string.

**5. Run Database Migrations**
*   [ ] **Apply Entity Framework Migrations:** Once the backend is redeployed, it needs to apply the database schema. How you do this depends on your deployment setup:
    *   **Automatic Migrations (Recommended for Cloud):** Configure your `Program.cs` to automatically apply migrations on startup.
    *   **Manual Migrations:** SSH into the machine running your backend and run `dotnet ef database update`.

After completing these steps, your database should be set up and connected to your backend.