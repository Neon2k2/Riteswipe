# Azure Security Center
resource "azurerm_security_center_subscription_pricing" "security_center" {
  tier          = "Standard"
  resource_type = "VirtualMachines"
}

resource "azurerm_security_center_contact" "security_contact" {
  email = var.security_contact_email
  phone = var.security_contact_phone

  alert_notifications = true
  alerts_to_admins   = true
}

# Web Application Firewall
resource "azurerm_frontdoor_firewall_policy" "waf" {
  name                = "waf-riteswipe-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  enabled            = true
  mode               = "Prevention"

  managed_rule {
    type    = "DefaultRuleSet"
    version = "1.0"
  }

  managed_rule {
    type    = "Microsoft_BotManagerRuleSet"
    version = "1.0"
  }

  custom_rule {
    name     = "BlockHighRiskCountries"
    enabled  = true
    priority = 100
    type     = "MatchRule"
    action   = "Block"

    match_condition {
      match_variable     = "RemoteAddr"
      operator          = "GeoMatch"
      negation_condition = false
      match_values      = var.blocked_countries
    }
  }

  custom_rule {
    name     = "RateLimitRule"
    enabled  = true
    priority = 200
    type     = "RateLimitRule"
    action   = "Block"

    match_condition {
      match_variable = "RequestUri"
      operator      = "Any"
    }

    rate_limit_duration_in_minutes = 1
    rate_limit_threshold          = 1000
  }
}

# DDoS Protection
resource "azurerm_network_ddos_protection_plan" "ddos" {
  name                = "ddos-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  tags = local.common_tags
}

# Private Endpoints
resource "azurerm_private_endpoint" "sql" {
  name                = "pe-sql-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  subnet_id           = azurerm_subnet.db_subnet.id

  private_service_connection {
    name                           = "sql-privateserviceconnection"
    private_connection_resource_id = azurerm_mssql_server.sql.id
    subresource_names             = ["sqlServer"]
    is_manual_connection          = false
  }

  tags = local.common_tags
}

resource "azurerm_private_endpoint" "redis" {
  name                = "pe-redis-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  subnet_id           = azurerm_subnet.db_subnet.id

  private_service_connection {
    name                           = "redis-privateserviceconnection"
    private_connection_resource_id = azurerm_redis_cache.redis.id
    subresource_names             = ["redisCache"]
    is_manual_connection          = false
  }

  tags = local.common_tags
}

# Key Vault Access Policies
resource "azurerm_key_vault_access_policy" "app" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_web_app.api.identity[0].principal_id

  key_permissions = [
    "Get",
  ]

  secret_permissions = [
    "Get",
  ]
}

# Azure AD Integration
resource "azurerm_user_assigned_identity" "api_identity" {
  name                = "id-api-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  tags = local.common_tags
}

# Security Policies
resource "azurerm_policy_definition" "require_tag" {
  name         = "require-environment-tag"
  policy_type  = "Custom"
  mode         = "Indexed"
  display_name = "Require environment tag on resources"

  policy_rule = <<POLICY_RULE
{
  "if": {
    "allOf": [
      {
        "field": "tags['Environment']",
        "exists": "false"
      }
    ]
  },
  "then": {
    "effect": "deny"
  }
}
POLICY_RULE

  parameters = <<PARAMETERS
{
  "effect": {
    "type": "String",
    "defaultValue": "deny",
    "allowedValues": ["audit", "deny"]
  }
}
PARAMETERS
}

# Azure Monitor Diagnostic Settings
resource "azurerm_monitor_diagnostic_setting" "keyvault" {
  name                       = "diag-kv-${var.environment}"
  target_resource_id        = azurerm_key_vault.kv.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.workspace.id

  enabled_log {
    category = "AuditEvent"
  }

  enabled_log {
    category = "AzurePolicyEvaluationDetails"
  }

  metric {
    category = "AllMetrics"
  }
}

# Security Center Workflow Automation
resource "azurerm_logic_app_workflow" "security_alerts" {
  name                = "logic-secalerts-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  workflow_parameters = {
    "$connections" = jsonencode({
      teams = {
        connectionId = azurerm_api_connection.teams.id
        connectionName = "teams"
        id = "/subscriptions/${data.azurerm_client_config.current.subscription_id}/providers/Microsoft.Web/locations/${azurerm_resource_group.rg.location}/managedApis/teams"
      }
    })
  }

  tags = local.common_tags
}

# API Connection for Teams
resource "azurerm_api_connection" "teams" {
  name                = "api-teams-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  managed_api_id     = "/subscriptions/${data.azurerm_client_config.current.subscription_id}/providers/Microsoft.Web/locations/${azurerm_resource_group.rg.location}/managedApis/teams"
  display_name       = "Teams Connection"

  parameter_values = {
    "token:TenantId" = data.azurerm_client_config.current.tenant_id
  }
}
