FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env
WORKDIR /src

COPY ["Demo.Interfaces/", "Demo.Interfaces/"]
COPY ["Demo.Services", "Demo.Services/"]
COPY ["Demo.Functions/", "Demo.Functions/"]

RUN dotnet restore "Demo.Interfaces/Demo.Interfaces.csproj"
RUN dotnet restore "Demo.Services/Demo.Services.csproj"
RUN dotnet restore "Demo.Functions/Demo.Functions.csproj"

COPY . .

WORKDIR "/src/"
RUN dotnet build "Demo.Interfaces/Demo.Interfaces.csproj" -c Release -o /Demo.Functions
RUN dotnet build "Demo.Services/Demo.Services.csproj" -c Release -o /Demo.Functions
RUN dotnet build "Demo.Functions/Demo.Functions.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Demo.Functions/Demo.Functions.csproj" -c Release -o /app

WORKDIR /app
COPY --from=publish /app .
RUN cd /app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:3.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]