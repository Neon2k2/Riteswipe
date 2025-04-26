# Recovery Services Vault
resource "azurerm_recovery_services_vault" "vault" {
  name                = "rsv-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
  soft_delete_enabled = true

  tags = local.common_tags
}

# SQL Database Backup Policy
resource "azurerm_backup_policy_vm" "sql_backup" {
  name                = "policy-sql-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  recovery_vault_name = azurerm_recovery_services_vault.vault.name

  timezone = "UTC"

  backup {
    frequency = "Daily"
    time      = "23:00"
  }

  retention_daily {
    count = 7
  }

  retention_weekly {
    count    = 4
    weekdays = ["Sunday"]
  }

  retention_monthly {
    count    = 12
    weekdays = ["Sunday"]
    weeks    = ["First"]
  }

  retention_yearly {
    count    = 1
    weekdays = ["Sunday"]
    weeks    = ["First"]
    months   = ["January"]
  }
}

# Storage Account for File Backups
resource "azurerm_storage_account" "backup" {
  name                     = "stbackup${var.environment}${random_string.storage_random.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "GRS"
  min_tls_version         = "TLS1_2"

  blob_properties {
    versioning_enabled = true
    
    container_delete_retention_policy {
      days = 7
    }

    delete_retention_policy {
      days = 7
    }
  }

  tags = local.common_tags
}

# Backup Container
resource "azurerm_storage_container" "backup" {
  name                  = "backup"
  storage_account_name  = azurerm_storage_account.backup.name
  container_access_type = "private"
}

# Key Vault Backup
resource "azurerm_key_vault_backup" "kv_backup" {
  key_vault_id = azurerm_key_vault.kv.id
  storage_account_id = azurerm_storage_account.backup.id
  storage_container_name = azurerm_storage_container.backup.name
}

# Random String for Storage Account Name
resource "random_string" "storage_random" {
  length  = 8
  special = false
  upper   = false
}
