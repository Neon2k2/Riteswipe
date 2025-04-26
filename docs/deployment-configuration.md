# RiteSwipe Deployment and Configuration Guide

## Environment Setup

### 1. Development Environment

#### Required Software
```bash
# Install .NET SDK 8.0
winget install Microsoft.DotNet.SDK.8

# Install Node.js 20.x
winget install OpenJS.NodeJS.LTS

# Install Azure CLI
winget install Microsoft.AzureCLI

# Install Terraform
winget install HashiCorp.Terraform
```

#### IDE Setup
```bash
# Visual Studio Code Extensions
code --install-extension ms-dotnettools.csharp
code --install-extension ms-azuretools.vscode-azureterraform
code --install-extension dbaeumer.vscode-eslint
code --install-extension esbenp.prettier-vscode
```

### 2. Azure Configuration

#### Service Principal Setup
```bash
# Login to Azure
az login

# Create Service Principal
az ad sp create-for-rbac --name "riteswipe-sp" --role contributor

# Note down the output:
# {
#   "appId": "<client_id>",
#   "displayName": "riteswipe-sp",
#   "password": "<client_secret>",
#   "tenant": "<tenant_id>"
# }
```

#### Environment Variables
```bash
# Azure credentials
export ARM_CLIENT_ID="<client_id>"
export ARM_CLIENT_SECRET="<client_secret>"
export ARM_SUBSCRIPTION_ID="<subscription_id>"
export ARM_TENANT_ID="<tenant_id>"

# Application settings
export ASPNETCORE_ENVIRONMENT="Development"
export AZURE_WEBAPP_NAME="app-riteswipe-api"
export DOCKER_REGISTRY="your-registry"
```

## Infrastructure Deployment

### 1. Initialize Terraform

```bash
# Create backend storage
az group create --name rg-tfstate --location eastus
az storage account create --name riteswipetfstate --resource-group rg-tfstate --sku Standard_LRS
az storage container create --name tfstate --account-name riteswipetfstate

# Initialize Terraform
cd infrastructure
terraform init \
  -backend-config="storage_account_name=riteswipetfstate" \
  -backend-config="container_name=tfstate" \
  -backend-config="key=terraform.tfstate"
```

### 2. Configure Variables

#### terraform.tfvars
```hcl
environment = "production"
location = "eastus"
app_service_sku = "P1v2"

# Database
sql_admin_username = "riteswipe_admin"
sql_admin_password = "<generated_password>"

# Redis
redis_sku = "Premium"
redis_family = "P"
redis_capacity = 1

# Monitoring
alert_email = "alerts@riteswipe.com"
alert_phone = "+1234567890"

# Security
allowed_ips = ["office_ip_1", "office_ip_2"]
blocked_countries = ["CountryA", "CountryB"]
```

### 3. Deploy Resources

```bash
# Validate configuration
terraform validate

# Plan deployment
terraform plan -out=tfplan

# Apply changes
terraform apply tfplan

# Save outputs
terraform output > deployment_outputs.txt
```

## Application Deployment

### 1. Backend API

#### Build and Test
```bash
# Restore packages
dotnet restore RiteSwipe.Api/RiteSwipe.Api.csproj

# Run tests
dotnet test RiteSwipe.Tests/RiteSwipe.Tests.csproj

# Publish application
dotnet publish RiteSwipe.Api/RiteSwipe.Api.csproj -c Release -o ./publish
```

#### Container Deployment
```bash
# Build container
docker build -t riteswipe-api:latest .

# Test locally
docker run -p 5000:80 riteswipe-api:latest

# Push to registry
docker tag riteswipe-api:latest $DOCKER_REGISTRY/riteswipe-api:latest
docker push $DOCKER_REGISTRY/riteswipe-api:latest
```

### 2. Frontend Web

#### Build and Deploy
```bash
# Install dependencies
cd RiteSwipe.Web
npm install

# Run tests
npm test

# Build production
npm run build

# Deploy to Static Web App
az staticwebapp deploy --source-location "build"
```

## Database Management

### 1. Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate --project RiteSwipe.Infrastructure

# Update database
dotnet ef database update --project RiteSwipe.Infrastructure

# Generate script
dotnet ef migrations script --project RiteSwipe.Infrastructure --output ./migrations.sql
```

### 2. Seed Data

```bash
# Run seeder
dotnet run --project RiteSwipe.Infrastructure/Seed/DatabaseSeeder.csproj

