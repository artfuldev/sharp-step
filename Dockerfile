ARG APP_HOME=/usr/app/sharp-step

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS core
WORKDIR $APP_HOME/SharpStep.Core
COPY ./SharpStep.Core .
RUN dotnet build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS solvers
COPY --from=core $APP_HOME/SharpStep.Core $APP_HOME/SharpStep.Core
COPY ./SharpStep.Solvers $APP_HOME/SharpStep.Solvers
WORKDIR $APP_HOME/SharpStep.Solvers
RUN dotnet build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test
COPY --from=solvers $APP_HOME/SharpStep.Core $APP_HOME/SharpStep.Core
COPY --from=solvers $APP_HOME/SharpStep.Solvers $APP_HOME/SharpStep.Solvers
COPY ./SharpStep.Tests $APP_HOME/SharpStep.Tests
WORKDIR $APP_HOME/SharpStep.Tests
RUN dotnet test

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
COPY --from=test $APP_HOME/SharpStep.Core $APP_HOME/SharpStep.Core
COPY --from=test $APP_HOME/SharpStep.Solvers $APP_HOME/SharpStep.Solvers
COPY ./SharpStep.Console $APP_HOME/SharpStep.Console
WORKDIR $APP_HOME/SharpStep.Console
RUN dotnet publish -c Release -o $APP_HOME/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=publish $APP_HOME/out $APP_HOME/out
WORKDIR $APP_HOME/out
ENTRYPOINT dotnet SharpStep.Console.dll
