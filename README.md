# cCoder.AppSecurity

`cCoder.AppSecurity` contains the App Security domain for the cCoder platform.

It owns application-level users, roles, privileges, and user-role links. SSO users and account tokens remain owned by `cCoder.Security`.

## Contents

- `src/cCoder.AppSecurity`
  The main library package published to NuGet.
- `src/AppSecurity.Web`
  The public API host for the App Security domain.
- `src/AppSecurity.HostedServices`
  The background/event host for App Security maintenance and event handlers.
- `src/cCoder.AppSecurity.Tests`
  Unit tests for the domain.
- `src/AppSecurity.AcceptanceTests`
  Acceptance tests for the Web host.
- `src/AppSecurity.HostedServices.AcceptanceTests`
  Acceptance tests for the Hosted Services host.

## Responsibilities

AppSecurity creates and maintains app-local security state:

- Apps and app domains.
- App users.
- Roles and privileges.
- User-role links.
- App usage analysis hosted service.
- App-local cleanup driven by app delete events.

AppSecurity consumes `cCoder.Security` account lifecycle events, resolves the target app from the event request domain, and creates or updates the related app-local user and default role links. It does not send account emails and does not own SSO users or tokens.

Security account events consumed:

- `security_account_registration_created`
- `security_account_registration_confirmed`
- `security_account_invitation_created`
- `security_account_invitation_accepted`
- `security_account_password_reset_requested`

## Build

```powershell
dotnet build src/cCoder.AppSecurity.sln -v minimal
```

## Test

```powershell
dotnet test src/cCoder.AppSecurity.sln -v minimal --no-build
```

## Local Configuration

The runnable hosts read local secrets from environment variables rather than committed config.

Before running `src/AppSecurity.Web` or `src/AppSecurity.HostedServices`, set:

- `ConnectionStrings__Core`
- `ConnectionStrings__SSO`
- `Settings__DecryptionKey`

The committed `appsettings.json` keeps these values blank so user or machine environment variables can supply them during local development.

`AppSecurity.Web` exposes `/Health` and the App Security API endpoints.

`AppSecurity.HostedServices` exposes `/Health` plus `/`, which returns a plain-text report of hosted background services and event listeners.

When testing against a local `cCoder.Security` checkout, pass:

```powershell
dotnet test src/cCoder.AppSecurity.sln -v minimal /p:UseLocalSecurity=true
```

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
