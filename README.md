
# Sistema de Administración de Gimnasio

## Descripción

Sistema web desarrollado para administrar los clientes, planes de membresía, membresías, pagos y asistencias de un gimnasio. Cuenta con control de acceso mediante usuarios y roles.

## Integrantes

- Andrey Artavia Arce
- Randy Fabián Hernández Suárez

## Tecnologías utilizadas

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- LINQ
- SQL Server
- Bootstrap
- Visual Studio 2026
- GitHub

## Módulos del sistema

- Inicio de sesión.
- Administración de usuarios.
- Administración de roles.
- Administración de clientes.
- Administración de planes de membresía.
- Administración de membresías.
- Administración de pagos.
- Administración de asistencias.

## Roles del sistema

### Administrador

Tiene acceso a todos los módulos del sistema.

### Empleado

Tiene acceso a los módulos de clientes, membresías, pagos y asistencias.

## Seguridad

El sistema utiliza autenticación mediante cookies, protección de contraseñas mediante hash y restricciones de acceso según el rol del usuario.

## Base de datos

El sistema utiliza SQL Server y la base de datos SistemaGimnasioDB.

La cadena de conexión se encuentra configurada en el archivo appsettings.json y debe ajustarse según el servidor SQL de cada equipo.

## Instalación y ejecución

1. Clonar o descargar el repositorio.
2. Abrir la solución en Visual Studio 2026.
3. Crear la base de datos en SQL Server.
4. Configurar la cadena de conexión en appsettings.json.
5. Restaurar los paquetes NuGet.
6. Ejecutar el proyecto presionando F5.

## Información académica

- Universidad Católica de Costa Rica.
- Curso: Lenguajes Avanzados.
- Profesor: MSc. Allam Mauricio Fernández Rivera.
- Año: 2026.
