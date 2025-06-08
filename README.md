# Url Shortener

## General Overview

The project is a basic web api that exposes 2 endpoints:  
### - /api/shorten:
Receives a url in a string format and returns a shortened url.  

### - /{short_code}
Redirect to the original url based on the short_code it receives from the route.

## Technological Overview

To generate a **unique** short_code, I used a hash function of SHA256 and truncated the first six chars.  
This makes the code so unique that if you generate 4e13 URLs, there is a 1% chance for a collision to happen.  
To put that into perspective, that are only about 1.2e9 different URLs out there

## How to Build The Project

Install an SQL server and create a database on it.  
Pull the code from this repo into a project and install the following packages:

```
Swashbuckle.AspNetCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
```
Create a connection string to your database and update it in appsettings.json  
On your project run
```
dotnet ef database update
```

Finally, run the project!

  
