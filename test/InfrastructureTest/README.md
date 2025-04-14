# Infrastructure Test Project


## Store API key in secrets
use nuget package `dotnet add package Microsoft.Extensions.Configuration.UserSecrets`

```bash
dotnet user-secrets init --project path/to/TestProject.csproj
dotnet user-secrets set "WeatherApi:ApiKey" "your-actual-api-key" --project path/to/TestProject.csproj
```