# syntax=docker/dockerfile:1
# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md


FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-noble-aot AS build
WORKDIR /source

# Build the app

# dotnet build "./releasesapi.csproj" -c Release -o /app/build

COPY --link . .
RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    dotnet publish -o /app releasesapi.csproj \
        && rm /app/*.dbg /app/*.Development.json

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-preview-noble-chiseled
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./releasesapi"]


# docker run --rm -it -p 8000:8080 -e ASPNETCORE_HTTP_PORTS=8080 releasesapi
