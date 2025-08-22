# ASP.NET Ecommerce API
A RESTful ASP.NET Core Web API demo for an **e-commerce platform**. It handles products, carts, orders, payments, and user management with authentication and role-based authorization.

## 🚀 Key Features

- **Products:** Full CRUD operations, with admin-only endpoints for creating, updating, and deleting products.  
- **Cart:** Add, remove, clear items, and calculate total cost for checkout.  
- **Orders:** Manage user orders and get order history and details.  
- **Payment Integration:** Stripe checkout session and webhook handling for verification and order creation.  
- **User Management:** JWT authentication and role-based authorization for Admin and User access.  
- **Additional Features:** Pagination, API versioning for maintainability and rate limiting to manage traffic.

## 🛠️ Technologies Used
- ASP.NET Core
- Entity Framework Core with SQL Server
- Unit and Integration Tests with xUnit
- CI with GitHub Actions
- Controller → Service → Repository pattern
- JWT Authentication
- API Versioning
- Rate Limiting
- Swagger/OpenAPI

## 📚 Project Source
This project is based on the roadmap.sh E-commerce API project:  
[https://roadmap.sh/projects/ecommerce-api](https://roadmap.sh/projects/ecommerce-api)


## 🌐 Live Demo
Access the live API via Swagger (the server may take a few seconds to start if idle):  
[https://wz-ecommerce-api.runasp.net/swagger/index.html](https://wz-ecommerce-api.runasp.net/swagger/index.html)


## 📡 API Endpoints

<details>
<summary>🛒 Cart Endpoints</summary>
  
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/cart` | Get all cart items from user's cart | ✅ |
| `POST` | `/cart` | Add product to user's cart | ✅ |
| `PUT` | `/cart/{cartItemId}` | Update cart item quantity or remove if 0 or negative | ✅ |
| `DELETE` | `/cart/clear` | Remove all items from cart | ✅ |

</details>
  
<details>
<summary>📦 Orders Endpoints</summary>
  
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/order` | Get all user orders | ✅ |
| `GET` | `/order/{orderId}` | Get order by ID | ✅ |

</details>

<details>
<summary>💳 Payment Endpoints</summary>
  
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/payment/create-checkout-session` | Create Stripe checkout session | ✅ |
| `GET` | `/payment/payment-success` | Success page after checkout | ❌ |
| `GET` | `/payment/payment-cancelled` | Cancelled page after checkout | ❌ |
| `POST` | `/payment/webhook` | Handle Stripe webhooks | ❌ |

</details>

<details>
<summary>🛍️ Products Endpoints</summary>
  
| Method | Endpoint | Description | Auth | Admin |
|--------|----------|-------------|------|-------|
| `GET` | `/products` | Get all products | ❌ | ❌ |
| `POST` | `/products` | Create new product | ✅ | ✅ |
| `GET` | `/products/filter` | Get products by filter criteria | ❌ | ❌ |
| `GET` | `/products/{id}` | Get product by ID | ❌ | ❌ |
| `PUT` | `/products/{id}` | Update product | ✅ | ✅ |
| `DELETE` | `/products/{id}` | Delete product | ✅ | ✅ |

</details>

<details>
<summary>👥 Users Endpoints</summary>
  
| Method | Endpoint | Description | Auth | Admin |
|--------|----------|-------------|------|-------|
| `POST` | `/register` | Register new user | ❌ | ❌ |
| `POST` | `/login` | Login user | ❌ | ❌ |
| `POST` | `/refresh-token` | Refresh JWT token | ✅ | ❌ |
| `POST` | `/make-admin` | Grant admin privileges | ✅ | ✅ |

</details>


## ⚙️ Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server
- Stripe account (optional, for payment processing)

### Installation

1. **Clone the repository**  
   `git clone https://github.com/weizheng2/aspnet-ecommerce-api.git`

2. **Configure application settings**  
   Update your appsettings.json, environment variables, or user-secrets with the required keys:  
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Your SQL Server connection string"
     },
     "Stripe": {
       "SecretKey": "",
       "EndPointSecret": ""
     },
     "AppSettings": {
       "BaseUrl": "(Optional) Used for Stripe success and cancel URLs""
     }
   }
   ```
   
3. **Run and test the application locally**  
   `dotnet run`  
   `http://localhost:5177/swagger/index.html`
