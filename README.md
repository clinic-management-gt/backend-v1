# Backend - Clinic

This is the backend for the clinic management system. Developed in .NET, it exposes a REST API with endpoints like:

- `GET /pacientes`
- `GET /testdb` (optional, used for database connection testing)

This backend runs inside a Docker container and connects to a PostgreSQL database.  
The initial phase is intended to verify that all services are correctly included and running.

A folder called `Clinica` is used to organize the code, inspect logs, and access data stored in PostgreSQL.

---

## 📁 Configuration Files

### 🐳 Dockerfile

The `Dockerfile` contains the instructions to build the backend image, including:

Step-by-step explanation:

1. **Use a .NET SDK image** as the base for building the app.
2. **Set the working directory** inside the container.
3. **Copy the project files** into the container.
4. **Restore and build** the .NET project.
5. **Expose port 8080**, which is mapped to port 9000 on the host.

---

### 🛠️ docker-compose.yml

The `docker-compose.yml` file orchestrates the following services:

1. **Backend in .NET**
2. **PostgreSQL database**
3. **Adminer** (a web-based database manager)

All services are connected via a Docker network called `clinica-network`. You can use a different network name, just make sure both backend and frontend share the same one.

```yaml
networks:
  default:
    external:
      name: clinica-network
```

---

## ✅ How to Run the Environment

1. Open a terminal in the backend project root folder.
2. Make sure Docker is installed and running.
3. Ensure the Docker network is created:

```bash
docker network ls  # to check
docker network create clinica-network  # only if not exists
```

4. Run the backend container:

```bash
docker-compose up --build
```

> 💡 You should start:
> 1. The Docker network
> 2. The backend
> 3. The frontend (from its own folder)

Once running:

- Backend is available at: `http://localhost:9000`
- Example endpoints:
  - `GET /pacientes` – returns dummy patient data or real DB data depending on controller setup
  - `GET /testdb` – checks database connection (optional)

- Adminer interface (for DB inspection) at: `http://localhost:9001`

---

## 🧩 Functionality

- The `/pacientes` endpoint returns sample patients or real entries from the PostgreSQL table `pacientes`.
- The backend reads the database connection string from the environment variable: `ConnectionStrings__DefaultConnection`.
