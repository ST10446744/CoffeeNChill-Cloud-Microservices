# CoffeeNChill-Cloud-Microservices
A cloud-based CoffeeNChill microservices platform developed using ASP.NET Core Web API, Azure Functions, Azure Storage, Docker, JWT authentication and AWS EC2 deployment.
# CoffeeNChill – Cloud-Based Microservices Platform

**A cloud-based coffee shop management platform built using ASP.NET Core Web API, Azure Functions, Azure Storage, Docker and AWS EC2.**

---

## 1. Important Project Information

| Item                    | Details                                             |
| ----------------------- | --------------------------------------------------- |
| Project Name            | CoffeeNChill                                        |
| Project Type            | Cloud-Based Microservices Application               |
| Backend Language        | C#                                                  |
| Framework               | .NET 8                                              |
| Cloud Architecture      | Microservices                                       |
| Database / Storage      | Azure Table Storage, Blob Storage and Queue Storage |
| API Gateway             | ASP.NET Core Web API                                |
| Authentication          | JWT (JSON Web Tokens)                               |
| Password Security       | PBKDF2 Password Hashing                             |
| Containerisation        | Docker                                              |
| Container Orchestration | Docker Compose                                      |
| Local Storage Emulator  | Azurite                                             |
| Cloud Deployment        | AWS EC2 – Ubuntu                                    |
| API Documentation       | Swagger / OpenAPI                                   |

### Important Links

