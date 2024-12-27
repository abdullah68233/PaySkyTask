# Employment System API

This is the Employment System API that enables employers to manage job vacancies and applicants to search and apply for these positions.

## Prerequisites

1. **.NET Core SDK**: Install the latest version of .NET Core SDK from [Microsoft's official site](https://dotnet.microsoft.com/download).
2. **SQL Server**: Ensure a SQL Server instance is running locally or accessible remotely.
3. **Swagger UI**: Swagger is integrated to test the API endpoints directly from the browser.
4. **Postman or any API Testing Tool (optional)**: Use for additional testing.

---

## Setup Instructions

just un zip the project and open it in visual studio 2022 or above  and run the project

Open the appsettings.json file in the project root.
Locate the ConnectionStrings section and update the DefaultConnection value with your SQL Server details:
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;MultipleActiveResultSets=true"
}


Run Database Migrations:

Open a terminal in the project folder.

Run the commands in the database migration folder:


