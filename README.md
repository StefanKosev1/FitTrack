# FitTrack

## Run The Project

From the repository root:

```bash
cd /Users/stefankosev/RiderProjects/FitTrack
dotnet build --no-restore src/FitTrack.Web/FitTrack.Web.csproj -v:minimal
dotnet run --no-build --project src/FitTrack.Web/FitTrack.Web.csproj --launch-profile http
```

Open the app at:

```text
http://localhost:5149
```

If you already built the project and want to skip rebuilding:

```bash
cd /Users/stefankosev/RiderProjects/FitTrack
dotnet run --no-build --project src/FitTrack.Web/FitTrack.Web.csproj --launch-profile http
```

If dependencies need to be restored first:

```bash
cd /Users/stefankosev/RiderProjects/FitTrack
dotnet restore src/FitTrack.Web/FitTrack.Web.csproj
dotnet build src/FitTrack.Web/FitTrack.Web.csproj -v:minimal
dotnet run --no-build --project src/FitTrack.Web/FitTrack.Web.csproj --launch-profile http
```

## Run The Tests

From the repository root:

```bash
cd /Users/stefankosev/RiderProjects/FitTrack
dotnet test tests/FitTrack.Core.Tests/FitTrack.Core.Tests.csproj
```

Successful output should include:

```text
Passed!  - Failed: 0, Passed: 5, Skipped: 0, Total: 5
```
