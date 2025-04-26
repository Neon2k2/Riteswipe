# Application Dashboard
resource "azurerm_dashboard" "app" {
  name                = "dash-riteswipe-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.common_tags

  dashboard_properties = <<DASHBOARD
{
    "lenses": {
        "0": {
            "order": 0,
            "parts": {
                "0": {
                    "position": {
                        "x": 0,
                        "y": 0,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [
                            {
                                "name": "resourceType",
                                "value": "Microsoft.Insights/components"
                            }
                        ],
                        "type": "Extension/AppInsightsExtension/PartType/AvailabilityNavPart"
                    }
                },
                "1": {
                    "position": {
                        "x": 6,
                        "y": 0,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [
                            {
                                "name": "resourceType",
                                "value": "Microsoft.Insights/components"
                            }
                        ],
                        "type": "Extension/AppInsightsExtension/PartType/QuickPulseButtonPart"
                    }
                },
                "2": {
                    "position": {
                        "x": 0,
                        "y": 4,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [
                            {
                                "name": "ComponentId",
                                "value": "${azurerm_application_insights.insights.id}"
                            }
                        ],
                        "type": "Extension/AppInsightsExtension/PartType/FailuresCurvedPart"
                    }
                },
                "3": {
                    "position": {
                        "x": 6,
                        "y": 4,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [
                            {
                                "name": "ComponentId",
                                "value": "${azurerm_application_insights.insights.id}"
                            }
                        ],
                        "type": "Extension/AppInsightsExtension/PartType/PerformancePart"
                    }
                }
            }
        }
    }
}
DASHBOARD
}

# Infrastructure Dashboard
resource "azurerm_dashboard" "infra" {
  name                = "dash-infra-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.common_tags

  dashboard_properties = <<DASHBOARD
{
    "lenses": {
        "0": {
            "order": 0,
            "parts": {
                "0": {
                    "position": {
                        "x": 0,
                        "y": 0,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [],
                        "type": "Extension/Microsoft_Azure_Monitoring/PartType/MetricsChartPart",
                        "settings": {
                            "content": {
                                "options": {
                                    "chart": {
                                        "metrics": [
                                            {
                                                "resourceMetadata": {
                                                    "id": "${azurerm_linux_web_app.api.id}"
                                                },
                                                "name": "CpuPercentage",
                                                "aggregationType": "Average",
                                                "namespace": "microsoft.web/sites",
                                                "metricVisualization": {
                                                    "displayName": "CPU Percentage"
                                                }
                                            }
                                        ],
                                        "title": "CPU Usage",
                                        "visualization": {
                                            "chartType": "Line"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                "1": {
                    "position": {
                        "x": 6,
                        "y": 0,
                        "colSpan": 6,
                        "rowSpan": 4
                    },
                    "metadata": {
                        "inputs": [],
                        "type": "Extension/Microsoft_Azure_Monitoring/PartType/MetricsChartPart",
                        "settings": {
                            "content": {
                                "options": {
                                    "chart": {
                                        "metrics": [
                                            {
                                                "resourceMetadata": {
                                                    "id": "${azurerm_linux_web_app.api.id}"
                                                },
                                                "name": "MemoryPercentage",
                                                "aggregationType": "Average",
                                                "namespace": "microsoft.web/sites",
                                                "metricVisualization": {
                                                    "displayName": "Memory Percentage"
                                                }
                                            }
                                        ],
                                        "title": "Memory Usage",
                                        "visualization": {
                                            "chartType": "Line"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
DASHBOARD
}
