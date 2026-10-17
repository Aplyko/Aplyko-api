# Aplyko-api

REST API and backend services for Aplyko.

## About
Aplyko is designed to centralize and automatically update job applications that you've applied to directly on our service. This repository (`Aplyko-api`) provides the core backend API powering these services.

The platform integrates with various mailing services via OAuth2 APIs to fetch application updates, confirmation emails, and status changes directly from your inbox, keeping your application dashboard synchronized automatically.

---

## Getting Started

### Prerequisites
- [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/) installed on your machine.

### Running for Development
To run the application in development mode with live rebuilding and configuration:

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d --build
```

### Running for Production
To run the application in production mode:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

---

## API Documentation

Once the application is running, you can explore the interactive API documentation via Scalar at:
- **`http://{HOST}:{API_PORT}/scalar/v1`**


## Authors
- David LIN
- Villon ZHANG

