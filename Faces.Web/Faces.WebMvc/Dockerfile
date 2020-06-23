#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Messaging.InterfacesConstants/Faces.Web/Faces.WebMvc/Faces.WebMvc.csproj", "Messaging.InterfacesConstants/Faces.Web/Faces.WebMvc/"]
RUN dotnet restore "Messaging.InterfacesConstants/Faces.Web/Faces.WebMvc/Faces.WebMvc.csproj"
COPY . .
WORKDIR "/src/Messaging.InterfacesConstants/Faces.Web/Faces.WebMvc"
RUN dotnet build "Faces.WebMvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Faces.WebMvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Faces.WebMvc.dll"]