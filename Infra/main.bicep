// 1. Parámetros 
param webAppName string = 'WAPPCUSAPPD0210' 
param existingPlanName string = 'ASPLCUSAPPD01'
param location string = resourceGroup().location

// 2. Le decimos a Bicep que este App Service Plan YA EXISTE (usando la palabra clave 'existing')
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' existing = {
  name: existingPlanName
}

// 3. Creamos la Web App y la metemos dentro del Plan existente
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      alwaysOn: true // Mantiene el microservicio despierto (requiere plan Basic o superior)
      linuxFxVersion: 'DOTNETCORE|8.0' // Le dice a Azure que prepare una máquina Linux con .NET 8
    }
  }
}