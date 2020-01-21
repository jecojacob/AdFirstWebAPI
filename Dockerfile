FROM mcr.microsoft.com/dotnet/core/aspnet

WORKDIR /app
COPY /bin/Debug/netcoreapp3.1/publish .

ENTRYPOINT ["dotnet" , "AdviceFirstApi.dll"]