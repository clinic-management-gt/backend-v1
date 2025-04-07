# Backend - Clinic

This is the backend for the clinic management system. It is developed in .NET and exposes a REST API with endpoints such as:

- `GET /pacientes`
- `GET /testdb`

The backend is designed to run inside a Docker container and connect to a PostgreSQL database.  
This first phase aims to verify that all services are included and working correctly.

A folder named `Clinica` has been set up to organize the code, inspect system logs, and access the data stored in PostgreSQL.

---

## 📁 Configuration Files

### 🐳 Dockerfile

The `Dockerfile` contains the necessary instructions to build the backend image, including:

- Selecting a base .NET image
- Installing dependencies
- Restoring and compiling the project
- Exposing the application port

---

### 🛠️ docker-compose.yml

The `docker-compose.yml` file orchestrates three services:

1. **Backend in .NET (ASPNET)**
2. **PostgreSQL database**
3. **Adminer** (a visual database manager)

Additionally, a network called `clinica-network` has been created to allow seamless communication between the backend, frontend, and other services.

---

## ✅ How to Run the Environment

```bash
docker-compose up --build
```

Once running:

🔹 Backend available at: `http://localhost:9000`  
Endpoints:

- `GET /pacientes`
- `GET /testdb`

🔹 Adminer (database manager): `http://localhost:9001`
