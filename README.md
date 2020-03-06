# REST-API in dotnet core 3.1

This is a quick and dirty example of an api server written in dotnet core 3.1. You will need a mysql database to authenticate users. You can find a dump in the dump-folder of this repository.

## How to get it run

1. Simply install a MySql-Server, create a database and apply the dump.
2. Clone this repo (offcourse)
3. Edit the connection string, located in `Utils/Database/MySql`.
```
string connection = "Server=localhost;Database=mydb;Uid=root;Pwd=root;SslMode=Preferred;";
```
4. Run the server.

### Login credentials:
```
Username: user4211
Password: demo
```

### Routes

You can test the routes with postman. A file to import is located in the postman-folder.
