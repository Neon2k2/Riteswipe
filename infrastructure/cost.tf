# Cost Management Budget
resource "azurerm_consumption_budget_resource_group" "budget" {
  name              = "budget-riteswipe-${var.environment}"
  resource_group_id = azurerm_resource_group.rg.id

  amount     = var.monthly_budget
  time_grain = "Monthly"

  time_period {
    start_date = formatdate("YYYY-MM-01", timestamp())
    end_date   = timeadd(formatdate("YYYY-MM-01", timestamp()), "8760h") # 1 year
  }

  notification {
    enabled        = true
    threshold      = 70.0
    threshold_type = "Actual"
    operator       = "GreaterThan"

    contact_emails = [var.alert_email]
  }

  notification {
    enabled        = true
    threshold      = 90.0
    threshold_type = "Actual"
    operator       = "GreaterThan"

    contact_emails = [var.alert_email]
  }

  notification {
    enabled        = true
    threshold      = 100.0
    threshold_type = "Actual"
    operator       = "GreaterThan"

    contact_emails = [var.alert_email]
  }
}

# Cost Management Export
resource "azurerm_cost_management_export_resource_group" "export" {
  name              = "export-riteswipe-${var.environment}"
  resource_group_id = azurerm_resource_group.rg.id

  recurrence_type = "Monthly"
  recurrence_period {
    from = formatdate("YYYY-MM-01", timestamp())
  }

  export_data_storage_location {
    container_id = "${azurerm_storage_account.backup.id}/blobServices/default/containers/${azurerm_storage_container.backup.name}"
    root_folder_path = "cost-exports"
  }

  export_data_options {
    type = "Usage"
    time_frame = "MonthToDate"
  }

  delivery_info {
    destination {
      resource_id = azurerm_storage_account.backup.id
      container   = azurerm_storage_container.backup.name
    }
  }
}

# Resource Tags for Cost Allocation
resource "azurerm_resource_group" "cost_center" {
  for_each = var.cost_centers

  name     = "rg-${each.key}-${var.environment}"
  location = var.location

  tags = merge(local.common_tags, {
    CostCenter = each.value.name
    Department = each.value.department
    Owner      = each.value.owner
  })
}

# Cost Analysis Workbook
resource "azurerm_application_insights_workbook" "cost" {
  name                = "wb-cost-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location           = azurerm_resource_group.rg.location
  display_name       = "Cost Analysis"
  
  data_json = jsonencode({
    version = "Notebook/1.0",
    items = [
      {
        type = "cost",
        settings = {
          scope = azurerm_resource_group.rg.id,
          timeframe = "Last30Days",
          view = "chart",
          chartType = "line",
          granularity = "Daily"
        }
      },
      {
        type = "cost",
        settings = {
          scope = azurerm_resource_group.rg.id,
          timeframe = "Last30Days",
          view = "table",
          grouping = ["ResourceType", "ResourceGroup"]
        }
      }
    ]
  })

  tags = local.common_tags
}
