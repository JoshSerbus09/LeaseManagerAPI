# LeaseManagerAPI
  - CRUD Lease API w/ Sqlite Data store
  
# Dependencies:
  - Visual Studio 2019
  - .Net Core 3.1
  - .Net Standard 5

# Configure:
  - The base implementation is a Sqlite data store, but there is an `ILeaseDao` that can be implemented to support another datasource. 
  - Currently the datasource is built on startup, but this can be modified to inject the service dependencies at run-time.
  
# Run:
  - open solution in Visual Studio 2019 and run in IISExpress
  OR
  - open a command line at the root directory and run the command 'dotnet LeaseManagerApi

# Logging:
  - Logs for the application can be found under "${Base Directory}/Logs/logfile.txt"
