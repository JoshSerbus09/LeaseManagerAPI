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
  - Simply open "LeaseManagerAPI.sln" in visual studio 2019, and run using IISExpress.
  - The launch page is a swagger UI built from the endpoints. 
  - This allows you to test API functionality independent of a user interface, or having to manually send requests via Postman (or equivalent)

# Logging:
  - Logs for the application can be found under "${Base Directory}/Logs/logfile.txt"
