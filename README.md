# UntitledCinema

## Project Description
UntitledCinema is a full-featured cinema management system with a microservice architecture, web client, and mobile application. The project provides capabilities for viewing movie schedules, booking tickets, managing user accounts, and much more.

## Project Architecture
The project is built on a microservice architecture and consists of the following components:

### Architecture Diagram
```
┌─────────────────┐     ┌─────────────────┐
│   Web Client    │     │  Mobile Client  │
│    (Angular)    │     │    (Flutter)    │
└────────┬────────┘     └────────┬────────┘
         │                       │
         │                       │
         │      ┌───────────────┐│
         └──────►               ◄┘
                │  API Gateway  │
                │    (NGINX)    │
                │               │
                └─┬─────┬─────┬─┘
                  │     │     │
┌─────────────────┘     │     └─────────────────┐
│                       │                       │
│                       │                       │
▼                       ▼                       ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│  User Service   │ │  Movie Service  │ │ Booking Service │
│    (.NET)       │ │     (.NET)      │ │     (.NET)      │
└─────────┬───────┘ └─────────┬───────┘ └─────────┬───────┘
          │                   │                   │
          ▼                   ▼                   ▼
    ┌───────────┐       ┌───────────┐       ┌───────────┐
    │PostgreSQL │       │PostgreSQL │       │ MongoDB   │
    └───────────┘       └───────────┘       └───────────┘
          
                    ┌───────────────┐
                    │     Redis     │◄───┐
                    │   (Caching)   │    │
                    └───────────────┘    │
                                         │
                    ┌───────────────┐    │
                    │    RabbitMQ   │    │
                    │  (Messaging)  │────┼───────┐
                    └───────────────┘    │       │
                                         │       │
                    ┌───────────────┐    │       │
                    │     MinIO     │◄───┘       │
                    │ (File Storage)│            │
                    └───────────────┘            │
                                                 │
                                                 ▼
                                         ┌───────────────┐
                                         │ All Services  │
                                         │ (.NET)        │
                                         └───────────────┘
```

### Backend (Microservices)
- **API Gateway** - NGINX gateway for routing requests to microservices
- **User Service** - service for user management and authentication
- **Movie Service** - service for managing movies, showings, and cinemas
- **Booking Service** - service for ticket booking

### Data Storage
- PostgreSQL for User Service and Movie Service
- MongoDB for Booking Service
- Redis for caching
- MinIO for file storage
- RabbitMQ for asynchronous communication between services

### Frontend
- **Web Application** - Angular 19 using PrimeNG and TailwindCSS
- **Mobile Application** - Flutter

## Requirements
- Docker and Docker Compose
- .NET SDK (for development)
- Node.js and npm (for web client)
- Flutter (for mobile application)

## Running the Project

### Running all components via Docker Compose
```bash
cd src
docker-compose up -d
```

### Running the web client for development
```bash
cd src/client
npm install
npm run start
```

### Running the mobile application for development
```bash
cd src/mobile
flutter pub get
flutter run
```

## Access Ports
- **Web Application**: http://localhost:80
- **API Gateway**: http://localhost:80/api
- **RabbitMQ Management**: http://localhost:15673
- **MinIO Console**: http://localhost:9001

## Project Structure
```
src/
├── client/            # Angular web application
├── mobile/            # Flutter mobile application
├── server/
│   ├── UserService/   # User service
│   ├── MovieService/  # Movie service
│   ├── BookingService/# Booking service
│   └── Shared/        # Shared code for services
├── nginx/             # NGINX configuration
└── docker-compose.yml # Docker-compose configuration
```

## Technologies

### Backend
- .NET
- ASP.NET Core
- PostgreSQL
- MongoDB
- Redis
- RabbitMQ
- MinIO

### Web Client
- Angular 19
- TypeScript
- PrimeNG
- TailwindCSS
- SignalR (for real-time communication)

### Mobile Application
- Flutter
- Dart
- Provider (state management)
- Dio (HTTP client)
- SignalR (for real-time communication)

## Features
- User registration and authentication
- Movie browsing and filtering
- Session scheduling and management
- Seat selection and ticket booking
- Payment processing
- User reviews and ratings
- Admin dashboard for cinema management

## Getting Started
1. Clone the repository
2. Install the required dependencies
3. Run the project using Docker Compose
4. Access the web application at http://localhost:80

## Contributors
- Your name and contact information

## License
This project is licensed under the MIT License - see the LICENSE file for details.