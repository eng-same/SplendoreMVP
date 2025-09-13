# Splendore Store — MVP

> **A minimum viable product (MVP)** for a larger Splendore Store system: a digital sales platform that will eventually integrate a desktop staff application (SPO) and additional capabilities.

## Project Overview

Splendore Store System is an operational prototype that demonstrates the core workflows of an integrated e‑commerce and store‑management solution. The MVP focuses on the essential features required to run an online clothing & accessories store while providing a basic synchronization channel between a backend/desktop staff application and the public web store through a RESTful API. The system supports Arabic and English user interfaces to reach a broader audience.

## نبذة عن المشروع (بالعربية)

مشروع "Splendore Store System" هو نموذج أولي عملي (MVP) لنظام مبيعات رقمي يدعم تكامل العمليات بين بيئة سطح المكتب للموظفين والمتجر الإلكتروني للعملاء. يهدف إلى توفير تجربة مستخدم فعالة لبيع الملابس والإكسسوارات مع مزامنة أساسية في الوقت الحقيقي بين واجهات العميل والموظف عبر API، ودعم اللغتين العربية والإنجليزية.

## MVP Scope (What is included)

### Users in the MVP

* **Store staff / Warehouse manager (Desktop app)**

  * Add & update products
  * Track inventory
  * Process orders from the web store

* **Customer (Web store)**

  * Browse products
  * Register and login
  * Add products to cart and checkout (checkout requires login)
  * Choose payment method (represented/simulated in MVP)

### Core features

1. **Product management (Desktop)**

   * Add product (name, price, images, stock)
   * Categories (e.g., clothing, accessories, shoes)
   * Product options (size, color)
   * Real‑time sync to web store via API

2. **Product browsing & purchasing (Web store)**

   * Responsive UI in Arabic & English
   * Sections: Featured, Best sellers, Discounts
   * Product details and selectable options
   * Shopping cart and account flow
   * Checkout with address, payment method (simulated), pickup or shipping

3. **Customer accounts & auth**

   * Sign up (name, email, password)
   * Login via email & password
   * Inventory update after sale

4. **Order management (Desktop)**

   * New orders appear to staff after purchase
   * Order status lifecycle: e.g., "Preparing" → "Ready for pickup / Shipping"
   * Inventory updates after orders processed

## خارطة الطريق بعد الـ MVP (Out-of-scope for MVP / Deferred)

* Returns & refunds system
* Advanced sales reports & analytics
* Low‑stock notifications and alerts
* Real (production) payment gateway integration
* On‑site POS (point of sale) terminal integration

## Objectives of the MVP

* Validate the core online sales flows (browse → buy)
* Test integration & synchronization between management backend (desktop) and web storefront
* Verify user experience from product discovery to order completion

## Architecture & future vision

The long‑term Splendore solution will consist of at least three main parts:

1. **Desktop staff application (SPO)** — a desktop client for store employees to manage products, inventory and orders. (Planned as a separate deliverable.)
2. **Web storefront** — the customer‑facing website where browsing, account management and purchases occur (this repo contains the MVP web app).
3. **Central API / Integration layer** — a secure RESTful API that synchronizes data (products, inventory, orders) between the desktop client and the web store and will later serve mobile apps, POS terminals, and other channels.

The MVP implements the web storefront and the API endpoints required for basic synchronization. The desktop client is represented conceptually and its required API contracts are implemented so a desktop app can be developed and connected later.

## Suggested Tech Stack (MVP)

* Backend: **ASP.NET Core** (Web API + MVC) with Entity Framework Core
* Database: SQL Server / SQLite (for development)
* Frontend: Razor Views / Bootstrap 5 (responsive, RTL support for Arabic)
* Auth: ASP.NET Identity (email + password)
* API: RESTful endpoints for products, categories, orders, auth
* Desktop client (future): .NET desktop (WPF / WinForms) or equivalent

> These are the technologies used in the current MVP and the recommended choices for future desktop and integrations.

## Getting started (local development)

> The steps below are a general guide — adapt paths, connection strings and commands to your environment.

1. Clone the repo:

```bash
git clone <repo-url>
cd splendore-mvp
```

2. Configure settings:

* Edit `appsettings.json`  for the database connection string and any keys.

3. Apply migrations & seed data:

```bash
dotnet ef database update
or
Update-base
```

4. Run the web app:

```bash
dotnet run
# then open https://localhost:5001 or http://localhost:5000
```

5. (Optional) Run seeders to populate sample categories, products and images. The repo may include a `SeedProductsAsync` helper — use it for quick demo content.

## API (implemented in this MVP)

> **Note:** the full REST API layer is *not yet implemented*. The MVP currently exposes a small set of HTTP endpoints used by the web app. These are controller actions (MVC) rather than a full standalone API surface.

### Currently available endpoints

* **GET** `Cart/AddItem?productId={id}&qty={qty}&redirect={0|1}`

```csharp
public async Task<IActionResult> AddItem(int productId, int qty = 1, int redirect = 0)
{
    var cartCount = await _cartRepo.AddItem(productId, qty);
    if (redirect == 0)
        return Ok(cartCount);
    return RedirectToAction("GetUserCart");
}
```

* **GET** `Cart/GetTotalItemInCart`

```csharp
public async Task<IActionResult> GetTotalItemInCart()
{
    int cartItem = await _cartRepo.GetCartItemCount();
    return Ok(cartItem);
}
```

* **POST** `Admin/Admin/QuickUpdateStock?id={id}&stock={stock}` (protected by antiforgery)

```csharp
[ValidateAntiForgeryToken]
public async Task<IActionResult> QuickUpdateStock(int id, int stock)
{
    var product = await _productRepo.GetProductById(id);
    if (product == null) return NotFound();

    product.StockQuantity = stock;
    await _productRepo.Update(product);

    return Json(new { success = true, id = product.Id, stock = product.StockQuantity });
}
```

* **POST** `Admin/Admin/QuickUpdatePrice?id={id}&price={price}` (protected by antiforgery)

```csharp
[ValidateAntiForgeryToken]
public async Task<IActionResult> QuickUpdatePrice(int id, decimal price)
{
    var product = await _productRepo.GetProductById(id);
    if (product == null) return NotFound();

    product.Price = price;
    await _productRepo.Update(product);

    return Json(new { success = true, id = product.Id, price = product.Price });
}
```

### What's missing (planned)

* Public REST endpoints for products, categories and product search (`/api/products`, `/api/categories`)
* API endpoints for orders (`POST /api/orders`, `GET /api/orders/{id}`) with proper authentication
* Auth token endpoints for desktop client synchronization
* API versioning, rate limiting and API keys for desktop/third-party clients

## Contact

For questions or collaboration, contact the project owner or maintainers (eng.sami.cyber@gmail.com).

---

### ملاحظات ختامية (Arabic notes)

هذا المستند يصف نطاق النسخة الأولية (MVP) فقط — هو جزء من رؤية أكبر لنظام متكامل للإدارة والبيع عبر قنوات متعددة (سطح مكتب، المتجر الإلكتروني، ونقاط بيع مستقبلية). التصميم والواجهات الحالية مهيأة لتوسيع الوظائف لاحقًا دون إعادة تصميم جذري للبنية الأساسية.
