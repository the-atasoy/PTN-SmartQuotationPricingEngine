# Backend — Email Service

## Overview

Email sending is implemented using **MailKit**. In development, emails are sent to **Mailpit** (a local SMTP testing server) — no real emails leave the system.

---

## SMTP Configuration

Config lives in `appsettings.json`:

```json
"Email": {
  "SmtpHost": "localhost",
  "SmtpPort": 1025,
  "FromAddress": "noreply@piton.com.tr",
  "FromName": "PITON Technology"
}
```

In Docker Compose, `SmtpHost` becomes `mailpit` (the service name).

---

## Email Template

The quotation email body is an HTML template containing:

- Customer name
- Request number
- Sent date
- Currency
- Product table (name, quantity, unit price, discount, line total)
- Grand total
- Company branding (PITON Technology name/address)

Subject and greeting are localized using `IStringLocalizer` — sent in the customer's language or default `tr`.

---

## Error Handling

Email sending happens **outside** the database transaction in the `SendQuotation` flow. If email delivery fails:
- The failure is logged.
- The database transaction is **not** rolled back.
- The quotation status remains `Sent`.

---

## Development Testing

Mailpit provides a web UI to inspect sent emails:
- SMTP endpoint: `localhost:1025`
- Web UI: `localhost:8025`
