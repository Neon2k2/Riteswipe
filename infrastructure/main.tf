terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "riteswipe-tfstate"
    storage_account_name = "riteswipetfstate"
    container_name      = "tfstate"
    key                 = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.environment}-${var.location_short}"
  location = var.location

  tags = local.common_tags
}

# SQL Server
resource "azurerm_mssql_server" "sql" {
  name                         = "sql-riteswipe-${var.environment}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password

  tags = local.common_tags
}

# SQL Database
resource "azurerm_mssql_database" "db" {
  name           = "sqldb-riteswipe-${var.environment}"
  server_id      = azurerm_mssql_server.sql.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  sku_name       = var.sql_sku
  zone_redundant = false

  tags = local.common_tags
}

# Redis Cache
resource "azurerm_redis_cache" "redis" {
  name                = "redis-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  capacity           = 1
  family             = "C"
  sku_name           = "Standard"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  redis_configuration {
    maxmemory_reserved = 50
    maxmemory_delta    = 50
    maxmemory_policy   = "allkeys-lru"
  }

  tags = local.common_tags
}

# App Service Plan
resource "azurerm_service_plan" "plan" {
  name                = "plan-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type            = "Linux"
  sku_name           = var.app_service_sku

  tags = local.common_tags
}

# API App Service
resource "azurerm_linux_web_app" "api" {
  name                = "app-riteswipe-api-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    always_on = true
    application_stack {
      docker_image     = "${var.docker_registry}/riteswipe-api"
      docker_image_tag = var.api_version
    }
  }

  app_settings = {
    "DOCKER_REGISTRY_SERVER_URL"          = var.docker_registry
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.docker_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = var.docker_password
    "ConnectionStrings__DefaultConnection" = "Server=${azurerm_mssql_server.sql.fully_qualified_domain_name};Database=${azurerm_mssql_database.db.name};User=${var.sql_admin_username};Password=${var.sql_admin_password};TrustServerCertificate=True"
    "Redis__ConnectionString"             = azurerm_redis_cache.redis.primary_connection_string
    "JwtSettings__Secret"                 = var.jwt_secret
    "WEBSITES_PORT"                       = "80"
    "ASPNETCORE_ENVIRONMENT"              = var.environment
  }

  tags = local.common_tags
}

# Static Web App
resource "azurerm_static_site" "web" {
  name                = "stapp-riteswipe-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location           = azurerm_resource_group.rg.location
  sku_tier           = "Standard"
  sku_size           = "Standard"

  tags = local.common_tags
}

# Application Insights
resource "azurerm_application_insights" "insights" {
  name                = "appi-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"

  tags = local.common_tags
}

# Key Vault
resource "azurerm_key_vault" "kv" {
  name                = "kv-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  tenant_id          = data.azurerm_client_config.current.tenant_id
  sku_name           = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get", "List", "Create", "Delete", "Update",
    ]

    secret_permissions = [
      "Get", "List", "Set", "Delete",
    ]
  }

  tags = local.common_tags
}