# Verify data
sqlcmd -S $DB_SERVER -d $DB_NAME -U $DB_USER -P $DB_PASSWORD -Q "SELECT * FROM Users"
```

## Monitoring Setup

### 1. Application Insights

```bash
# Get instrumentation key
APPINSIGHTS_KEY=$(az monitor app-insights component show --query instrumentationKey -o tsv)

# Configure application
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY"
```

### 2. Log Analytics

```bash
# Create workspace
az monitor log-analytics workspace create \
  --resource-group rg-riteswipe \
  --workspace-name log-riteswipe

# Enable solutions
az monitor log-analytics solution create \
  --resource-group rg-riteswipe \
  --solution-type ContainerInsights
```

## Security Configuration

### 1. SSL Certificates

```bash
# Generate certificate
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout riteswipe.key -out riteswipe.crt

# Upload to Key Vault
az keyvault certificate import \
  --vault-name kv-riteswipe \
  --name riteswipe-cert \
  --file riteswipe.pfx
```

### 2. Network Security

```bash
# Configure firewall rules
az network firewall rule create \
  --collection-name "AllowWeb" \
  --destination-ports 80 443 \
  --protocols "TCP" \
  --source-addresses "*" \
  --destination-addresses "*" \
  --translated-port 80 \
  --action "Allow" \
  --priority 100 \
  --rule-name "AllowWeb"
```

## Performance Optimization

### 1. CDN Setup

```bash
# Create CDN profile
az cdn profile create \
  --name cdn-riteswipe \
  --resource-group rg-riteswipe \
  --sku Standard_Microsoft

# Add endpoint
az cdn endpoint create \
  --name cdn-static \
  --profile-name cdn-riteswipe \
  --resource-group rg-riteswipe \
  --origin static-origin
```

### 2. Redis Configuration

```bash
# Configure Redis
az redis update \
  --name redis-riteswipe \
  --resource-group rg-riteswipe \
  --set maxmemory-policy=allkeys-lru \
  --set maxfragmentationmemory-reserved=100

# Test connection
redis-cli -h $REDIS_HOST -p 6380 -a $REDIS_KEY --tls ping
```

## Maintenance Procedures

### 1. Backup Verification

```bash
# List backups
az backup item list \
  --resource-group rg-riteswipe \
  --vault-name rsv-riteswipe

# Test restore
az backup restore restore-disks \
  --resource-group rg-riteswipe \
  --vault-name rsv-riteswipe
```

### 2. Performance Monitoring

```bash
# View metrics
az monitor metrics list \
  --resource $APP_ID \
  --metric "CpuPercentage"

# Check logs
az monitor log-analytics query \
  --workspace $WORKSPACE_ID \
  --query "AppTraces | limit 10"
```

## Troubleshooting

### Common Issues

1. **Database Connection**
```bash
# Test connection
sqlcmd -S $DB_SERVER -d $DB_NAME -U $DB_USER -P $DB_PASSWORD

# Check firewall
az sql server firewall-rule list \
  --resource-group rg-riteswipe \
  --server sql-riteswipe
```

2. **Application Errors**
```bash
# View logs
az webapp log tail \
  --name app-riteswipe-api \
  --resource-group rg-riteswipe

# Check configuration
az webapp config appsettings list \
  --name app-riteswipe-api \
  --resource-group rg-riteswipe
```

## Rollback Procedures

### 1. Application Rollback

```bash
# Swap deployment slots
az webapp deployment slot swap \
  --name app-riteswipe-api \
  --resource-group rg-riteswipe \
  --slot staging \
  --target-slot production

# Verify health
curl https://app-riteswipe-api.azurewebsites.net/health
```

### 2. Database Rollback

```bash
# Point-in-time restore
az sql db restore \
  --resource-group rg-riteswipe \
  --server sql-riteswipe \
  --name sqldb-riteswipe \
  --time "2025-04-26T00:00:00Z"
```

## Compliance Verification

### 1. Security Assessment

```bash
# Run assessment
az security assessment create \
  --assessment-type "SqlVulnerability" \
  --resource-group rg-riteswipe

# View results
az security assessment list \
  --resource-group rg-riteswipe
```

### 2. Compliance Report

```bash
# Generate report
az security task create \
  --name "ComplianceReport" \
  --resource-group rg-riteswipe

# Export results
az security task export \
  --name "ComplianceReport" \
  --resource-group rg-riteswipe
```
