variable "environment" {
  description = "Environment name (e.g., dev, staging, prod)"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "location_short" {
  description = "Short name for Azure region"
  type        = string
  default     = "eus"
}

variable "sql_admin_username" {
  description = "SQL Server admin username"
  type        = string
}

variable "sql_admin_password" {
  description = "SQL Server admin password"
  type        = string
  sensitive   = true
}

variable "sql_sku" {
  description = "SQL Database SKU"
  type        = string
  default     = "Basic"
}

variable "app_service_sku" {
  description = "App Service Plan SKU"
  type        = string
  default     = "B1"
}

variable "docker_registry" {
  description = "Docker registry URL"
  type        = string
  default     = "docker.io"
}

variable "docker_username" {
  description = "Docker registry username"
  type        = string
}

variable "docker_password" {
  description = "Docker registry password"
  type        = string
  sensitive   = true
}

variable "api_version" {
  description = "API Docker image version"
  type        = string
  default     = "latest"
}

variable "jwt_secret" {
  description = "JWT signing secret"
  type        = string
  sensitive   = true
}

variable "alert_email" {
  description = "Email address for alerts and notifications"
  type        = string
}

variable "monthly_budget" {
  description = "Monthly budget amount in USD"
  type        = number
  default     = 1000
}

variable "cost_centers" {
  description = "Map of cost centers for resource allocation"
  type = map(object({
    name       = string
    department = string
    owner      = string
  }))
  default = {
    app = {
      name       = "Application"
      department = "Engineering"
      owner      = "App Team"
    }
    data = {
      name       = "Data"
      department = "Engineering"
      owner      = "Data Team"
    }
    shared = {
      name       = "Shared"
      department = "Operations"
      owner      = "Ops Team"
    }
  }
}

locals {
  common_tags = {
    Environment = var.environment
    Project     = "RiteSwipe"
    ManagedBy   = "Terraform"
  }
}
