FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
# Copy the config file 
COPY NLog.config .

EXPOSE 80

# Where to look for the metric data
RUN mkdir /data
VOLUME /data

# Where to store the snapshots
RUN mkdir /snapshots
VOLUME /snapshots

# Where to store the log files
RUN mkdir /logs
VOLUME /logs

# Where to look for the configuration
RUN mkdir /config
VOLUME /config

# define the environment variables
ENV DIR_NAME /data
ENV FILE_TYPE *.csv
ENV VMNAME_PATTERN vmpattern
ENV CONFIG_FILE /config/config.txt
ENV SNAPSHOT_PATH /snapshots
ENV VMSTATSGUI_WEBSERVER_URL docker01.dest.internal:5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY vmstats-akka.sln ./
COPY vmstats/*.csproj ./vmstats/
COPY vmstats-lang/*.csproj ./vmstats-lang/
COPY webserver/*.csproj ./webserver/

RUN dotnet restore
COPY . .

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