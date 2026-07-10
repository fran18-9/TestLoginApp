# TestLoginApp
## Uso
Crear una base de datos en SQL Server Express usando la siguiente consulta:
```
CREATE DATABASE TestLoginDb;
GO

USE TestLoginDb;
GO

CREATE TABLE usuarios (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario VARCHAR(20) NOT NULL UNIQUE,
    contraseña VARCHAR(100) NOT NULL,
    nombre VARCHAR(500) NOT NULL,
    rol VARCHAR(20) NOT NULL,
    intentos INT DEFAULT 0,
    timerFin DATETIME NULL
);

-- Usuario de prueba a ingresar
INSERT INTO usuarios VALUES ('46844596', '88888888', 'Mendoza Quispe, July', 'operador', 0, NULL);
```

## Explicación Breve del Desarrollo

### Descripción del Trabajo
Se desarrolló una aplicación web funcional de autenticación utilizando el patrón **MVC (Model-View-Controller)** en **ASP.NET Core**. La aplicación consta de un flujo completo de cuatro vistas interconectadas basadas en el diseño de Figma:

Se implementó un sistema de control de acceso que valida las credenciales contra una base de datos y un mecanismo de seguridad de **bloqueo temporal de 15 minutos** si se superan los 3 intentos fallidos consecutivos.

### Enfoque y Decisiones
El proyecto se abordó bajo un enfoque orientado a resultados, priorizando la entrega de un prototipo 100% funcional y estable dentro del tiempo límite:

* Se utilizó **Bootstrap** para replicar la estructura general de las vistas de Figma, complementándolo con CSS personalizado para colores y estilos específicos.
* Interactividad del lado del cliente para mejorar la experiencia de usuario, se incluyó la funcionalidad de mostrar/ocultar contraseña.
* Toda la lógica de negocio, validaciones y consultas de datos se centralizó directamente en la capa del **Controlador** utilizando **ADO.NET** (`SqlConnection`, `SqlCommand` y `SqlDataReader`). 
    > *Nota de arquitectura:* Se es consciente de que para aplicaciones empresariales a gran escala es una mejor práctica desacoplar el código utilizando una arquitectura de capas separadas (**Controller, Service, Repository**). Sin embargo, dadas las limitaciones de tiempo, se optó por un enfoque monolítico para garantizar la robustez, mitigar errores de integración y asegurar un flujo de datos impecable.

### Herramientas y Tecnologías Utilizadas
* **Framework Principal:** ASP.NET Core (MVC) con C#
* **Diseño y Maquetación:** Figma (Análisis) y Bootstrap (Estilos)
* **Lenguaje del Cliente:** JavaScript (Manipulación del DOM)
* **Base de Datos:** Microsoft SQL Server (Instancia local SQLEXPRESS)
* **Acceso a Datos:** ADO.NET (`Microsoft.Data.SqlClient`)
* **IDE / Entorno:** Visual Studio Community
