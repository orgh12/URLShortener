# Url Shortener

## General Overview

The project is a basic web api that exposes three endpoints:  
### - /api/shorten:
Receives a url and expiry in days (how many days until the url expires) in the body.
Can receive a custom short_code in the query

### - /{short_code}
Redirect to the original url based on the short_code it receives from the route.

### /api/stats/{short_code}
Receives a short_code and returns the number of clicks (how many people used it).

## Technological Overview

To generate a **unique** short_code, I used a hash function of SHA256 then encoded it into base62.   
Then, I took the first six chars of that base62 as the short_code.
If the result already exists I add a "salt" to the original URL and generate a new one with it.  
In total, since we have 62 different options for each position and a total six chars in the short_code  
This leaves us with $62^6$ options or 56B.  
Since there are only 1.2B different URLs, this should cover us


## How to Build The Project

Very Easy!  
Use the provided docker-compose file to build a container running the App.  
First, clone this repo to a local directory.  
Install Docker Desktop on your machine.
In the project directory run: 
```
docker-compose up --build
```
And that`s It!
  
