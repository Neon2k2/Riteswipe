# RiteSwipe Deployment Guide

## Prerequisites

### Development Environment
1. **Required Tools**
   - .NET SDK 8.0
   - Node.js 20.x
   - Docker Desktop
   - Azure CLI
   - Terraform 1.5+

2. **Azure Subscription**
   - Active subscription
   - Contributor access
   - Service principal credentials

3. **Source Control**
   - GitHub account
   - Repository access
   - Branch permissions

## Infrastructure Setup

### 1. Initialize Terraform
```bash
# Login to Azure
az login

# Create storage for Terraform state
az group create --name riteswipe-tfstate --location eastus
az storage account create --name riteswipetfstate --resource-group riteswipe-tfstate --location eastus --sku Standard_LRS
az storage container create --name tfstate --account-name riteswipetfstate

# Initialize Terraform
cd infrastructure
terraform init
```

### 2. Configure Variables
```bash
# Copy example vars file
cp terraform.tfvars.example terraform.tfvars

# Edit variables
vim terraform.tfvars
```

### 3. Deploy Infrastructure
```bash
# Plan deployment
terraform plan -out=tfplan

# Apply changes
terraform apply tfplan
```

## Application Deployment

### 1. Backend API

#### Build and Test
```bash
# Restore dependencies
dotnet restore

# Run tests
dotnet test

# Build release
dotnet publish -c Release
```

#### Container Deployment
```bash
# Build container
docker build -t riteswipe-api .

# Push to registry
docker push your-registry/riteswipe-api:latest
```

### 2. Frontend Web App

#### Build and Test
```bash
# Install dependencies
npm install

# Run tests
npm test

# Build production
npm run build
```

#### Static Site Deployment
```bash
# Deploy to Azure Static Web Apps
az staticwebapp deploy --source-location "build"
```

## Configuration

### 1. Environment Variables
```bash
# API Configuration
export ConnectionStrings__DefaultConnection="Server=..."
export Redis__ConnectionString="..."
export JwtSettings__Secret="..."

# Frontend Configuration
export REACT_APP_API_URL="..."
export REACT_APP_SIGNALR_URL="..."
```

### 2. SSL Certificates
```bash
# Generate certificate
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout key.pem -out cert.pem

# Upload to Key Vault
az keyvault certificate import --vault-name kv-riteswipe --name api-cert --file cert.pem
```

## Monitoring Setup

### 1. Application Insights
```bash
# Get instrumentation key
export APPINSIGHTS_KEY=$(az monitor app-insights component show --query instrumentationKey -o tsv)

# Configure application
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY"
```

### 2. Log Analytics
```bash
# Create workspace
az monitor log-analytics workspace create --resource-group rg-riteswipe --workspace-name log-riteswipe

# Enable solutions
az monitor log-analytics solution create --resource-group rg-riteswipe --solution-type SecurityInsights
```

## Security Configuration

### 1. Network Security
```bash
# Configure firewall rules
az network firewall rule create --collection-name "AllowWeb" --destination-ports 80 443 --protocols "TCP" --source-addresses "*" --destination-addresses "*" --translated-port 80 --action "Allow" --priority 100 --rule-name "AllowWeb"
```

### 2. Key Vault Access
```bash
# Grant access to application
az keyvault set-policy --name kv-riteswipe --object-id $APP_IDENTITY_ID --secret-permissions get list
```

## Database Management

### 1. Migrations
```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

### 2. Seed Data
```bash
# Run seed script
dotnet run --project src/RiteSwipe.Data/Seed/SeedData.csproj
```

## Maintenance Tasks

### 1. Backup Verification
```bash
# List backups
az backup item list --resource-group rg-riteswipe --vault-name rsv-riteswipe

# Test restore
az backup restore restore-disks --resource-group rg-riteswipe --vault-name rsv-riteswipe
```

### 2. Performance Monitoring
```bash
# View metrics
az monitor metrics list --resource $APP_ID --metric "CpuPercentage"

# Check logs
az monitor log-analytics query --workspace $WORKSPACE_ID --query "AppTraces | limit 10"
```

## Troubleshooting

### Common Issues

1. **Database Connection**
   - Check connection string
   - Verify firewall rules
   - Test network connectivity

2. **Authentication**
   - Validate JWT token
   - Check Azure AD settings
   - Verify client credentials

3. **Performance**
   - Monitor CPU usage
   - Check memory consumption
   - Analyze slow queries

### Support Contacts

- **DevOps Team**: devops@riteswipe.com
- **Security Team**: security@riteswipe.com
- **Database Team**: dba@riteswipe.com

## Rollback Procedures

### 1. Application Rollback
```bash
# Revert to previous version
az webapp deployment slot swap --name app-riteswipe --resource-group rg-riteswipe --slot staging --target-slot production
```

### 2. Database Rollback
```bash
# Restore database
az sql db restore --resource-group rg-riteswipe --server sql-riteswipe --name sqldb-riteswipe --time "2025-04-26T00:00:00Z"
```

## Compliance Checks

### 1. Security Scan
```bash
# Run security assessment
az security assessment create --assessment-type "SqlVulnerability" --resource-group rg-riteswipe

# View results
az security assessment list --resource-group rg-riteswipe
```

### 2. Compliance Report
```bash
# Generate report
az security task create --name "ComplianceReport" --resource-group rg-riteswipe

# Export results
az security task export --name "ComplianceReport" --resource-group rg-riteswipe
```
