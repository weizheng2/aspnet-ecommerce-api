# ASP.NET Ecommerce API
A RESTful ASP.NET Core Web API for an e-commerce platform, designed to handle products, carts, orders, payments, and user management with authentication and role-based authorization.

## 🚀 Key Features

- **Products:** Full CRUD operations, with admin-only endpoints for creating, updating, and deleting products.  
- **Cart:** Add, remove, clear items, and calculate total cost for checkout.  
- **Orders:** Manage user orders and retrieve order history and details.  
- **Payment Integration:** Stripe checkout session and webhook handling for verification and order creation.  
- **User Management:** JWT authentication and role-based authorization for Admin and User access.  
- **Additional Features:** Pagination, API versioning for maintainability and rate limiting to manage traffic.


## 🛠️ Technologies Used
- ASP.NET Core
- Entity Framework Core with SQL database
- JWT Authentication
- Stripe Payment Gateway
- API Versioning
- Rate Limiting
- Swagger/OpenAPI


## 📚 Project Source
This project is based on the roadmap.sh E-commerce API project:
[https://roadmap.sh/projects/ecommerce-api](https://roadmap.sh/projects/ecommerce-api)


## 🌐 Live Demo
Access the live API via Swagger (may take a few seconds to start if idle):
[https://wz-ecommerce-api.runasp.net/swagger/index.html](https://wz-ecommerce-api.runasp.net/swagger/index.html)

---

## API Endpoints

### Cart Management
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/cart` | Get all cart items from user's cart | ✅ |
| `POST` | `/cart` | Add product to user's cart | ✅ |
| `PUT` | `/cart/{cartItemId}` | Update cart item quantity or remove if 0 or negative | ✅ |
| `DELETE` | `/cart/clear` | Remove all items from cart | ✅ |

### Order Management  
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/order` | Get all user orders | ✅ |
| `GET` | `/order/{orderId}` | Get order by ID | ✅ |

### Payment Processing
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/payment/create-checkout-session` | Create Stripe checkout session | ✅ |
| `GET` | `/payment/payment-success` | Success page after checkout | ❌ |
| `GET` | `/payment/payment-cancelled` | Cancelled page after checkout | ❌ |
| `POST` | `/payment/webhook` | Handle Stripe webhooks | ❌ |

### Product Management
| Method | Endpoint | Description | Auth | Admin |
|--------|----------|-------------|------|-------|
| `GET` | `/products` | Get all products | ❌ | ❌ |
| `POST` | `/products` | Create new product | ✅ | ✅ |
| `GET` | `/products/filter` | Get products by filter criteria | ❌ | ❌ |
| `GET` | `/products/{id}` | Get product by ID | ❌ | ❌ |
| `PUT` | `/products/{id}` | Update product | ✅ | ✅ |
| `DELETE` | `/products/{id}` | Delete product | ✅ | ✅ |

### User Authentication
| Method | Endpoint | Description | Auth | Admin |
|--------|----------|-------------|------|-------|
| `POST` | `/register` | Register new user | ❌ | ❌ |
| `POST` | `/login` | Login user | ❌ | ❌ |
| `POST` | `/refresh-token` | Refresh JWT token | ✅ | ❌ |
| `POST` | `/make-admin` | Grant admin privileges | ✅ | ✅ |

---

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server
- Stripe Account (for payment processing)

### Installation

1. **Clone the repository**
  ```bash
   git clone https://github.com/weizheng2/aspnet-ecommerce-api.git
  ```
2. Configure keys in appsettings.json, environment variables, user-secrets, etc:  
  ```bash
   ConnectionStrings__DefaultConnection (Database)
   Stripe__SecretKey 
   Stripe__EndPointSecret
   AppSettings__BaseUrl (For Stripe success and cancel url)
