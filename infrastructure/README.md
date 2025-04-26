# RiteSwipe Infrastructure

This directory contains the Terraform configuration for deploying RiteSwipe infrastructure to Azure.

## Prerequisites

1. [Terraform](https://www.terraform.io/downloads.html) (>= 1.0.0)
2. [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. Azure subscription
4. Docker Hub account

## Infrastructure Components

- Resource Group
- Azure SQL Server and Database
- Azure Redis Cache
- App Service Plan
- Linux Web App (API)
- Static Web App (Frontend)
- Application Insights
- Key Vault

## Getting Started

1. Log in to Azure:
```bash
az login
```

2. Create a storage account for Terraform state:
```bash
az group create --name riteswipe-tfstate --location eastus
az storage account create --name riteswipetfstate --resource-group riteswipe-tfstate --location eastus --sku Standard_LRS
az storage container create --name tfstate --account-name riteswipetfstate
```

3. Copy and configure variables:
```bash
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your values
```

4. Initialize Terraform:
```bash
terraform init
```

5. Plan the deployment:
```bash
terraform plan -out=tfplan
```

6. Apply the configuration:
```bash
terraform apply tfplan
```

## Environment Variables

- `environment`: Deployment environment (dev, staging, prod)
- `location`: Azure region
- `sql_admin_username`: SQL Server admin username
- `sql_admin_password`: SQL Server admin password
- `docker_username`: Docker Hub username
- `docker_password`: Docker Hub password
- `jwt_secret`: Secret for JWT token signing

## Outputs

- `api_url`: URL of the deployed API
- `web_url`: URL of the static web app
- `sql_server_fqdn`: SQL Server fully qualified domain name
- `redis_connection_string`: Redis connection string
- `application_insights_key`: Application Insights instrumentation key
- `key_vault_uri`: Key Vault URI

## Cleanup

To destroy the infrastructure:
```bash
terraform destroy
```

## Security Notes

1. Never commit `terraform.tfvars` or any files containing sensitive information
2. Use Azure Key Vault for storing secrets in production
3. Enable Azure AD authentication for SQL Server in production
4. Configure network security groups appropriately
5. Enable HTTPS-only access for web applications
