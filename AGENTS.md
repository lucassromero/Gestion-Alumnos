# AGENTS.md — Gestión de Profesoras Particulares

## 1. Objetivo del proyecto

Desarrollar una aplicación web para que una profesora particular pueda gestionar sus alumnos, materias, programas de estudio, clases y materiales educativos.

El proyecto comienza como una demo funcional (MVP) para una sola profesora. Más adelante debe poder evolucionar a un SaaS multiusuario, donde distintas profesoras tengan cuentas y sus datos estén aislados.

La prioridad principal es el seguimiento académico: registrar qué estudió cada alumno, qué dificultades tuvo, cuánto avanzó y qué debe estudiar después.

## 2. Forma de trabajo del agente

- Antes de modificar archivos, inspeccionar la estructura existente del repositorio y respetar las decisiones ya tomadas.
- Trabajar en pasos pequeños y funcionales; no generar toda la aplicación de una sola vez.
- Explicar brevemente qué se va a implementar y qué archivos se modificarán.
- No reemplazar archivos completos si basta con un cambio puntual.
- No agregar dependencias sin justificar su necesidad.
- No inventar funcionalidades que no estén planteadas en este documento.
- Mantener nombres claros y consistentes en español para entidades y conceptos del dominio. Los nombres propios de tecnologías y convenciones del framework pueden permanecer en inglés.
- Validar los datos tanto en frontend como en backend cuando corresponda.
- Manejar errores y estados de carga de forma visible y comprensible.
- No afirmar que algo fue probado si no se ejecutaron las comprobaciones pertinentes.
- Si hay una decisión importante sin definir, proponer la alternativa más simple y explicar la decisión antes de introducir complejidad.

## 3. Stack tecnológico previsto

### Frontend
- React
- TypeScript
- Vite
- Tailwind CSS.

### Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core para acceso a datos y migraciones.

### Base de datos
- SQL Server.

### Herramientas
- Visual Studio Code como editor.
- Git y GitHub para control de versiones.

No confundir Visual Studio con Visual Studio Code. Los comandos y las instrucciones deben estar pensados para terminales y flujos de trabajo de VS Code.

## 4. Arquitectura prevista

Mantener frontend y backend separados:

- `frontend/`: aplicación React + TypeScript + Vite.
- `backend/`: API ASP.NET Core.

Si el repositorio ya tiene otra estructura, inspeccionarla y adaptarse sin mover archivos innecesariamente.

El frontend debe comunicarse con el backend mediante endpoints HTTP/REST. La lógica de negocio y las validaciones importantes deben residir en el backend, no solamente en React.

No conectar React directamente a SQL Server.

## 5. Alcance de la primera versión (MVP)

### 5.1 Dashboard
Mostrar un resumen útil:
- Cantidad de alumnos activos.
- Próximas clases.
- Clases pendientes o canceladas.
- Tareas o temas que requieren seguimiento.

### 5.2 Alumnos
Permitir:
- Listar alumnos.
- Crear, consultar y editar fichas.
- Marcar un alumno como inactivo sin borrar su historial.
- Guardar nombre, apellido, contacto, nivel educativo y observaciones.
- Consultar materias y el historial de clases del alumno.

### 5.3 Materias y programas de estudio
Permitir:
- Crear y administrar materias, por ejemplo Matemática, Inglés o Física.
- Asociar varias materias a un alumno.
- Organizar cada materia en contenidos o temas ordenados.
- Registrar el progreso individual de cada alumno en cada contenido.

Estados sugeridos para el progreso individual:
- Pendiente.
- En curso.
- Visto.
- Necesita repaso.
- Dominado.

El estado del contenido debe ser individual por alumno. No marcar un contenido como completado globalmente para todos los alumnos.

### 5.4 Registro de clases
Cada clase debe poder guardar:
- Alumno.
- Fecha y hora.
- Duración.
- Estado (por ejemplo, programada, realizada, cancelada).
- Observaciones de la clase.
- Temas trabajados.
- Dificultades observadas.
- Tarea o recomendación para la próxima clase.

Debe conservarse el historial, sin sobrescribir la información de clases anteriores al registrar una nueva.

### 5.5 Materiales educativos
Permitir registrar:
- Título.
- Descripción.
- Tipo de material, si corresponde.
- Archivo o enlace.
- Contenido al que está asociado.

Los materiales deberían poder reutilizarse entre alumnos. No guardar archivos binarios directamente en SQL Server; guardar la referencia y los metadatos en la base de datos. Durante el desarrollo se puede usar almacenamiento local, pero debe quedar encapsulado para poder cambiar a almacenamiento en la nube más adelante.

### 5.6 Agenda
La agenda puede incorporarse después de completar alumnos, materias y registro de clases. Debe permitir consultar clases por fecha y registrar cancelaciones.

## 6. Modelo de datos conceptual inicial

El modelo debe evolucionar según las necesidades reales y no debe implementarse a ciegas. Entidades previstas:

### Alumno
- IdAlumno
- Nombre
- Apellido
- Email (opcional)
- Telefono (opcional)
- NivelEducativo (opcional)
- Observaciones (opcional)
- Activo
- FechaAlta

### Materia
- IdMateria
- Nombre
- Descripcion (opcional)
- Activa

### AlumnoMateria
Tabla intermedia para la relación muchos-a-muchos entre alumnos y materias:
- IdAlumno
- IdMateria

### Contenido
- IdContenido
- IdMateria
- Tema
- Descripcion (opcional)
- Orden

### Clase
- IdClase
- IdAlumno
- FechaHora
- DuracionMinutos
- Observaciones (opcional)
- TareaProxima (opcional)
- Estado

