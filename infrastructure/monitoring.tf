# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "workspace" {
  name                = "log-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = local.common_tags
}

# Application Insights Workbook
resource "azurerm_application_insights_workbook" "monitoring" {
  name                = "wb-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  display_name        = "RiteSwipe Monitoring"
  data_json = jsonencode({
    version = "Notebook/1.0",
    items = [
      {
        type = "metric",
        settings = {
          metric = "requests/count",
          aggregation = "sum",
          timespan = "P1D",
          title = "API Requests"
        }
      },
      {
        type = "metric",
        settings = {
          metric = "exceptions/count",
          aggregation = "sum",
          timespan = "P1D",
          title = "Exceptions"
        }
      }
    ]
  })

  tags = local.common_tags
}

# Monitor Action Group
resource "azurerm_monitor_action_group" "critical" {
  name                = "ag-critical-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  short_name          = "critical"

  email_receiver {
    name          = "admin"
    email_address = var.alert_email
  }

  tags = local.common_tags
}

# Monitor Alert Rules
resource "azurerm_monitor_metric_alert" "high_cpu" {
  name                = "alert-highcpu-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  scopes              = [azurerm_service_plan.plan.id]
  description         = "Alert when CPU usage is high"

  criteria {
    metric_namespace = "Microsoft.Web/serverfarms"
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
  name                = "alert-highmemory-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  scopes              = [azurerm_service_plan.plan.id]
  description         = "Alert when memory usage is high"

  criteria {
    metric_namespace = "Microsoft.Web/serverfarms"
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

# Diagnostic Settings
resource "azurerm_monitor_diagnostic_setting" "app" {
  name                       = "diag-app-${var.environment}"
  target_resource_id        = azurerm_linux_web_app.api.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.workspace.id

  enabled_log {
    category = "AppServiceHTTPLogs"
  }

  enabled_log {
    category = "AppServiceConsoleLogs"
  }

  metric {
    category = "AllMetrics"
  }
}

resource "azurerm_monitor_diagnostic_setting" "sql" {
  name                       = "diag-sql-${var.environment}"
  target_resource_id        = azurerm_mssql_database.db.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.workspace.id

  enabled_log {
    category = "SQLInsights"
  }

  enabled_log {
    category = "AutomaticTuning"
  }

  metric {
    category = "AllMetrics"
  }
}
