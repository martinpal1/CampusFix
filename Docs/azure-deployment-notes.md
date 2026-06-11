# Azure deployment notes

Recommended Azure resources:

1. Azure App Service for the ASP.NET Core web application.
2. Azure SQL Database for relational data storage.
3. Optional Azure Blob Storage for request attachments.
4. Application settings in Azure App Service for production connection strings.

Suggested deployment flow:

```bash
az login
az group create --name campusfix-rg --location uksouth
az appservice plan create --name campusfix-plan --resource-group campusfix-rg --sku B1 --is-linux
az webapp create --resource-group campusfix-rg --plan campusfix-plan --name <unique-app-name> --runtime "DOTNETCORE:8.0"
```

For Azure SQL, create a server and database, then set the `DefaultConnection` connection string in App Service Configuration instead of storing production credentials in source control.

Security points to mention in a report:

- Secrets should be stored in Azure App Service configuration or Azure Key Vault.
- The production SQL connection uses encrypted transport.
- ASP.NET Core Identity stores password hashes rather than plain text passwords.
- Role-based `[Authorize]` attributes restrict administrative functions.
- `[ValidateAntiForgeryToken]` protects form posts from CSRF attacks.
