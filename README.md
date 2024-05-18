# Link Shortener API

## Descripción

Este repositorio contiene el código base para un potencial Acortador de Enlaces API. Aunque aún no está implementado como un servicio desplegado, el código proporciona una solución robusta y eficiente para transformar URLs largas y complicadas en direcciones más cortas y manejables. Este proyecto representa una oportunidad para explorar y aprender sobre las técnicas de optimización de enlaces.

## Instalación

1. Clona este repositorio en tu máquina local con `git clone https://github.com/tu-usuario/tu-repositorio.git`.
2. Navega hasta el directorio del proyecto con `cd ruta/a/tu/proyecto`.
3. Instala las dependencias necesarias con `dotnet restore`.
4. Ejecuta el proyecto con `dotnet run`.
5. Ejecuta las migraciones para crear la base de datos con:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update

## Uso

Para acortar una URL, realiza una solicitud POST a `http://localhost:5000/api/shorten` con un cuerpo JSON que contenga la URL que deseas acortar. Por ejemplo:

```json
{
    "Url": "https://www.ejemplo.com"
}
```

La API te devolverá una URL corta que puedes usar en lugar de la URL original.

Contribuciones
Las contribuciones son bienvenidas. Por favor, abre un issue para discutir lo que te gustaría cambiar o añadir.

Licencia
                    GNU GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
