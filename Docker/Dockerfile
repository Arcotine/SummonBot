
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk AS build-env
WORKDIR /app
COPY ./SummonBot.csproj .
RUN dotnet restore

COPY ./ .
RUN dotnet build -c Release
FROM build-env AS publish

RUN dotnet publish -c Release -o /publish

FROM base AS final
LABEL author="Ricky Sosa"
WORKDIR /app
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "SummonBot.dll"]