[README.md.md](https://github.com/user-attachments/files/30151010/README.md.md)
# E-commerce RESTful API

## Overview

This project is a robust and scalable RESTful API designed to power modern e-commerce applications. It provides a comprehensive set of endpoints for managing products, categories, shopping carts, orders, and user authentication, serving as a powerful backend for various frontend clients (web, mobile).

## Features

*   **Product Management:** CRUD operations for products, including details, images, and variants.
*   **Category & Subcategory Management:** Organize products into hierarchical categories.
*   **Shopping Cart Functionality:** Add, update, and remove items from the user's shopping cart.
*   **Order Processing:** Create and manage customer orders, including status updates.
*   **User Authentication & Authorization:** Secure user registration, login, and role-based access control using JWT (JSON Web Tokens) and ASP.NET Core Identity.
*   **Discount & Coupon Management:** Apply discounts and manage promotional coupons.
*   **Image Upload & Management:** Handle product images efficiently.
*   **API Documentation:** Interactive API documentation provided by Swagger/OpenAPI for easy integration and testing.

## Architecture & Design Principles

This API is built with a focus on maintainability, scalability, and performance, adhering to modern software engineering practices:

*   **RESTful Principles:** Designed as a stateless API with clear resource-based endpoints.
*   **Repository Pattern:** Implemented to abstract data access logic, ensuring a clean separation between business logic and data persistence. This enhances testability and allows for easier database changes.
*   **Dependency Injection:** Utilized extensively to manage service dependencies, promoting modularity and flexibility.
*   **Data Transfer Objects (DTOs):** Used to shape the data sent to and received from clients, protecting internal domain models and optimizing payload sizes.
*   **JWT Authentication:** Provides a secure, token-based authentication mechanism for API clients.
*   **Error Handling:** Centralized exception handling and meaningful error responses.

## Technologies Used

*   **Backend:**
    *   C#
    *   ASP.NET Core Web API
    *   Entity Framework Core (ORM)
    *   SQL Server (Database)
    *   LINQ (Language Integrated Query)
    *   ASP.NET Core Identity (User Management)
    *   JWT Bearer Authentication
    *   Swagger/OpenAPI (API Documentation)
*   **Frontend (Implicit):** Designed to be consumed by any frontend technology (e.g., React, Angular, Vue.js, Mobile Apps).

## Getting Started

To get the E-commerce API up and running on your local machine for development and testing purposes, follow these steps:

### Prerequisites

*   .NET SDK (version 8.0 or higher)
*   SQL Server (or SQL Server Express LocalDB)
*   Visual Studio (or Visual Studio Code with C# extension)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/YOUR_USERNAME/ECommerceApi.git
    cd ECommerceApi
    ```
2.  **Configure Database:**
    *   Open `appsettings.json` in the `e-comerce api` project.
    *   Update the `e-comerce` connection string to point to your local SQL Server instance.
    *   Run database migrations:
        ```bash
        dotnet ef database update --project "e-comerce api"
        ```
3.  **Configure JWT Secret:**
    *   Add a `JWT:secret` entry to your `appsettings.json` or user secrets with a strong, unique key.
4.  **Run the application:**
    ```bash
    dotnet run --project "e-comerce api"
    ```
    The API will typically run on `https://localhost:5064` (or a similar port) and Swagger UI will be available at `/swagger` endpoint.

## Contributing

Feel free to fork the repository, create a new branch, and submit pull requests. Any contributions are welcome!

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Contact

[Your Name] - [Your Email] - [Your LinkedIn Profile URL]
