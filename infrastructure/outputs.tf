output "api_url" {
  description = "URL of the API App Service"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "web_url" {
  description = "URL of the Static Web App"
  value       = azurerm_static_site.web.default_host_name
}

output "sql_server_fqdn" {
  description = "Fully qualified domain name of the SQL Server"
  value       = azurerm_mssql_server.sql.fully_qualified_domain_name
}

output "redis_connection_string" {
  description = "Redis connection string"
  value       = azurerm_redis_cache.redis.primary_connection_string
  sensitive   = true
}

output "application_insights_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.insights.instrumentation_key
  sensitive   = true
}

output "key_vault_uri" {
  description = "Key Vault URI"
  value       = azurerm_key_vault.kv.vault_uri
}
