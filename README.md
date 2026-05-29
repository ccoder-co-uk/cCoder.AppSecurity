# cCoder.AppSecurity

`cCoder.AppSecurity` contains the App Security domain for the cCoder platform.

## Contents

- `src/cCoder.AppSecurity`
  The main library package published to NuGet.
- `src/AppSecurity.Web`
  The standalone web host for the App Security domain.
- `src/cCoder.AppSecurity.Tests`
  Unit tests for the domain.
- `src/AppSecurity.AcceptanceTests`
  Acceptance tests for the standalone host.

## Build

```powershell
dotnet build src/cCoder.AppSecurity.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.AppSecurity.sln -v minimal --no-build
```

## Local Configuration

The standalone web host reads local secrets from environment variables rather than committed config.

Before running `src/AppSecurity.Web`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

## Package

The NuGet package produced by this repository is:

- `cCoder.AppSecurity`

## Publishing

GitHub Actions is configured to publish the main package using NuGet trusted publishing.

Before the first publish, configure a trusted publishing policy on nuget.org for:

- Repository owner: `ccoder-co-uk`
- Repository: `cCoder.AppSecurity`
- Workflow file: `publish.yml`

The workflow also expects a `NUGET_USER` repository secret containing the nuget.org profile name used during trusted publishing login.
