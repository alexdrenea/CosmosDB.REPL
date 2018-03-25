# CosmosDB REPL

Simple console application that connects to a CosmosDB graph account (DocDB comming soon) and allows you to run simple queries and asses performance and usage of your commands.
The tool can be used for quick experimentation and evaluation of queries but can also act as a playground for updating your CosmosDB client libraries, preview nuget package upates and so on.


## Windows

- Clone this repository
- Open in Visual Studio.
- Update the appsettings.json file with your cosmosDB connection details (you can add more than one connection). 
- Run the application.

## Linux

In case you haven't done so already, install `dotnet` on the machine. Follow steps [here](https://www.microsoft.com/net/learn/get-started/linux/ubuntu16-04) for assistance.

- Clone the repository.
- Update the appsettings.json file with your cosmosDB connection details (you can add more than one connection). 
- Navigate to the project folder (`cd CosmosDB.REPL/CosmosDB.REPL`)
- ```dotnet run``` to run the app.