# GKFashionApp

GKFashion MAUI app — simple storefront and admin UI.

## Build & Run (Android)

Prerequisites: .NET 8 SDK, Android SDK, Maui workloads

From project folder:

```bash
dotnet build -f net8.0-android
# or debug run
dotnet build -t:Run -f net8.0-android
```

## Notes
- Update `Services/DatabaseService.cs` `BaseUrl` to point to your API (localhost for dev with `adb reverse`, or your Render URL for production).
- Icons & splash are in `Resources/AppIcon` and `Resources/Splash`.
