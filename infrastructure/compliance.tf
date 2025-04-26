# Azure Policy for Compliance
resource "azurerm_policy_definition" "allowed_locations" {
  name         = "allowed-locations"
  policy_type  = "Custom"
  mode         = "Indexed"
  display_name = "Allowed locations for resources"

  policy_rule = <<POLICY_RULE
{
  "if": {
    "not": {
      "field": "location",
      "in": "[parameters('allowedLocations')]"
    }
  },
  "then": {
    "effect": "deny"
  }
}
POLICY_RULE

  parameters = <<PARAMETERS
{
  "allowedLocations": {
    "type": "Array",
    "metadata": {
      "description": "The list of allowed locations for resources.",
      "displayName": "Allowed locations"
    }
  }
}
PARAMETERS
}

# Compliance Monitoring
resource "azurerm_monitor_activity_log_alert" "compliance" {
  name                = "alert-compliance-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  scopes              = ["/subscriptions/${data.azurerm_client_config.current.subscription_id}"]
  description         = "Alert for compliance-related events"

  criteria {
    category = "Administrative"
    
    resource_provider = "Microsoft.Security"
    operation_name    = "Microsoft.Security/policies/write"
  }

  action {
    action_group_id = azurerm_monitor_action_group.critical.id
  }

  tags = local.common_tags
}

# Security Center Assessment
resource "azurerm_security_center_assessment" "compliance_assessment" {
  assessment_policy_id = "/providers/Microsoft.Security/assessmentMetadata/compliance_assessment"
  target_resource_id  = azurerm_resource_group.rg.id
}

# Log Analytics Solutions
resource "azurerm_log_analytics_solution" "security" {
  solution_name         = "Security"
  location             = azurerm_resource_group.rg.location
  resource_group_name  = azurerm_resource_group.rg.name
  workspace_resource_id = azurerm_log_analytics_workspace.workspace.id
  workspace_name       = azurerm_log_analytics_workspace.workspace.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/Security"
  }

  tags = local.common_tags
}

resource "azurerm_log_analytics_solution" "compliance" {
  solution_name         = "SecurityCenterFree"
  location             = azurerm_resource_group.rg.location
  resource_group_name  = azurerm_resource_group.rg.name
  workspace_resource_id = azurerm_log_analytics_workspace.workspace.id
  workspace_name       = azurerm_log_analytics_workspace.workspace.name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/SecurityCenterFree"
  }

  tags = local.common_tags
}

# Compliance Workbook
resource "azurerm_application_insights_workbook" "compliance" {
  name                = "wb-compliance-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  display_name        = "Compliance Dashboard"
  
  data_json = jsonencode({
    version = "Notebook/1.0",
    items = [
      {
        type = "query",
        settings = {
          query = "SecurityEvent | where EventID == 4624 | summarize count() by Account",
          title = "Login Events"
        }
      },
      {
        type = "query",
        settings = {
          query = "AzureActivity | where OperationName contains 'Microsoft.Security'",
          title = "Security Operations"
        }
      }
    ]
  })

  tags = local.common_tags
}

# Audit Policy
resource "azurerm_policy_definition" "audit_logs" {
  name         = "audit-logs-retention"
  policy_type  = "Custom"
  mode         = "All"
  display_name = "Audit log retention policy"

  policy_rule = <<POLICY_RULE
{
  "if": {
    "allOf": [
      {
        "field": "type",
        "equals": "Microsoft.Storage/storageAccounts"
      }
    ]
  },
  "then": {
    "effect": "auditIfNotExists",
    "details": {
      "type": "Microsoft.Storage/storageAccounts/blobServices",
      "existenceCondition": {
        "allOf": [
          {
            "field": "Microsoft.Storage/storageAccounts/blobServices/default.deleteRetentionPolicy.enabled",
            "equals": "true"
          },
          {
            "field": "Microsoft.Storage/storageAccounts/blobServices/default.deleteRetentionPolicy.days",
            "greaterOrEquals": 90
          }
        ]
      }
    }
  }
}
POLICY_RULE

  parameters = <<PARAMETERS
{
  "effect": {
    "type": "String",
    "defaultValue": "auditIfNotExists",
    "allowedValues": ["auditIfNotExists", "disabled"]
  }
}
PARAMETERS
}

# Compliance Reports Storage
resource "azurerm_storage_account" "compliance" {
  name                     = "stcomp${var.environment}${random_string.storage_random.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                = azurerm_resource_group.rg.location
  account_tier            = "Standard"
  account_replication_type = "GRS"
  min_tls_version         = "TLS1_2"

  blob_properties {
    versioning_enabled = true
    
    container_delete_retention_policy {
      days = 90
    }

    delete_retention_policy {
      days = 90
    }
  }

  tags = merge(local.common_tags, {
    DataClassification = "Confidential"
    Compliance        = "Required"
  })
}
