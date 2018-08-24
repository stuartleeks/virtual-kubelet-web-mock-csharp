FROM microsoft/dotnet:2.1-sdk as build

WORKDIR /src
COPY ./vk-web-mock.csproj .
RUN dotnet restore ./vk-web-mock.csproj
COPY . .
RUN dotnet build -c Release ./vk-web-mock.csproj

RUN dotnet publish vk-web-mock.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "vk-web-mock.dll"]