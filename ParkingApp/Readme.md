# ParkingApp

## Running locally

The app is based on .Net 8, the creator will assume you have it installed.

1. Clone the repository
2. Navigate to the ParkingApp directory
3. Run `dotnet run` in the terminal
4. Open a browser and navigate to `http://localhost:5115/swagger/index.html` to see the Swagger UI.
5. Use the Swagger UI to interact with the API.

Alternatives: 
1. Open the solution in Visual Studio and run the project.
2. Once above point 3 is done, one could use Postman or similar tool to interact with the API.

## In-memory database
The app uses in-memory database for the sake of simplicity.
This means that there is no **persistence** and all data will be lost when the app is stopped.

For enything more than a demo, a real database have to be used.
This would be achieved by replacing the in-memory database provider with a real database provider that handles, for example MS SQL Server.

With the introduction of a real database, entity could be decorated with at least `Key` attribute to define the primary key.
For the sake of in-memory testing this was deemed to be unnecessary.

## Carpark size
The carpark size is set in the `appsettings.json` file.
It is set to modest size of 10 parking spots for the sake of simplicity.

## Considerations, improvements and assumptions

### Concurrency
The app does not handle much concurrency issues, for example two users trying to park the last spot at the same time.
This could be solved by using transactions and/or locks.

### Registration number validation
The app does not validate the registration number format.
This could be solved by using regular expressions. The consideration would be to validate all possible formats, i.e. all European countries.
However, this would be a never-ending task as new formats could be introduced and limiting to certain countries would not be user-friendly.

### Security
The app does not have any security features, such as authentication and authorization.
Moreover, it is set up to run only via http and not https.
This is done, yet again, for the sake of simplicity. The creator of this app spend too much time faffing around with certificates for some local demos and is not keen on doing it again.

In a real-world scenario, the app should be secured using HTTPS and proper authentication and authorization mechanisms.

Moreover, the introduction of an id of parking record would be better than using just the registration number.
This would prevent from someone trying to exit a vehicle that is not theirs by knowing the registration number.

### Vehicle types
This is creators' nit-picking but the current vehicle types are really bad for real-life scenario.
One could consider BMW 5 series to be "Large" vehicle while Ford F150 would share the same category.
As anyone could see these are two very different vehicles in terms of size.

### Logging
Default logs are not that great, the improvement would be to introduce some better logging system like Serilog. Change of output from just the console to perhaps both console and the file would also improve things - in the unfortunate event of app crashing logs would be preserved.

### Parking charge
At the moment, the double will return something obscure like `0.300000000000000` which isn't a real-life money format.
This would have to be addressed to make it more user-friendly and realistic.