| Resource               | Link                                                                               |
| ---------------------- | ---------------------------------------------------------------------------------- |
| Live AWS API Gateway   | http://13.63.20.68:8080/swagger                                                    |
| GitHub Repository      | [Insert GitHub repository link]                                                    |
| Functions Docker Image | [Docker Hub – Functions](https://hub.docker.com/r/ghoste01/coffeenchill-functions) |
| Gateway Docker Image   | [Docker Hub – Gateway](https://hub.docker.com/r/ghoste01/coffeenchill-gateway)     |
| Azurite Docker Image   | [Docker Hub – Azurite](https://hub.docker.com/r/ghoste01/coffeenchill-azurite)     |
| Demonstration Video    | [Insert YouTube video link]                                                        |

**Deployment note:** The AWS public IP address may change if the EC2 instance is stopped and restarted without an Elastic IP address.

---

## 2. Project Overview

CoffeeNChill is a cloud-based microservices application designed to support the operations of a coffee shop.

The platform provides functionality for managing menu items, uploading and retrieving staff documents, processing customer orders and securing access to administrative functionality.

The application follows a distributed architecture in which different services handle specific responsibilities.

The system consists of three primary components:

1. **Azure Functions:** Handles menu management, document operations and background order processing.
2. **ASP.NET Core API Gateway:** Provides a central entry point for client requests and manages authentication and authorisation.
3. **Azure Storage Services:** Provides table, blob and queue storage for application data.

Docker is used to containerise the services, while Docker Compose manages the application containers. The completed application is deployed to an Ubuntu-based AWS EC2 virtual server.

The project demonstrates cloud application development, RESTful API design, asynchronous processing, authentication, containerisation and cloud deployment.

---

## 3. System Architecture

CoffeeNChill uses a microservices-based architecture.

### Architecture Components

| Component           | Responsibility                                     |
| ------------------- | -------------------------------------------------- |
| Client              | Sends HTTP requests to the application             |
| API Gateway         | Central entry point for API requests               |
| Azure Functions     | Executes business logic and application operations |
| Azure Table Storage | Stores menu items, orders and user accounts        |
| Azure Blob Storage  | Stores staff documents                             |
| Azure Queue Storage | Handles asynchronous order-processing messages     |
| Azurite             | Provides local Azure Storage emulation             |
| Docker              | Packages application components into containers    |
| Docker Compose      | Starts and manages the application services        |
| AWS EC2             | Hosts the deployed application                     |

### Request Flow

1. A client sends a request to the ASP.NET Core API Gateway.
2. The Gateway determines whether authentication is required.
3. JWT authentication and role-based authorisation are applied to protected endpoints.
4. The Gateway communicates with the Azure Functions service.
5. Azure Functions performs the requested operation.
6. Data is stored or retrieved from the appropriate Azure Storage service.
7. The response is returned to the client through the Gateway.

### Order Processing Flow

1. A customer submits an order through the API Gateway.
2. The Gateway forwards the request to Azure Functions.
3. The order is serialised into JSON.
4. The message is placed into the Azure Queue Storage order-processing queue.
5. A queue-triggered Azure Function receives the message.
6. The order is saved in Azure Table Storage.
7. The order-processing function simulates the following status progression:

`Received → Preparing → Ready → Collected`

This approach separates the order submission process from the background processing logic.

---

## 4. Technologies and Tools

| Technology           | Purpose                           |
| -------------------- | --------------------------------- |
| C#                   | Backend programming language      |
| .NET 8               | Application development framework |
| ASP.NET Core Web API | API Gateway                       |
| Azure Functions      | Serverless business logic         |
| Azure.Data.Tables    | Table Storage integration         |
| Azure.Storage.Blobs  | Blob Storage integration          |
| Azure.Storage.Queues | Queue Storage integration         |
| JWT                  | Authentication and access control |
| PBKDF2               | Password hashing                  |
| Docker               | Containerisation                  |
| Docker Compose       | Multi-container deployment        |
| Azurite              | Local Azure Storage emulator      |
| AWS EC2              | Virtual server hosting            |
| Ubuntu               | Server operating system           |
| Swagger / OpenAPI    | API documentation and testing     |
| Postman              | API testing                       |
| Git and GitHub       | Source control and collaboration  |
| Visual Studio 2022   | Development environment           |

---

## 5. Application Features

### 5.1 Menu Management

The application supports:

* Creating menu items.
* Retrieving all menu items.
* Retrieving menu items by category.
* Updating existing menu items.
* Deleting menu items.

Each menu item contains:

* Category
* SKU
* Name
* Description
* Price
* Availability

Menu items are stored in Azure Table Storage using the category as the PartitionKey and SKU as the RowKey.

### 5.2 Staff Document Management

The application supports:

* Uploading staff documents.
* Listing uploaded documents.
* Downloading documents by filename.

Documents are stored in Azure Blob Storage inside the `staff-docs` container.

Document endpoints are protected by role-based authorisation at the API Gateway.

### 5.3 Order Management

The application supports:

* Submitting customer orders.
* Sending orders to Azure Queue Storage.
* Processing orders asynchronously.
* Saving order information in Azure Table Storage.
* Retrieving order status.

The order-processing queue is named:

`order-processing-queue`

### 5.4 Authentication and Authorisation

The API Gateway implements JWT-based authentication.

The authentication functionality includes:

* Customer registration.
* Customer login.
* Password hashing using PBKDF2.
* JWT generation.
* Token validation.
* Role-based authorisation.

JWT claims include:

* Name
* Email
* Role

The application uses role-based access restrictions for administrative and staff operations.

---

## 6. API Documentation and Endpoints

The API can be explored through Swagger.

**Live Swagger documentation:**

http://13.63.20.68:8080/swagger

### Authentication Endpoints

| Method | Endpoint             | Description                                |
| ------ | -------------------- | ------------------------------------------ |
| POST   | `/api/auth/register` | Register a customer                        |
| POST   | `/api/auth/login`    | Authenticate a customer and generate a JWT |

### Menu Endpoints

| Method | Endpoint                        | Description                     |
| ------ | ------------------------------- | ------------------------------- |
| POST   | `/api/menu`                     | Create a menu item              |
| GET    | `/api/menu`                     | Retrieve all menu items         |
| GET    | `/api/menu/category/{category}` | Retrieve menu items by category |
| PUT    | `/api/menu/{category}/{id}`     | Update a menu item              |
| DELETE | `/api/menu/{category}/{id}`     | Delete a menu item              |

Menu retrieval is publicly accessible. Menu modification operations are restricted to authorised Admin and Staff roles.

### Document Endpoints

| Method | Endpoint                             | Description                   |
| ------ | ------------------------------------ | ----------------------------- |
| POST   | `/api/documents/upload`              | Upload a staff document       |
| GET    | `/api/documents`                     | Retrieve the document listing |
| GET    | `/api/documents/download/{fileName}` | Download a document           |

Document operations require an authorised Admin or Staff role.

### Order Endpoints

| Method | Endpoint                  | Description                               |
| ------ | ------------------------- | ----------------------------------------- |
| POST   | `/api/orders/queue`       | Submit an order for background processing |
| GET    | `/api/orders/{date}/{id}` | Retrieve order status                     |

Order submission is publicly accessible. Retrieving order status through the Gateway requires an authorised Admin or Staff role.

---

## 7. Project Structure

The solution is organised into two primary projects.

```text
CoffeeNChill/
│
├── CoffeeNChill.Functions/
│   │
│   ├── Models/
│   │   ├── MenuItem.cs
│   │   ├── Order.cs
│   │   └── OrderRequest.cs
│   │
│   ├── CreateMenuItem.cs
│   ├── GetAllMenuItems.cs
│   ├── GetMenuItemsByCategory.cs
│   ├── UpdateMenuItem.cs
│   ├── DeleteMenuItem.cs
│   │
│   ├── UploadStaffDocument.cs
│   ├── ListStaffDocuments.cs
│   ├── DownloadStaffDocument.cs
│   │
│   ├── QueueOrder.cs
│   ├── ProcessOrderQueue.cs
│   ├── GetOrderStatus.cs
│   │
│   ├── Dockerfile
│   └── local.settings.json
│
├── CoffeeNChill.Gateway/
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── MenuController.cs
│   │   ├── OrdersController.cs
│   │   └── DocumentsController.cs
│   │
│   ├── Models/
│   │   ├── UserAccount.cs
│   │   ├── RegisterRequest.cs
│   │   └── LoginRequest.cs
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
│
├── docker-compose.yml
├── README.md
└── CoffeeNChill.slnx
```

*Note: The structure above highlights the principal application files. Additional configuration and project files are omitted for readability.*

---

## 8. Installation and Local Setup

### Prerequisites

The following software is required:

* Windows 10/11 or a compatible operating system.
* Visual Studio 2022.
* .NET 8 SDK.
* Docker Desktop.
* Docker Compose.
* Git.
* Postman (recommended).

### Step 1: Clone the Repository

```bash
git clone <GITHUB_REPOSITORY_URL>
```

Navigate into the solution directory:

```bash
cd CoffeeNChill
```

### Step 2: Start the Application Using Docker Compose

Ensure Docker Desktop is running.

Execute:

```bash
docker compose up -d
```

Docker Compose downloads the published images and starts the following services:

* Azurite
* CoffeeNChill Functions
* CoffeeNChill Gateway

### Step 3: Verify Containers

```bash
docker compose ps
```

View application logs:

```bash
docker compose logs
```

### Step 4: Access Swagger

Open:

```text
http://localhost:8080/swagger
```

### Step 5: Stop the Application

```bash
docker compose down
```

To remove the persistent Azurite data volume as well:

```bash
docker compose down -v
```

**Warning:** Removing the volume deletes the locally persisted storage data.

---

## 9. Docker Hub Images

The application uses published Docker images.

| Service         | Docker Image                           |
| --------------- | -------------------------------------- |
| Azure Functions | `ghoste01/coffeenchill-functions:v2.0` |
| API Gateway     | `ghoste01/coffeenchill-gateway:v1.0`   |
| Azurite         | `ghoste01/coffeenchill-azurite:v1.0`   |

To pull the images manually:

```bash
docker pull ghoste01/coffeenchill-functions:v2.0
docker pull ghoste01/coffeenchill-gateway:v1.0
docker pull ghoste01/coffeenchill-azurite:v1.0
```

The root `docker-compose.yml` file references these published images rather than building the application from source.

---

## 10. AWS VPS Deployment

CoffeeNChill was deployed to an AWS EC2 virtual server running Ubuntu Linux.

### Server Configuration

| Configuration     | Value               |
| ----------------- | ------------------- |
| Cloud Provider    | Amazon Web Services |
| Service           | EC2                 |
| Operating System  | Ubuntu 26.04 LTS    |
| Instance Type     | t3.small            |
| Storage           | 20 GiB              |
| Container Runtime | Docker              |
| Orchestration     | Docker Compose      |
| Application Port  | 8080                |

### Deployment Process

1. An Ubuntu EC2 instance was created.
2. SSH access was configured using an EC2 key pair.
3. The server was accessed through Windows PowerShell.
4. Ubuntu packages were updated.
5. Docker Engine was installed.
6. Docker Compose was installed.
7. The CoffeeNChill deployment directory was created.
8. The Docker Compose configuration was transferred to the server.
9. The application containers were started.
10. The EC2 security group was configured to allow inbound TCP traffic on port 8080.
11. The public Swagger endpoint was accessed through a web browser.

### Public Application URL

http://13.63.20.68:8080/swagger

The application is hosted on AWS EC2 and is accessible through its public IPv4 address while the instance is running and the required network rules are enabled.

---

## 11. Authentication and Security

CoffeeNChill uses JWT bearer authentication to protect restricted API operations.

### Registration

Customers can register using their full name, email address and password.

The application hashes passwords before storing them in Azure Table Storage.

### Login

When valid login credentials are supplied, the application generates a signed JWT.

The token contains the user's identity and role claims.

### Authorisation

Protected endpoints require a valid bearer token.

The application applies role restrictions using ASP.NET Core authorisation.

### Security Considerations

* Passwords are stored as hashes rather than plaintext.
* JWT tokens have an expiry time.
* Protected endpoints require authentication.
* Role-based restrictions are applied to administrative operations.
* Storage services are kept internal to the Docker network in the VPS deployment.

**Production considerations:** The demonstration configuration must be strengthened before production use. JWT signing keys should be stored securely using environment-specific secrets or a secrets manager. HTTPS should be configured, and the development storage emulator should be replaced with appropriate production Azure Storage resources.

---

## 12. Testing and Verification

The application was developed and tested using Visual Studio, Swagger and Postman.

### Functions Testing

The following menu and document operations were manually tested locally:

* Creating menu items.
* Retrieving menu items.
* Retrieving menu items by category.
* Updating menu items.
* Deleting menu items.
* Uploading staff documents.
* Listing staff documents.
* Downloading staff documents.

Successful HTTP responses were received for these tested operations.

### Gateway Testing

The API Gateway was built and launched successfully.

Swagger was used to inspect the available endpoints, and customer registration was tested.

### Docker Testing

The three published Docker images were deployed using Docker Compose.

The containers were verified using:

```bash
docker compose ps
```

### AWS Testing

The application was deployed to AWS EC2 and the public Swagger endpoint was successfully opened in a browser.

### Additional Testing

The following should be included in the final demonstration and verified before submission:

* Customer login and JWT validation.
* Access to protected endpoints using a bearer token.
* Rejection of unauthorised requests.
* Order submission.
* Queue-triggered order processing.
* Order status retrieval.

---

## 13. Challenges and Technical Solutions

The development process involved several technical challenges.

| Challenge                                                   | Solution                                                       |
| ----------------------------------------------------------- | -------------------------------------------------------------- |
| Azure Storage dependencies                                  | Installed the required Azure SDK packages                      |
| Local Azure Storage requirements                            | Used Azurite through Docker                                    |
| Entity Framework and framework compatibility considerations | Used compatible .NET package versions                          |
| Communication between containers                            | Used Docker Compose service names and internal networking      |
| API authentication                                          | Implemented JWT bearer authentication                          |
| Password security                                           | Used ASP.NET Core PasswordHasher with PBKDF2                   |
| Background order processing                                 | Implemented Azure Queue Storage and a queue-triggered Function |
| Container deployment                                        | Created Dockerfiles for the Functions and Gateway projects     |
| Multi-service deployment                                    | Configured Docker Compose                                      |
| Public cloud hosting                                        | Deployed the application to an Ubuntu AWS EC2 instance         |
| Remote server access                                        | Configured SSH access using an EC2 key pair                    |

---

## 14. Conclusion

CoffeeNChill demonstrates the development and deployment of a cloud-based microservices application using modern Microsoft and cloud technologies.

The project integrates Azure Functions, Azure Storage, an ASP.NET Core API Gateway, JWT authentication, Docker and AWS EC2.

The application demonstrates how individual services can communicate through HTTP requests and asynchronous messaging while maintaining separate responsibilities.

The completed deployment provides a publicly accessible API documentation endpoint through AWS EC2.

The project also provides practical experience in RESTful API development, cloud storage, containerisation, security, distributed systems and cloud infrastructure management.

---

## 15. References – Harvard Style

The following sources provide technical documentation and background information for the technologies and implementation approaches used in CoffeeNChill.

1. Amazon Web Services (n.d.) *Amazon EC2 User Guide*. Available at: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/concepts.html (Accessed: 4 November 2026).

2. Amazon Web Services (n.d.) *Security groups for your EC2 instances*. Available at: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ec2-security-groups.html (Accessed: 4 November 2026).

3. Docker (n.d.) *Docker Compose documentation*. Available at: https://docs.docker.com/compose/ (Accessed: 4 November 2026).

4. Docker (n.d.) *Dockerfile reference*. Available at: https://docs.docker.com/reference/dockerfile/ (Accessed: 4 November 2026).

5. Microsoft (n.d.) *Azure Functions documentation*. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/ (Accessed: 4 November 2026).

6. Microsoft (n.d.) *Azure Functions isolated worker model*. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide (Accessed: 4 November 2026).

7. Microsoft (n.d.) *Introduction to Azure Table Storage*. Available at: https://learn.microsoft.com/en-us/azure/storage/tables/table-storage-overview (Accessed: 4 November 2026).

8. Microsoft (n.d.) *Introduction to Azure Blob Storage*. Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction (Accessed: 4 November 2026).

9. Microsoft (n.d.) *Azure Queue Storage introduction*. Available at: https://learn.microsoft.com/en-us/azure/storage/queues/storage-queues-introduction (Accessed: 4 November 2026).

10. Microsoft (n.d.) *Azurite emulator for Azure Storage*. Available at: https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite (Accessed: 4 November 2026).

11. Microsoft (n.d.) *ASP.NET Core Web API documentation*. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/ (Accessed: 4 November 2026).

12. Microsoft (n.d.) *Overview of ASP.NET Core authentication*. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/ (Accessed: 4 November 2026).

13. Microsoft (n.d.) *PasswordHasher<TUser> Class*. Available at: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1 (Accessed: 4 November 2026).

14. OpenAPI Initiative (n.d.) *OpenAPI Specification*. Available at: https://spec.openapis.org/oas/latest.html (Accessed: 4 November 2026).

15. Postman (n.d.) *Postman Learning Center*. Available at: https://learning.postman.com/ (Accessed: 4 November 2026).

---

## 16. AI Assistance Declaration

AI-based assistance was used as a supplementary development and learning resource during the CoffeeNChill project.

ChatGPT (OpenAI) was used to assist with understanding technical concepts, troubleshooting development issues, interpreting errors, planning implementation steps and improving project documentation.

All application implementation, configuration, testing and deployment activities were carried out and reviewed as part of the project development process.

**AI tool reference:**

OpenAI (2026) *ChatGPT*. Available at: https://chatgpt.com/ (Accessed: 4 November 2026).

---

**Project:** CoffeeNChill
**Framework:** .NET 8
**Language:** C#
**Deployment:** AWS EC2
**Containerisation:** Docker and Docker Compose
