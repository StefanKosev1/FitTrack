# FitTrack

## Run The Project

From the repository root:

```bash
dotnet build src/FitTrack.Web/FitTrack.Web.csproj
dotnet run --project src/FitTrack.Web/FitTrack.Web.csproj --launch-profile http
```

Open the app at:

```text
http://localhost:5149
```

If you already built the project and want to skip rebuilding:

```bash
dotnet run --no-build --project src/FitTrack.Web/FitTrack.Web.csproj --launch-profile http
```
