# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "logs" {
  name                = "log-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 90

  tags = local.common_tags
}

# Application Insights for API
resource "azurerm_application_insights" "api" {
  name                = "appi-api-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.logs.id

  tags = local.common_tags
}

# Diagnostic Settings for App Service
resource "azurerm_monitor_diagnostic_setting" "app_service" {
  name                       = "diag-app-${var.environment}"
  target_resource_id         = azurerm_linux_web_app.api.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.logs.id

  enabled_log {
    category = "AppServiceHTTPLogs"
  }

  enabled_log {
    category = "AppServiceConsoleLogs"
  }

  enabled_log {
    category = "AppServiceAppLogs"
  }

  metric {
    category = "AllMetrics"
  }
}

# Diagnostic Settings for SQL Database
resource "azurerm_monitor_diagnostic_setting" "sql" {
  name                       = "diag-sql-${var.environment}"
  target_resource_id         = azurerm_mssql_database.db.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.logs.id

  enabled_log {
    category = "SQLInsights"
  }

  enabled_log {
    category = "AutomaticTuning"
  }

  enabled_log {
    category = "QueryStoreRuntimeStatistics"
  }

  metric {
    category = "Basic"
  }

  metric {
    category = "InstanceAndAppAdvanced"
  }
}

# Log Analytics Solutions
resource "azurerm_log_analytics_solution" "container_insights" {
  solution_name         = "ContainerInsights"
  location             = azurerm_resource_group.rg.location
  resource_group_name  = azurerm_resource_group.rg.name
  workspace_resource_id = azurerm_log_analytics_workspace.logs.id
  workspace_name       = azurerm_log_analytics_workspace.logs.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }

  tags = local.common_tags
}

# Alert Rules
resource "azurerm_monitor_metric_alert" "high_cpu" {
  name                = "alert-cpu-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  scopes              = [azurerm_linux_web_app.api.id]
  description         = "Alert when CPU usage is high"

  criteria {
    metric_namespace = "Microsoft.Web/sites"
    metric_name      = "CpuPercentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }

  action {
    action_group_id = azurerm_monitor_action_group.critical.id
  }

  tags = local.common_tags
}

resource "azurerm_monitor_metric_alert" "high_memory" {
  name                = "alert-memory-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  scopes              = [azurerm_linux_web_app.api.id]
  description         = "Alert when memory usage is high"

  criteria {
    metric_namespace = "Microsoft.Web/sites"
    metric_name      = "MemoryPercentage"
    aggregation      = "Average"
    operator         = "GreaterThan"
    threshold        = 80
  }

  action {
    action_group_id = azurerm_monitor_action_group.critical.id
  }

  tags = local.common_tags
}

# Log Analytics Queries
resource "azurerm_log_analytics_saved_search" "error_logs" {
  name                       = "ErrorLogs"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.logs.id
  category                  = "Errors"
  display_name              = "Application Errors"
  query                     = <<QUERY
AppTraces
| where SeverityLevel == 3
| project TimeGenerated, Message, OperationName
| order by TimeGenerated desc
QUERY
}

resource "azurerm_log_analytics_saved_search" "performance_logs" {
  name                       = "PerformanceLogs"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.logs.id
  category                  = "Performance"
  display_name              = "Performance Metrics"
  query                     = <<QUERY
Perf
| where ObjectName == "Processor" and CounterName == "% Processor Time"
| summarize avg(CounterValue) by bin(TimeGenerated, 5m)
| render timechart
QUERY
}

# Export Settings
resource "azurerm_monitor_diagnostic_setting" "export_logs" {
  name                       = "export-logs-${var.environment}"
  target_resource_id         = azurerm_log_analytics_workspace.logs.id
  storage_account_id         = azurerm_storage_account.logs.id

  enabled_log {
    category = "Audit"
  }

  enabled_log {
    category = "AllMetrics"
  }
}

# Storage Account for Log Export
resource "azurerm_storage_account" "logs" {
  name                     = "strlogs${var.environment}${random_string.storage_random.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                = azurerm_resource_group.rg.location
  account_tier            = "Standard"
  account_replication_type = "LRS"
  min_tls_version         = "TLS1_2"

  tags = local.common_tags
}
