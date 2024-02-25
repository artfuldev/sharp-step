ARG APP_HOME=/usr/app/sharp-step

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR $APP_HOME/SharpStep.Core
COPY ./SharpStep.Core .
RUN dotnet build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test
COPY --from=build $APP_HOME/SharpStep.Core $APP_HOME/SharpStep.Core
COPY ./SharpStep.Tests $APP_HOME/SharpStep.Tests
WORKDIR $APP_HOME/SharpStep.Tests
RUN dotnet test

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
COPY --from=build $APP_HOME/SharpStep.Core $APP_HOME/SharpStep.Core
COPY ./SharpStep.Console $APP_HOME/SharpStep.Console
WORKDIR $APP_HOME/SharpStep.Console
RUN dotnet publish -c Release -o $APP_HOME/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=publish $APP_HOME/out $APP_HOME/out
WORKDIR $APP_HOME/out
ENTRYPOINT dotnet SharpStep.Console.dll