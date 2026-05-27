# Backend — Localization

## Overview

Language is resolved from the `Accept-Language` request header. Supported cultures: `tr` (default), `en`.

Localization uses **JSON resource files** instead of `.resx`, loaded via ASP.NET Core's `IStringLocalizer` with the `Microsoft.Extensions.Localization` JSON provider.

---

## Resource Files

Located at `API/Resources/`:
- `tr.json`
- `en.json`

### File Format

```json
// API/Resources/tr.json
{
  "Validation.Required": "Bu alan zorunludur",
  "Validation.InvalidEmail": "Geçerli bir e-posta adresi giriniz",
  "Error.NotFound": "Kayıt bulunamadı",
  "Error.Unauthorized": "Yetkisiz erişim",
  "Email.Subject.Quotation": "Teklif Bildirimi - {0}",
  "Email.Body.Greeting": "Sayın {0},",
  "Request.NoPreviousPrice": "Henüz teklif değeri girilmemiş"
}
```

```json
// API/Resources/en.json
{
  "Validation.Required": "This field is required",
  "Validation.InvalidEmail": "Please enter a valid email address",
  "Error.NotFound": "Record not found",
  "Error.Unauthorized": "Unauthorized access",
  "Email.Subject.Quotation": "Quotation Notification - {0}",
  "Email.Body.Greeting": "Dear {0},",
  "Request.NoPreviousPrice": "No previous quotation price available"
}
```

---

## Configuration (`Program.cs`)

```csharp
// Register JSON-based localization
builder.Services.AddLocalization();

// Configure JSON resource path
builder.Services.Configure<LocalizationOptions>(options =>
{
    options.ResourcesPath = "Resources";
});

// Add request localization middleware
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "tr", "en" };
    options.SetDefaultCulture("tr");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

// In the pipeline
app.UseRequestLocalization();
```

> **Note:** To use JSON files with `IStringLocalizer`, install the `My.Extensions.Localization.Json` NuGet package (or equivalent JSON localization provider) and register it in `DependencyInjection.cs`.

---

## Usage in Handlers

```csharp
public class SendQuotationCommandHandler
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    // Inject via constructor

    // Usage
    var message = _localizer["Error.NotFound"];
    var subject = _localizer["Email.Subject.Quotation", requestNo];
}
```

---

## Keys to Localize (Minimum)

| Key | Purpose |
|---|---|
| `Validation.Required` | FluentValidation required field messages |
| `Validation.InvalidEmail` | FluentValidation email format messages |
| `Error.NotFound` | NotFoundException responses |
| `Error.Unauthorized` | UnauthorizedException responses |
| `Email.Subject.Quotation` | Quotation email subject line |
| `Email.Body.Greeting` | Quotation email greeting |
| `Request.NoPreviousPrice` | Displayed when a product has no price history |

Email subject and body are also rendered using the resolved language.
