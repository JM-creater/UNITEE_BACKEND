# UNITEE_BACKEND

The backend of UNITEE is the core engine driving our web-based storefront specializing in school uniforms and related merchandise. Built on a modern technology stack, it is designed for high performance and reliability, handling all data-intensive operations efficiently.

## Features

- **High Performance**: Efficiently manages all data-intensive operations.
- **Reliability**: Ensures consistent and dependable service.
- **Scalability**: Built to handle increasing loads as the business grows.

## Getting Started

### Prerequisites

- .NET Core SDK
- SQL Server

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/JM-creater/UNITEE_BACKEND.git
    ```
2. Navigate to the project directory and build the project:
    ```bash
    cd UNITEE_BACKEND
    dotnet build
    ```
3. Update the database connection string in `appsettings.json`.

### Running the Application

1. Apply database migrations:
    ```bash
    dotnet ef database update
    ```
2. Start the server:
    ```bash
    dotnet run
    ```

## License

This project is licensed under the MIT License.
