# CDN Profile
resource "azurerm_cdn_profile" "cdn" {
  name                = "cdn-riteswipe-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                = "Standard_Microsoft"

  tags = local.common_tags
}

# CDN Endpoint for Static Content
resource "azurerm_cdn_endpoint" "static" {
  name                = "cdn-static-${var.environment}"
  profile_name        = azurerm_cdn_profile.cdn.name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  origin {
    name       = "static-origin"
    host_name  = azurerm_storage_account.static.primary_web_host
  }

  optimization_type = "GeneralWebDelivery"

  delivery_rule {
    name  = "CacheStaticFiles"
    order = 1

    cache_expiration_action {
      behavior = "Override"
      duration = "7.00:00:00"
    }

    condition {
      path_pattern {
        operator     = "BeginsWith"
        match_values = ["/static/"]
      }
    }
  }

  tags = local.common_tags
}

# Storage Account for Static Content
resource "azurerm_storage_account" "static" {
  name                     = "strstatic${var.environment}${random_string.storage_random.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                = azurerm_resource_group.rg.location
  account_tier            = "Standard"
  account_replication_type = "LRS"
  account_kind            = "StorageV2"
  static_website {
    index_document = "index.html"
    error_404_document = "404.html"
  }

  tags = local.common_tags
}

# Application Gateway for API Load Balancing
resource "azurerm_application_gateway" "api" {
  name                = "agw-riteswipe-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  sku {
    name     = "Standard_v2"
    tier     = "Standard_v2"
    capacity = 2
  }

  gateway_ip_configuration {
    name      = "gateway-ip-config"
    subnet_id = azurerm_subnet.app_subnet.id
  }

  frontend_port {
    name = "https-port"
    port = 443
  }

  frontend_ip_configuration {
    name                 = "frontend-ip-config"
    public_ip_address_id = azurerm_public_ip.agw.id
  }

  backend_address_pool {
    name = "api-pool"
    fqdns = [azurerm_linux_web_app.api.default_hostname]
  }

  backend_http_settings {
    name                  = "api-settings"
    cookie_based_affinity = "Disabled"
    port                 = 443
    protocol             = "Https"
    request_timeout      = 30
    probe_name           = "api-probe"
  }

  probe {
    name                = "api-probe"
    host                = azurerm_linux_web_app.api.default_hostname
    protocol            = "Https"
    path                = "/health"
    interval            = 30
    timeout             = 30
    unhealthy_threshold = 3
  }

  http_listener {
    name                           = "https-listener"
    frontend_ip_configuration_name = "frontend-ip-config"
    frontend_port_name            = "https-port"
    protocol                      = "Https"
    ssl_certificate_name          = "api-cert"
  }

  ssl_certificate {
    name     = "api-cert"
    data     = filebase64(var.ssl_cert_path)
    password = var.ssl_cert_password
  }

  request_routing_rule {
    name                       = "api-rule"
    rule_type                 = "Basic"
    http_listener_name        = "https-listener"
    backend_address_pool_name = "api-pool"
    backend_http_settings_name = "api-settings"
    priority                  = 1
  }

  waf_configuration {
    enabled                  = true
    firewall_mode           = "Prevention"
    rule_set_type          = "OWASP"
    rule_set_version       = "3.2"
    file_upload_limit_mb   = 100
    max_request_body_size_kb = 128
  }

  tags = local.common_tags
}

# Redis Cache Premium with Clustering
resource "azurerm_redis_cache" "redis_premium" {
  name                = "redis-premium-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  capacity           = 1
  family             = "P"
  sku_name           = "Premium"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  shard_count        = 3

  redis_configuration {
    maxmemory_reserved = 100
    maxmemory_delta    = 100
    maxmemory_policy   = "allkeys-lru"
    maxfragmentationmemory_reserved = 100
  }

  patch_schedule {
    day_of_week    = "Sunday"
    start_hour_utc = 2
  }

  tags = local.common_tags
}

# Public IP for Application Gateway
resource "azurerm_public_ip" "agw" {
  name                = "pip-agw-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  allocation_method   = "Static"
  sku                = "Standard"

  tags = local.common_tags
}
