FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar todo y publicar
COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o /out

# Contenedor final superligero
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app
COPY --from=build /out .

# Configurar el puerto dinámico de Railway de forma segura
ENV ASPNETCORE_URLS=http://+:8080
CMD ["sh", "-c", "./LOGIN --urls http://0.0.0.0:$PORT"]