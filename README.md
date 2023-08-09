# Example WFE + FB

Based on ASP.NET + React template

## Requirements

1) MSSQL
2) Node 16.x
3) .NET 7

## Getting started

1) Configure MSSQL for WFE
2) Set connection string in appsettings.json
3) Start solution

## Enable https

1) In Program.cs uncomment `UseHsts` and `UseHttpsRedirection`
2) In ClientApp/.env set `HTTPS=true`
3) In Sample.csproj of SpaProxyServerUrl attribute set https instead of http