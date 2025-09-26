# Backend - Clinic

This is the backend for the clinic management system. Developed in .NET, it exposes a REST API with endpoints like:

- `GET /ping` (optional, used for test the connection)

This backend runs inside a Docker container and connects to a PostgreSQL database.  
The initial phase is intended to verify that all services are correctly included and running.

A folder called `Clinica` is used to organize the code, inspect logs, and access data stored in PostgreSQL.

---

## 🔧 Prerequisitos

Antes de compilar o ejecutar el backend asegúrate de tener instalado el SDK de .NET 9 y el runtime de ASP.NET Core 9.0:

- `dotnet --list-sdks` debe mostrar `9.x`
- `dotnet --list-runtimes` debe incluir `Microsoft.AspNetCore.App 9.x`

Según tu sistema operativo puedes instalarlos con:

- **Ubuntu/Debian**: `sudo apt-get update && sudo apt-get install -y dotnet-sdk-9.0 aspnetcore-runtime-9.0`
- **Fedora/RHEL/CentOS**: `sudo dnf install dotnet-sdk-9.0 aspnetcore-runtime-9.0`
- **Arch/Manjaro**: `sudo pacman -S dotnet-sdk aspnet-runtime`
- **Windows (PowerShell admin)**: `winget install Microsoft.DotNet.SDK.9`
- **macOS (Homebrew)**: `brew install --cask dotnet-sdk`

Después de instalarlos, reinicia la terminal y valida nuevamente con `dotnet --info`.

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
    external: true
    name: //RED
```

## ✅ How to Run the Environment

1. Open a terminal in the backend project root folder.
2. Make sure Docker is installed and running.
3. Ensure the Docker network is created:

```bash
docker network ls  # to check
docker network create clinica-network  # only if not exists
```

> [!IMPORTANT]  
> \*.example files, such as .env and docker-compose, you must duplicate and add your own credentials and ports.

4. Run the backend container:

```bash
docker compose up --build -d
```
5. If you want to delete all the database, run:


```bash
docker compose down -v
```

> 💡 You should start:
>
> 1. The Docker network
> 2. The backend
> 3. The frontend (from its own folder) "https://github.com/clinic-management-gt/frontend-v1"

Once running:

- Backend is available at: `http://localhost:9000`
- Example endpoints:
  - `GET /ping` – checks database connection (optional)

- Adminer interface (for DB inspection) at: `http://localhost:9001`

---

## 🏛️ Entity-Relationship Diagram

![Diagram ER.](/proyecto-software-tables.png "Diagram ER")

[ERD](https://dbdiagram.io/d/proyecto-software-67f1f3d34f7afba1847d7bda) 

---

# 📘 API REST - Sistema de Gestión Clínica

## 🧩 Functionality

This document describes all available endpoints in the Clinic's REST API, along with their functionality and the database fields involved.

---

## 📅 AppointmentsController

### GET `/appointments`
Returns all appointments scheduled for the current day.

- Optional query parameter: `status`
  - Valid values: `confirmado`, `pendiente`, `completado`, `cancelado`, `espera`
- Response data:
  - Patient's full name
  - Doctor's full name
  - Appointment status
  - Appointment date

---

## 🩺 MedicalRecordsController

### GET `/medicalrecords/{id}`
Fetches a specific medical record by its ID.

- Parameter: medical record `id`
- Response data:
  - Patient ID
  - Weight, height
  - Family history
  - Notes
  - Created and updated timestamps

### PATCH `/medicalrecords/{id}`
Updates selected fields of a medical record.

- Request body: JSON with fields to update (`weight`, `height`, `family_history`, `notes`)
- Automatically updates the `updated_at` field

---

## 👤 PatientsController

### GET `/patients`
Returns all registered patients.

- Response data includes:
  - ID, name, last name
  - Birthdate, address, gender
  - Blood type ID, patient type ID
  - Created and updated timestamps

### GET `/patients/{id}`
Fetches a specific patient by ID.

### POST `/patients`
Registers a new patient.

- Request body: JSON with required fields:
  - `name`, `lastName`, `birthdate`, `address`, `gender`, `bloodTypeId`, `patientTypeId`

### PATCH `/patients/{id}`
Updates a patient's information.

- Request body: JSON with fields to be updated
- Automatically updates the `updated_at` field

### GET `/patients/{id}/medicalrecords`
Returns all medical records for a specific patient.

### POST `/patients/{id}/medicalrecords`
Creates a new medical record for the specified patient.

- Request body: JSON with:
  - `weight`, `height`, `family_history`, `notes`

### GET `/patients/{id}/exams`
Lists all medical exams linked to a patient.

- Includes: exam ID, result text, file path, created timestamp

### POST `/patients/{id}/exams`
Adds a new exam record for the patient.

- Request body: JSON with:
  - `examId`, `resultText`, `resultFilePath`

### GET `/patients/{id}/growthcurve`
Generates patient growth curves based on medical records.

- Response data:
  - Height vs age
  - Weight vs age
  - BMI vs age
  - Weight vs height

---

## 📁 PatientExamsController

### POST `/patient/exams`
Uploads an exam file to Cloudflare R2 and creates a record in the database.

- Form-Data:
  - `patientId`
  - `examId`
  - `resultText`
  - `file` (e.g., PDF or image)

---

## 💊 RecipesController

### GET `/recipes/patient/{id}`
Returns all medical prescriptions associated with a patient.

- Parameter: patient ID
- Response data:
  - Prescription
  - Medicine ID
  - Dosage
  - Duration
  - Frequency (optional)
  - Observations (optional)


---

## ⚙️ Requisitos de conexión a base de datos

The connection is made via `DefaultConnection` in `appsettings.json`. Make sure you have PostgreSQL enabled and configured correctly.

---

© Proyecto Clinic Managment

