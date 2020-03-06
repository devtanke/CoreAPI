FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN mkdir /app
ADD ./netcoreapp3.1 /app

EXPOSE 80

WORKDIR /app
ENTRYPOINT ["dotnet", "CoreAPI.dll"]
