# Secrets Audit - Complete List
**Date:** 2025-10-23  
**Purpose:** Identify all secrets that should be in Google Cloud Secret Manager

---

## ✅ Backend Secrets (Google Cloud Secret Manager)

### 1. **Database Credentials**
**Used in:** `Program.cs` (lines 12-17)

```bash
# Secret Names in Google Cloud:
DB_HOST           # e.g., "10.1.2.3" or "localhost"
DB_NAME           # e.g., "tentrentals" or "tent-rental-db"
DB_USER           # e.g., "postgres"
DB_PASSWORD       # 🔐 CRITICAL SECRET
```

**Current Code:**
```csharp
var dbHost = builder.Configuration["DB_HOST"];
var dbName = builder.Configuration["DB_NAME"];
var dbUser = builder.Configuration["DB_USER"];
var dbPassword = builder.Configuration["DB_PASSWORD"];
```

**Status:** ✅ Already using environment variables

---

### 2. **Stripe Secret Key**
**Used in:** `Services/StripePaymentService.cs` (line 11)

```bash
# Secret Name in Google Cloud:
STRIPE_SECRET_KEY  # 🔐 CRITICAL SECRET
                   # Test: sk_test_...
                   # Live: sk_live_...
```

**Current Code:**
```csharp
var apiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
```

**Status:** ✅ Already using environment variable

---

### 3. **SMTP Email Credentials**
**Used in:** `Services/EmailService.cs` (lines 26-30)

```bash
# Secret Names in Google Cloud:
SMTP_SERVER    # e.g., "smtp.gmail.com" or "smtp.sendgrid.net"
SMTP_PORT      # e.g., "587" (TLS) or "465" (SSL)
SMTP_FROM      # e.g., "noreply@stepbackfarm.com"
SMTP_USERNAME  # e.g., "apikey" (SendGrid) or email address
SMTP_PASSWORD  # 🔐 CRITICAL SECRET (API key or password)
```

**Current Code:**
```csharp
var server = Environment.GetEnvironmentVariable("SMTP_SERVER");
var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
var from = Environment.GetEnvironmentVariable("SMTP_FROM");
var username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
```

**Status:** ✅ Already using environment variables

---

### 4. **Google Maps API Key**
**Used in:** `Services/GeocodingService.cs` (line 14)

```bash
# Secret Name in Google Cloud:
GOOGLE_MAPS_API_KEY  # 🔐 SECRET
                     # Format: AIzaSy...
```

**Current Code:**
```csharp
var apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY") 
          ?? configuration["GoogleMaps:ApiKey"];
```

**Status:** ✅ Already using environment variable (with fallback to config)

---

### 5. **CORS Allowed Origins**
**Used in:** `Program.cs` (line 107)

```bash
# Environment Variable (not necessarily secret, but environment-specific):
ALLOWED_ORIGINS  # e.g., "https://yourdomain.com,https://www.yourdomain.com"
```

**Current Code:**
```csharp
var allowedOriginsRaw = builder.Configuration["ALLOWED_ORIGINS"];
```

**Status:** ✅ Already using environment variable
**Note:** Not a secret, but environment-specific configuration

---

### 6. **Frontend Base URL**
**Used in:** `Services/AuthService.cs` (line 67)

```bash
# Environment Variable:
FRONTEND_BASE_URL  # e.g., "https://yourdomain.com" or "http://localhost:5173"
```

**Current Code:**
```csharp
var frontendBaseUrl = _configuration["FRONTEND_BASE_URL"];
```

**Status:** ✅ Already using configuration
**Note:** Not a secret, but environment-specific

---

### 7. **Admin Email**
**Used in:** `Services/BookingService.cs` (line 232)

```bash
# Environment Variable:
ADMIN_EMAIL  # e.g., "david@stepbackfarm.com"
```

