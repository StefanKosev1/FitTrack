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

From the repository root, run all unit and integration tests:

```bash
cd /Users/stefankosev/RiderProjects/FitTrack
dotnet test FitTrack.slnx
```

Run only the core unit tests:

```bash
dotnet test tests/FitTrack.Core.Tests/FitTrack.Core.Tests.csproj
```

Run only the web integration tests:

```bash
dotnet test tests/FitTrack.Web.Tests/FitTrack.Web.Tests.csproj
```

After dependencies have already been restored, use `--no-restore` for faster runs:

```bash
dotnet test FitTrack.slnx --no-restore
```

Successful full-suite output should report:

```text
Core tests: 26 passed
Integration tests: 3 passed
Total: 29 passed
```
