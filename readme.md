# Nguyen Ordering System

This is a comprehensive **Order Management System** built with **.NET 8**, structured using the **MVC architecture**. It provides **RESTful APIs** to manage products, orders, and user authentication, with a clean separation of concerns through dedicated services for products, orders, and admin functionalities.  

The system leverages **Entity Framework Core (EF Core)** as the ORM for seamless interaction with a **PostgreSQL** database, while **Redis** is integrated for caching and **API Limiting** to improve performance. **JWT authentication** ensures secure user access.  

In addition, the project includes a suite of **unit tests** to maintain code reliability and quality.


---

### Key Features:

* **User Authentication:** Secure registration and login for both regular users and administrators.
* **Product Management:** Admins can create, read, update, and delete products.
* **Order Processing:** Users can create and view their orders.
* **Admin Dashboard:** Admins have access to all orders and sales statistics.
* **CI/CD Pipeline:** A GitHub Actions workflow is set up for continuous integration and deployment.
* **Containerized:** The entire application is containerized using Docker for easy setup and deployment.

---

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker](https://www.docker.com/products/docker-desktop) and [Docker Compose](https://docs.docker.com/compose/install/)
* A text editor or IDE like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)

---

### How to Run

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/lenguyen153/nguyen-ordering-system.git
    ```

2.  **Set up environment variables:**
    Create a `.env` file in the root of the project and add the following variables:
    ```bash
    POSTGRES_DB=ordering_system
    POSTGRES_USER=user
    POSTGRES_PASSWORD=password
    POSTGRES_CONNECTION_STRING="Host=db;Port=5432;Database=ordering_system;Username=user;Password=password"
    REDIS=redis:6379
    JWT_KEY="a-super-secret-key-that-is-long-enough"
    JWT_ISSUER="your-issuer"
    JWT_AUDIENCE="your-audience"
    JWT_EXPIRY_MINUTES=60
    ```

3.  **Run with Docker Compose:**
    ```bash
    docker-compose up --build
    ```
    The API will be available at `http://localhost:5057`.

4.  **Accessing the API:**
    You can access the Swagger UI to interact with the API at `http://localhost:5057/swagger`.

---

### Project Structure
```bash
.
├── .github/
│   └── workflows/
│       └── ci-cd.yml
├── src/
│   └── OrderManagementSystem/
│       ├── Auth/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Models/
│       ├── Properties/
│       ├── Services/
│       ├── OrderManagementSystem.csproj
│       ├── Dockerfile
│       └── Program.cs
├── tests/
│   └── OrderManagementSystem.UnitTests/
│       ├── OrderManagementSystem.UnitTests.csproj
│       └── UnitTest1.cs
├── .gitignore
├── docker-compose.yml
└── Nguyen-Ordering-System.sln
```

---

### Endpoints

Here are some of the main API endpoints:

#### Auth
* `POST /api/auth/register`: Register a new user.
* `POST /api/auth/login`: Log in to get a JWT token.

#### Products
* `GET /api/products`: Get all products (Admin and User).
* `POST /api/products`: Create a new product (Admin only).

#### Orders
* `POST /api/orders`: Create a new order (User and Admin).
* `GET /api/orders/my`: Get your own orders (User and Admin).

#### Admin
* `GET /api/admin/orders`: Get all orders (Admin only).
* `GET /api/admin/stats`: Get sales statistics (Admin only).

---

### Seeding the Database

The application automatically seeds the database with an admin user on startup. You can use these credentials to log in as an administrator:

* **Email:** `admin@example.com`
* **Password:** `Admin@12345`
