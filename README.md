# TechStore Customer Web

ASP.NET Core MVC customer storefront with Tailwind CSS.

## Requirements

- .NET 10 SDK
- Node.js 20 or newer
- npm 10 or newer

## Run after cloning

```powershell
git clone <repository-url>
cd e-commerce-web-customer
dotnet run
```

The .NET build automatically runs `npm ci` when frontend dependencies are
missing or outdated, then generates `wwwroot/css/tailwind.css`.

Open the HTTP or HTTPS URL shown in the terminal. The default development URLs
are:

- `http://localhost:5132`
- `https://localhost:7124`

## Development

Run Tailwind in watch mode in one terminal:

```powershell
npm ci
npm run css:watch
```

Run ASP.NET Core in another terminal:

```powershell
dotnet watch run
```

Local configuration belongs in `appsettings.json` or
`appsettings.Development.json`. Use `appsettings.example.json` as the template;
do not commit credentials.
