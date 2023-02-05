FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["AzureCliCredentialContainer.csproj", "./"]
RUN dotnet restore "./AzureCliCredentialContainer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AzureCliCredentialContainer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AzureCliCredentialContainer.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get clean && apt-get update
RUN apt install curl -y
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureCliCredentialContainer.dll"]