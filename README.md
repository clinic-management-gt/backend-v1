# Backend - Clínica

Este es el backend del sistema de gestión para una clínica. Está desarrollado en .NET y expone una API REST con endpoints como:

- `GET /pacientes`
- `GET /testdb`

Este backend está preparado para ejecutarse en un contenedor Docker y conectarse con una base de datos PostgreSQL.  
Esta primera fase busca verificar que todos los servicios estén incluidos y funcionando correctamente.

Se ha configurado una carpeta llamada `Clinica`, que permite organizar el código, revisar logs del sistema y acceder a los datos almacenados en PostgreSQL.

---

## 📁 Archivos de Configuración

### 🐳 Dockerfile

El archivo `Dockerfile` contiene las instrucciones necesarias para construir la imagen del backend, incluyendo:

- Selección de imagen base de .NET
- Instalación de dependencias
- Restauración y compilación del proyecto
- Exposición del puerto de la aplicación

### 🛠️ docker-compose.yml

El archivo `docker-compose.yml` permite orquestar tres servicios:

1. **Backend en .NET (ASPNET)**
2. **Base de datos PostgreSQL**
3. **Adminer** (interfaz visual para la base de datos)

Además, se ha creado una red llamada `clinica-network` para que el backend pueda comunicarse fácilmente con el frontend y los demás servicios.

---

## ✅ Pasos para levantar el entorno

```bash
docker-compose up --build
```

Una vez levantado:

🔹 Backend disponible en: http://localhost:9000
Endpoints:

GET /pacientes

GET /testdb

🔹 Adminer (gestor de base de datos): http://localhost:9001
