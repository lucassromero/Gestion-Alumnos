# Gestión de Profesoras Particulares

Aplicación web para gestionar alumnos, materias, contenidos, clases y materiales educativos.

> Estado actual: **MVP en desarrollo**. Funcionan el CRUD de alumnos y el módulo de materias con asociación a alumnos.

## Estructura

- `frontend/`: aplicación React + TypeScript + Vite + Tailwind CSS.
- `backend/`: API ASP.NET Core Web API + Entity Framework Core.
- `backend/GestionProfesora.Api.Tests/`: pruebas de integración de la API con xUnit.

## Requisitos

- Node.js
- .NET 9 SDK
- SQL Server (desarrollo con `\.\SQLEXPRESS` y autenticación de Windows)

## Configuración

### Base de datos

La cadena de conexión para desarrollo usa autenticación de Windows y está en `backend/appsettings.Development.json`.

Si querés cambiarla o si usás autenticación de SQL Server, configurala mediante `user-secrets` o variables de entorno para no guardar credenciales en el repositorio:

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GestionProfesora;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Frontend

Copiá el archivo de ejemplo y ajustá la URL del backend si es necesario:

```bash
cd frontend
cp .env.example .env.local
```

## Ejecutar en desarrollo

1. Restaurar la herramienta local de EF Core:

```bash
dotnet tool restore
```

2. Aplicar las migraciones a la base de datos:

```bash
cd backend
dotnet dotnet-ef database update
```

3. Iniciar el backend:

```bash
cd backend
dotnet run
```

Por defecto escucha en `http://localhost:5000`.

4. En otra terminal, iniciar el frontend:

```bash
cd frontend
npm install
npm run dev
```

Por defecto el frontend se abre en `http://localhost:5173`.

### Datos de demostración

Para probar la interfaz con datos de ejemplo sin tocar la base de desarrollo, existe el entorno `Demo`, que usa la base `GestionProfesora_Demo`.

1. Iniciar el backend en modo demo:

```bash
cd backend
dotnet run --launch-profile demo
```

La primera vez se crea la base, se aplican las migraciones y se cargan alumnos y materias de ejemplo. En arranques posteriores no se duplican los datos.

2. Iniciar el frontend normalmente:

```bash
cd frontend
npm run dev
```

La base de tests `GestionProfesora_Test` es independiente y los tests la recrean en cada ejecución, por lo que no sirve para demos manuales.

## Construir y probar

### Backend

```bash
cd backend
dotnet build
dotnet test
```

### Frontend

```bash
cd frontend
npm run lint
npm run build
```

## Funcionalidades actuales

- CRUD de alumnos (listar, crear, consultar, editar, activar/desactivar).
- CRUD de materias (listar, crear, consultar, editar, activar/desactivar).
- Asociación y desasociación de materias desde la ficha del alumno; la ficha de la materia muestra los alumnos asociados.
- Validaciones en frontend y backend (incluido rechazo de nombres de materia duplicados).
- Estados de carga, error y listado vacío.
- Interfaz responsive con Tailwind CSS.
- Pruebas automatizadas de integración del backend.

## Decisiones técnicas

- La desactivación de alumnos es lógica: no se borran físicamente para conservar el historial.
- La desactivación de materias también es lógica (`Activa`), sin borrado físico.
- El nombre de una materia es único por profesora.
- La relación muchos-a-muchos entre alumnos y materias se gestiona desde la ficha del alumno.
- La entidad `Profesora` y las relaciones de propiedad se modelaron desde el inicio, aunque todavía no hay autenticación. Mientras tanto, el backend resuelve una profesora sembrada en la base de datos.
- No se envía `IdProfesora` desde el cliente.
- No se guardan contraseñas ni credenciales en el repositorio.

## Próximos pasos

- Contenidos y progreso individual por alumno.
- Registro e historial de clases.
- Materiales educativos.
- Dashboard con datos reales.
- Agenda.