**Current Code:**
```csharp
var adminEmail = _configuration["ADMIN_EMAIL"] ?? "david@stepbackfarm.com";
```

**Status:** ✅ Already using configuration with fallback
**Note:** Not a secret, just configuration

---

### 8. **JWT Authentication (Google OAuth)**
**Used in:** `Program.cs` (lines 45-48)

```bash
# Environment Variable:
SERVICE_URL  # Your API's public URL for JWT audience validation
             # e.g., "https://api.yourdomain.com"
```

**Current Code:**
```csharp
options.Authority = "https://accounts.google.com";
options.Audience = Environment.GetEnvironmentVariable("SERVICE_URL");
```

**Status:** ✅ Already using environment variable
**Note:** Not a secret, but must match your OAuth configuration

---

## 🌐 Frontend Secrets (Build-time Environment Variables)

### 1. **Stripe Publishable Key**
**Used in:** `frontend/src/pages/CheckoutPage.jsx` (line 7)

```bash
# Environment Variable (Vite):
VITE_STRIPE_PUBLISHABLE_KEY  # Format: pk_test_... or pk_live_...
```

**Status:** ✅ In `.env.example`
**Note:** ⚠️ This is SAFE to expose in frontend code (it's meant to be public)
**Value in .env.example:** `pk_test_51SDGz3PCTlw7D5jlwFuPpMOqtzuZ39cWhwlYKuuTiJBmALislxA0PCfEoxuhnpQ9D5TK5PAl9cdVUEALoERjfcQY00dYuqJZlZ`

---

### 2. **API Base URL**
**Used in:** `frontend/src/services/api.js` (line 3) and `PortalLoginPage.jsx` (line 20)

```bash
# Environment Variable (Vite):
VITE_API_BASE_URL  # e.g., "https://api.yourdomain.com/api" or "http://localhost:5000/api"
```

**Status:** ✅ In `.env.example`
**Note:** Not a secret, just configuration

---

## 📋 Complete Secrets Checklist

### 🔐 CRITICAL SECRETS (Must be in Secret Manager):

- [ ] **STRIPE_SECRET_KEY** - Payment processing
- [ ] **SMTP_PASSWORD** - Email sending
- [ ] **DB_PASSWORD** - Database access
- [ ] **GOOGLE_MAPS_API_KEY** - Geocoding service

### 🔧 Configuration Values (Environment-specific, not secrets):

- [ ] **DB_HOST** - Database host
- [ ] **DB_NAME** - Database name
- [ ] **DB_USER** - Database username (could be secret depending on setup)
- [ ] **SMTP_SERVER** - Email server
- [ ] **SMTP_PORT** - Email port
- [ ] **SMTP_FROM** - From email address
- [ ] **SMTP_USERNAME** - Email username (could be secret)
- [ ] **ALLOWED_ORIGINS** - CORS configuration
- [ ] **FRONTEND_BASE_URL** - Frontend URL
- [ ] **ADMIN_EMAIL** - Admin notification email
- [ ] **SERVICE_URL** - API public URL
- [ ] **PORT** - Application port (Cloud Run provides this)

### 🌐 Frontend Build Variables (Safe to expose):

- [ ] **VITE_STRIPE_PUBLISHABLE_KEY** - Stripe public key (meant to be public)
- [ ] **VITE_API_BASE_URL** - API endpoint URL

---

## 🚀 Implementation Status

### ✅ **What's Already Done:**

1. All backend services use `Environment.GetEnvironmentVariable()` ✅
2. No hardcoded secrets in production code paths ✅
3. Fallbacks exist for development (appsettings.Development.json) ✅
4. Frontend uses Vite environment variables ✅

### ⚠️ **What Needs Attention:**

1. **appsettings.json** still has database password "password"
   - This is only used in development (Port 5432)
   - Production uses environment variables
   - **Action:** Already safe, but could remove to be extra cautious

2. **appsettings.Development.json** has placeholder values
   - `YOUR_API_KEY_HERE`, `your-password`, etc.
   - **Action:** Already safe (just placeholders)

3. **Frontend .env.example** has Stripe test key
   - This is the PUBLISHABLE key (safe to expose)
   - **Action:** No action needed, but ensure `.env` is in `.gitignore` ✅

---

## 📝 Google Cloud Secret Manager Setup

### Create Secrets in Google Cloud:

```bash
# Set your project
gcloud config set project YOUR_PROJECT_ID

# Create backend secrets
echo -n "your_actual_stripe_secret_key" | gcloud secrets create stripe-secret-key --data-file=-
echo -n "your_smtp_password" | gcloud secrets create smtp-password --data-file=-
echo -n "your_db_password" | gcloud secrets create db-password --data-file=-
echo -n "your_google_maps_key" | gcloud secrets create google-maps-api-key --data-file=-

# Grant access to Cloud Run service account
gcloud secrets add-iam-policy-binding stripe-secret-key \
    --member="serviceAccount:YOUR_SERVICE_ACCOUNT@YOUR_PROJECT.iam.gserviceaccount.com" \
    --role="roles/secretmanager.secretAccessor"

gcloud secrets add-iam-policy-binding smtp-password \
    --member="serviceAccount:YOUR_SERVICE_ACCOUNT@YOUR_PROJECT.iam.gserviceaccount.com" \
    --role="roles/secretmanager.secretAccessor"

gcloud secrets add-iam-policy-binding db-password \
    --member="serviceAccount:YOUR_SERVICE_ACCOUNT@YOUR_PROJECT.iam.gserviceaccount.com" \
    --role="roles/secretmanager.secretAccessor"

gcloud secrets add-iam-policy-binding google-maps-api-key \
    --member="serviceAccount:YOUR_SERVICE_ACCOUNT@YOUR_PROJECT.iam.gserviceaccount.com" \
    --role="roles/secretmanager.secretAccessor"
```

### Set Environment Variables in Cloud Run:

```bash
gcloud run services update YOUR_SERVICE_NAME \
  --set-env-vars="DB_HOST=your-db-host" \
  --set-env-vars="DB_NAME=tentrentals" \
  --set-env-vars="DB_USER=postgres" \
  --set-env-vars="SMTP_SERVER=smtp.gmail.com" \
  --set-env-vars="SMTP_PORT=587" \
  --set-env-vars="SMTP_FROM=noreply@stepbackfarm.com" \
  --set-env-vars="SMTP_USERNAME=your-username" \
  --set-env-vars="ALLOWED_ORIGINS=https://yourdomain.com" \
  --set-env-vars="FRONTEND_BASE_URL=https://yourdomain.com" \
  --set-env-vars="ADMIN_EMAIL=david@stepbackfarm.com" \
  --set-env-vars="SERVICE_URL=https://api.yourdomain.com" \
  --update-secrets="STRIPE_SECRET_KEY=stripe-secret-key:latest" \
  --update-secrets="SMTP_PASSWORD=smtp-password:latest" \
  --update-secrets="DB_PASSWORD=db-password:latest" \
  --update-secrets="GOOGLE_MAPS_API_KEY=google-maps-api-key:latest"
```

---

## ✅ Summary

### Total Secrets Identified: **4 Critical**
1. ✅ STRIPE_SECRET_KEY
2. ✅ SMTP_PASSWORD
3. ✅ DB_PASSWORD
4. ✅ GOOGLE_MAPS_API_KEY

### Total Config Variables: **10**
All are already using environment variables or configuration properly!

### **Your Code is Already Well-Structured! 🎉**

Your application is already using environment variables correctly. You just need to:
1. Create the secrets in Google Cloud Secret Manager
2. Configure Cloud Run to use them
3. Optionally: Remove placeholder values from appsettings files (not critical)

---

**No additional secrets found!** Your implementation is solid. ✅
