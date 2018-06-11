#FROM microsoft/aspnetcore-build:2.0.0 AS build-env
#WORKDIR /app

# Copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore

# Copy everything else and build
#COPY . ./
#RUN dotnet publish -c Release -o out

# Build runtime image
#FROM microsoft/aspnetcore:2.0.0
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "cad_restapi.dll"]





FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
ENV DIR_NAME temp
ENV FILE_TYPE *.csv
ENV VMNAME_PATTERN vmpattern


FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY vmstats-akka.sln ./
COPY transforms/*.csproj ./transforms/
COPY vmstats/*.csproj ./vmstats/
COPY vmstats-lang/*.csproj ./vmstats-lang/
COPY webserver/*.csproj ./webserver/

RUN dotnet restore
COPY . .
WORKDIR /src/transforms
RUN dotnet build -c Release -o /app

WORKDIR /src/vmstats
RUN dotnet build -c Release -o /app

WORKDIR /src/vmstats-lang
RUN dotnet build -c Release -o /app

WORKDIR /src/webserver
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webserver.dll"]