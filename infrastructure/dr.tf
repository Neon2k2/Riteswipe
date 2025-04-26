# Secondary Region Resources
locals {
  secondary_location = "westus"
  secondary_location_short = "wus"
}

# Secondary Resource Group
resource "azurerm_resource_group" "rg_secondary" {
  name     = "rg-${var.environment}-${local.secondary_location_short}"
  location = local.secondary_location

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Secondary SQL Server
resource "azurerm_mssql_server" "sql_secondary" {
  name                         = "sql-riteswipe-${var.environment}-${local.secondary_location_short}"
  resource_group_name          = azurerm_resource_group.rg_secondary.name
  location                     = azurerm_resource_group.rg_secondary.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Enable Geo-Replication for SQL Database
resource "azurerm_mssql_database" "db_secondary" {
  name                        = "sqldb-riteswipe-${var.environment}"
  server_id                   = azurerm_mssql_server.sql_secondary.id
  create_mode                 = "Secondary"
  creation_source_database_id = azurerm_mssql_database.db.id

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Secondary App Service Plan
resource "azurerm_service_plan" "plan_secondary" {
  name                = "plan-riteswipe-${var.environment}-${local.secondary_location_short}"
  location            = azurerm_resource_group.rg_secondary.location
  resource_group_name = azurerm_resource_group.rg_secondary.name
  os_type            = "Linux"
  sku_name           = var.app_service_sku

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Secondary API App Service
resource "azurerm_linux_web_app" "api_secondary" {
  name                = "app-riteswipe-api-${var.environment}-${local.secondary_location_short}"
  location            = azurerm_resource_group.rg_secondary.location
  resource_group_name = azurerm_resource_group.rg_secondary.name
  service_plan_id     = azurerm_service_plan.plan_secondary.id

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
    "ConnectionStrings__DefaultConnection" = "Server=${azurerm_mssql_server.sql_secondary.fully_qualified_domain_name};Database=${azurerm_mssql_database.db_secondary.name};User=${var.sql_admin_username};Password=${var.sql_admin_password};TrustServerCertificate=True"
    "Redis__ConnectionString"             = azurerm_redis_cache.redis_secondary.primary_connection_string
    "JwtSettings__Secret"                 = var.jwt_secret
    "WEBSITES_PORT"                       = "80"
    "ASPNETCORE_ENVIRONMENT"              = var.environment
  }

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Secondary Redis Cache
resource "azurerm_redis_cache" "redis_secondary" {
  name                = "redis-riteswipe-${var.environment}-${local.secondary_location_short}"
  location            = azurerm_resource_group.rg_secondary.location
  resource_group_name = azurerm_resource_group.rg_secondary.name
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

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Traffic Manager Profile
resource "azurerm_traffic_manager_profile" "tm" {
  name                = "tm-riteswipe-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name

  traffic_routing_method = "Priority"

  dns_config {
    relative_name = "riteswipe-${var.environment}"
    ttl          = 60
  }

  monitor_config {
    protocol                     = "HTTPS"
    port                        = 443
    path                        = "/health"
    interval_in_seconds         = 30
    timeout_in_seconds         = 10
    tolerated_number_of_failures = 3
  }

  tags = local.common_tags
}

# Primary Endpoint
resource "azurerm_traffic_manager_endpoint" "primary" {
  name                = "primary"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target_resource_id  = azurerm_linux_web_app.api.id
  type                = "AzureEndpoints"
  priority            = 1
}

# Secondary Endpoint
resource "azurerm_traffic_manager_endpoint" "secondary" {
  name                = "secondary"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target_resource_id  = azurerm_linux_web_app.api_secondary.id
  type                = "AzureEndpoints"
  priority            = 2
}

# Site Recovery Vault
resource "azurerm_recovery_services_vault" "vault_dr" {
  name                = "rsv-riteswipe-dr-${var.environment}"
  location            = azurerm_resource_group.rg_secondary.location
  resource_group_name = azurerm_resource_group.rg_secondary.name
  sku                 = "Standard"
  soft_delete_enabled = true

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

# Replication Policy
resource "azurerm_site_recovery_replication_policy" "policy" {
  name                                                 = "policy-replication-${var.environment}"
  resource_group_name                                 = azurerm_resource_group.rg_secondary.name
  recovery_vault_name                                 = azurerm_recovery_services_vault.vault_dr.name
  recovery_point_retention_in_minutes                = 24 * 60 # 24 hours
  application_consistent_snapshot_frequency_in_minutes = 4 * 60 # 4 hours
}

# Failover Automation Runbook
resource "azurerm_automation_account" "automation" {
  name                = "auto-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg_secondary.location
  resource_group_name = azurerm_resource_group.rg_secondary.name
  sku_name           = "Basic"

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}

resource "azurerm_automation_runbook" "failover" {
  name                    = "rb-failover-${var.environment}"
  location                = azurerm_resource_group.rg_secondary.location
  resource_group_name     = azurerm_resource_group.rg_secondary.name
  automation_account_name = azurerm_automation_account.automation.name
  log_verbose            = true
  log_progress           = true
  description            = "Runbook for automated failover process"
  runbook_type          = "PowerShell"

  content = <<CONTENT
# Failover Runbook
param(
    [string]$SubscriptionId,
    [string]$ResourceGroupName,
    [string]$TrafficManagerProfileName
)

# Login to Azure
Connect-AzAccount -Identity

# Set subscription context
Set-AzContext -SubscriptionId $SubscriptionId

# Swap Traffic Manager endpoints priority
$profile = Get-AzTrafficManagerProfile -ResourceGroupName $ResourceGroupName -Name $TrafficManagerProfileName
$endpoints = Get-AzTrafficManagerEndpoint -ProfileName $TrafficManagerProfileName -ResourceGroupName $ResourceGroupName

foreach ($endpoint in $endpoints) {
    if ($endpoint.Priority -eq 1) {
        $endpoint.Priority = 2
    } else {
        $endpoint.Priority = 1
    }
    Set-AzTrafficManagerEndpoint -TrafficManagerEndpoint $endpoint
}
CONTENT

  tags = merge(local.common_tags, {
    Role = "DR"
  })
}
