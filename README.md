# InfoTrackSEO
InfoTrack Tech Test
Developed By: Chamal Pulasthi | Completed On: 12/03/2021

The Challenge

The main challenge I saw was to extract the data from the static search pages without sacrificing the cleanliness of the code. 

The approach

1)	Separated the solution into few projects/layers 
-	Web  - the .net core web api project
-	Domain – layer for more concrete business logic and services
-	Core – abstract layer for domain models
-	Tests – unit tests
2)	Defined the search engine base addresses in the appsettings.json.
3)	Implemented a service type pattern to decouple the search engine specific extraction logic.
4)	Defined two separate http clients and Injected them into the search services based on the service type.
5)	Injected a service factory into the search controller which resolves the service using a service provider.
6)	Each search service utilizes a common set of functions to help with their specific data extractions. These functions were moved to a helper class.
7)	Covered the service helper common extraction logic with unit tests in the tests project.
8)	Added a simple UI and used simple MVVM framework (Knockout Js) to bind the controls and to map the viewmodel.
9)	Knockout js was also used in the client-side validations.
10)	Ajax call to the API delivers both summarized and the detailed results to the UI.

What more can we do?

What I really wanted to do (unfortunately the time did not permit) was to implement a repository pattern using the EF core with a sqlLite DB and log the handled search requests into a simple table so that we can show a history of results. Assuming the results change (although the static results probably never change) we can show the history in a chart which will look really nice. 😊
