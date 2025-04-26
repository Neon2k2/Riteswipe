# Autoscaling Settings for App Service
resource "azurerm_monitor_autoscale_setting" "app_scaling" {
  name                = "as-app-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  target_resource_id  = azurerm_service_plan.plan.id

  profile {
    name = "defaultProfile"

    capacity {
      default = 1
      minimum = 1
      maximum = 10
    }

    # Scale out on high CPU
    rule {
      metric_trigger {
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.plan.id
        time_grain        = "PT1M"
        statistic         = "Average"
        time_window       = "PT10M"
        time_aggregation  = "Average"
        operator          = "GreaterThan"
        threshold         = 75
      }

      scale_action {
        direction = "Increase"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT5M"
      }
    }

    # Scale out on high memory
    rule {
      metric_trigger {
        metric_name        = "MemoryPercentage"
        metric_resource_id = azurerm_service_plan.plan.id
        time_grain        = "PT1M"
        statistic         = "Average"
        time_window       = "PT10M"
        time_aggregation  = "Average"
        operator          = "GreaterThan"
        threshold         = 75
      }

      scale_action {
        direction = "Increase"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT5M"
      }
    }

    # Scale in on low CPU
    rule {
      metric_trigger {
        metric_name        = "CpuPercentage"
        metric_resource_id = azurerm_service_plan.plan.id
        time_grain        = "PT1M"
        statistic         = "Average"
        time_window       = "PT10M"
        time_aggregation  = "Average"
        operator          = "LessThan"
        threshold         = 25
      }

      scale_action {
        direction = "Decrease"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT5M"
      }
    }

    # Scale in on low memory
    rule {
      metric_trigger {
        metric_name        = "MemoryPercentage"
        metric_resource_id = azurerm_service_plan.plan.id
        time_grain        = "PT1M"
        statistic         = "Average"
        time_window       = "PT10M"
        time_aggregation  = "Average"
        operator          = "LessThan"
        threshold         = 25
      }

      scale_action {
        direction = "Decrease"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT5M"
      }
    }

    # Scale based on request queue length
    rule {
      metric_trigger {
        metric_name        = "HttpQueueLength"
        metric_resource_id = azurerm_service_plan.plan.id
        time_grain        = "PT1M"
        statistic         = "Average"
        time_window       = "PT5M"
        time_aggregation  = "Average"
        operator          = "GreaterThan"
        threshold         = 100
      }

      scale_action {
        direction = "Increase"
        type      = "ChangeCount"
        value     = "1"
        cooldown  = "PT5M"
      }
    }
  }

  # Schedule-based scaling for business hours
  profile {
    name = "businessHours"
    
    capacity {
      default = 2
      minimum = 2
      maximum = 10
    }

    recurrence {
      timezone = "Asia/Kolkata"
      days     = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
      hours    = [9]
      minutes  = [0]
    }
  }

  # Schedule-based scaling for non-business hours
  profile {
    name = "nonBusinessHours"
    
    capacity {
      default = 1
      minimum = 1
      maximum = 3
    }

    recurrence {
      timezone = "Asia/Kolkata"
      days     = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
      hours    = [18]
      minutes  = [0]
    }
  }

  notification {
    email {
      send_to_subscription_administrator    = true
      send_to_subscription_co_administrator = true
      custom_emails                        = [var.alert_email]
    }
  }

  tags = local.common_tags
}
