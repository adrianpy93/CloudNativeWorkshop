@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' = {
  name: take('cosmosdb-${uniqueString(resourceGroup().id)}', 44)
  location: location
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    disableLocalAuth: true
  }
  kind: 'GlobalDocumentDB'
  tags: {
    'aspire-resource-name': 'cosmosdb'
  }
}

resource cartdb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-08-15' = {
  name: 'cartdb'
  location: location
  properties: {
    resource: {
      id: 'cartdb'
    }
  }
  parent: cosmosdb
}

resource carts 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-08-15' = {
  name: 'carts'
  location: location
  properties: {
    resource: {
      id: 'carts'
      partitionKey: {
        paths: [
          '/pk'
        ]
        kind: 'Hash'
      }
    }
  }
  parent: cartdb
}

output connectionString string = cosmosdb.properties.documentEndpoint

output name string = cosmosdb.name