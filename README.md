## CS5721-VH
CS5721 - Vaccine Hub

#### Overview

With Vaccine Hub, customers can search for and book vaccine products (COVID vaccine) at any hospital. Users can access the platform in two ways - as customers or as administrators. Administrators can add or update products, centers, and inventory. Customers can also be created and viewed by administrators. The customer can register, book an appointment by credit card payment at any center for any day within the next 7 days, and cancel it before the appointment date. After the first dose is successful, customers can book for the second dose. The second dose can be taken at any time between the minimum and maximum interval between the first and second dosage. A customer cannot take more than one vaccine of a particular type. Customers will get the certificate generated next day. Future scope involves vaccine administrators conducting vaccinations at customer’s homes and allowing the customer to choose which option they prefer, managing storage and supply to center and manufacturer details.

#### Business scenario

When government vaccination programmes will become less frequent after a pandemic, our company must provide a system that permits customers to order covid vaccines. The system checks the available vaccines in the database based on the locations and stocks, then provides the customer with the results. The agents also manage and redistribute the stock among the different centers. The certificates are generated the day after the appointment date and sent to the customers the next day.

#### Ports and Services

|Service|Port|Notes|
|-|-|-|
|VaccineHub|5001|Vaccine Hub Restful Services|
|Mock Services|5010|Vaccine Hub Third Party Mock Service|

#### Prerequisites

Install the following:
1. [JetBrains Rider](https://www.jetbrains.com/rider/)
2. [`dotnet` SDK](https://dotnet.microsoft.com/download/dotnet-core/5.0). *Version 5.0 is currently required!*

#### Process

1. Clone repository:
    - [Vaccine Hub](https://github.com/venkateshprasadem/CS5721-VH)
2. `dotnet build VaccineHub.sln` in the CS5721-VH directory (*Try to build in Release Version*)
3. Within the CS5721-VH repository, Run the Vaccine Web service `dotnet VaccineHub.Web/bin/Debug/net5.0/VaccineHub.Web.dll`
3. Within the CS5721-VH repository, Run the Mock service `dotnet ./VaccineHub.MockServices/bin/Debug/net5.0/VaccineHub.MockServices.dll`
6. [Navigate to the swagger page](http://localhost:5001/swagger/index.html) in a browser of choice.

### Client API Authentication

Once project is built and running, we will need to authenticate using basic authentication in order to make API requests. 
In memory database, we are seeding Admin Api User with username - admin@studentmail.ul.ie and password - admin. 
If we plug in another database, We need to perform [database migration activity](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) and then we need to call the ApiUser POST endpoint and register Admins and Customer.

