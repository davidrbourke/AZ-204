# ServiceBusTopicFilters

- One Publisher
- Receiver sub-a has sql filter expression on Application Property MsgProp=1
- Receiver sub-b has no filters, reads all messages

## Azure CLI Commands
Creates topic and gets connection string. Connections string should be updated in the appSettings.json for each project.
There are separate different connection strings for the publisher and subscriber.

`az group create --name az204sbt --location UKSouth`

`az servicebus namespace create --name az204sbtdemo --resource-group az204sbt --location UKSouth --sku Standard`

`az servicebus topic create --name dbk-topic-demo --resource-group az204sbt --namespace-name az204sbtdemo`

`az servicebus topic subscription create --name sub-a --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo`

`az servicebus topic subscription create --name sub-b --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo`

`az servicebus topic subscription rule create --name filter-msgprop-1 --namespace-name az204sbtdemo --resource-group az204sbt --subscription-name sub-a --topic-name dbk-topic-demo --filter-sql-expression MsgProp=1`

`az servicebus topic authorization-rule create --name demo-publisher --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo --rights Send`

`az servicebus topic authorization-rule create --name demo-subscriber --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo --rights Listen`

`az servicebus topic authorization-rule list --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo`

`az servicebus topic authorization-rule keys list --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo --name demo-publisher`

`az servicebus topic authorization-rule keys list --namespace-name az204sbtdemo --resource-group az204sbt --topic-name dbk-topic-demo --name demo-subscriber`