### ClaseContenido
Tabla intermedia para registrar qué contenidos se trabajaron en una clase:
- IdClase
- IdContenido
- Estado
- Observaciones (opcional)

### ProgresoAlumnoContenido
Se recomienda una entidad independiente para representar el estado actual de cada alumno respecto de cada contenido:
- IdAlumno
- IdContenido
- Estado
- Observaciones (opcional)
- FechaActualizacion

Esta entidad evita confundir el historial de contenidos vistos en una clase con el dominio actual del tema por parte del alumno.

### Material
- IdMaterial
- IdContenido (opcional)
- Titulo
- Descripcion (opcional)
- ArchivoUrl o referencia de almacenamiento (opcional)
- FechaCarga

Ajustar las relaciones si un material debe asociarse a más de un contenido. Evitar duplicar archivos solo para relacionarlos con varios temas.

## 7. Preparación para un futuro SaaS

Aunque el MVP se use con una sola profesora, no diseñar la aplicación de una forma que haga difícil separar los datos después.

- Prever una entidad Usuario/Profesora.
- Diseñar la propiedad de los datos para que alumnos, clases, materiales y otros registros pertenezcan a una profesora.
- Modelar `Profesora` y las relaciones de propiedad desde el esquema inicial del MVP, aunque todavía no haya autenticación.
- Resolver la profesora actual en el backend mediante un servicio; mientras no exista login, se usa una profesora sembrada en la base de datos.
- En una etapa posterior, añadir autenticación segura y autorización en el backend.
- Toda consulta y modificación debe quedar limitada a la profesora autenticada.
- Nunca confiar en un `IdProfesora` enviado por el cliente como única comprobación de autorización.
- No implementar autenticación casera ni almacenar contraseñas en texto plano.
- No es necesario construir pagos, suscripciones ni panel de administración del SaaS en el MVP.
- Antes de publicar, revisar aislamiento entre usuarios, seguridad, backups, manejo de archivos y configuración de secretos.

## 8. Seguridad y privacidad

- No incluir contraseñas, claves ni cadenas de conexión con credenciales en el repositorio.
- Usar configuración local/secrets y variables de entorno para valores sensibles.
- No registrar datos personales innecesarios en logs.
- Validar las entradas del usuario y usar Entity Framework Core de forma segura.
- No exponer endpoints de modificación sin la autorización correspondiente cuando se incorpore autenticación.
- Usar datos ficticios durante desarrollo y demostraciones.
- No borrar físicamente alumnos si eso destruye su historial; preferir desactivación cuando sea apropiado.

## 9. Diseño de interfaz

La interfaz debe ser:
- Clara, moderna y profesional.
- Responsive, usable desde computadora, tablet y celular.
- Fácil de utilizar para una persona que no necesariamente tenga conocimientos técnicos.
- Consistente en botones, formularios, tablas, navegación, mensajes y estados vacíos.
- Accesible: etiquetas visibles, contraste suficiente, navegación por teclado y mensajes de error comprensibles.

Navegación inicial sugerida:
- Dashboard
- Alumnos
- Materias y contenidos
- Clases
- Materiales

No agregar pantallas vacías o botones sin funcionalidad. Si una función todavía no está implementada, indicarlo claramente.

## 10. Orden de implementación

Trabajar en esta secuencia:

1. Inspeccionar el entorno, el repositorio y las versiones de Node.js y .NET disponibles.
2. Crear o verificar el frontend React + TypeScript + Vite.
3. Crear o verificar el backend ASP.NET Core Web API.
4. Configurar SQL Server y Entity Framework Core.
5. Diseñar entidades, relaciones y migraciones iniciales.
6. Implementar CRUD de alumnos.
7. Implementar materias y asociación alumno-materia.
8. Implementar contenidos y progreso individual.
9. Implementar registro e historial de clases.
10. Implementar materiales.
11. Construir el dashboard con datos reales de la API.
12. Mejorar agenda, responsive, validaciones y manejo de errores.
13. Documentar cómo ejecutar el proyecto desde VS Code.

No avanzar a una etapa posterior si la actual no compila o tiene errores importantes sin explicar.

## 11. Calidad y comprobaciones

- Mantener TypeScript estricto y evitar `any` salvo justificación.
- Mantener controladores del backend pequeños; mover reglas de negocio a servicios cuando sea útil.
- Usar DTOs para las entradas y salidas de la API cuando corresponda.
- Usar migraciones de Entity Framework Core para versionar el esquema de base de datos.
- No modificar la base manualmente de forma que quede desincronizada de las migraciones.
- Probar los endpoints principales y los casos de error, preferentemente con pruebas automatizadas de integración.
- Ejecutar el build del frontend, la compilación del backend y las pruebas después de cambios relevantes.
- Documentar comandos, configuración requerida y decisiones técnicas importantes.

## 12. Próximo objetivo concreto

Los módulos de Alumnos y de Materias (con asociación alumno-materia desde la ficha del alumno) ya están funcionales y probados. Continuar con el siguiente módulo:

1. Implementar contenidos (`Contenido`): temas ordenados dentro de cada materia.
2. Implementar `ProgresoAlumnoContenido` con los estados Pendiente, En curso, Visto, Necesita repaso y Dominado.
3. Crear los endpoints y las pantallas para administrar contenidos por materia y registrar el progreso individual de cada alumno.
4. Aplicar los mismos criterios de validación, manejo de errores, desactivación lógica y pruebas automatizadas.

El estado de un contenido siempre es individual por alumno; nunca global para todos los alumnos. No avanzar a clases hasta que contenidos y progreso estén completos y probados.
