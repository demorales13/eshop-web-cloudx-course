# Instance Identifier Configuration

## Overview
Each App Service instance displays a visual badge with its name in the header to differentiate between multiple running instances.

## Configuration

### Local Development
Default value is set in `src/Web/appsettings.json`:
```json
"InstanceName": "Local"
```

### Azure App Services
Configure the `InstanceName` setting as an Application Setting (environment variable) in each App Service:

#### Option 1: Azure Portal
1. Go to App Service → Configuration → Application settings
2. Add new setting:
   - **Name**: `InstanceName`
   - **Value**: Choose one of:
     - `Production` (red badge)
     - `Staging` (yellow/warning badge)
     - `Secondary` (blue/info badge)
     - Any custom name (gray badge)

#### Option 2: Azure CLI
```bash
# For eshop-cloudx (Production)
az webapp config appsettings set --name eshop-cloudx --resource-group <your-rg> --settings InstanceName="Production"

# For eshop-web-cloudx (Staging)
az webapp config appsettings set --name eshop-web-cloudx --resource-group <your-rg> --settings InstanceName="Staging"

# For eshop-web-cloudx-secondary (Secondary)
az webapp config appsettings set --name eshop-web-cloudx-secondary --resource-group <your-rg> --settings InstanceName="Secondary"
```

#### Option 3: GitHub Actions Workflow (Recommended)
Add the setting during deployment in `.github/workflows/deploy-web-multi-appservice.yml`:

```yaml
- name: Deploy to eshop-cloudx
  uses: azure/webapps-deploy@v3
  with:
    app-name: 'eshop-cloudx'
    slot-name: 'Production'
    package: .

- name: Set InstanceName for eshop-cloudx
  run: |
    az webapp config appsettings set --name eshop-cloudx --resource-group <your-rg> --settings InstanceName="Production"
```

## Badge Colors
- **Production** → Red badge (`badge-danger`)
- **Staging** → Yellow badge (`badge-warning`)
- **Secondary** → Blue badge (`badge-info`)
- **Local** or other → Gray badge (`badge-secondary`)

## Visual Result
The instance name will appear next to the eShop logo in the header:

```
[eShop Logo] [Production]  <- Red badge
[eShop Logo] [Staging]     <- Yellow badge
[eShop Logo] [Secondary]   <- Blue badge
```
