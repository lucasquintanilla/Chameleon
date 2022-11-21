# Voxed

Proyecto free, cross-platform y open-source

> https://voxed.club

# Tech Stack

- .NET 6
- PostgreSQL
- MySQL
- Sqlite
- Entity Framework Core
- SignalR
- Telegram

## Requisitos previos

### Software requerido

- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/)
- [Git](https://git-scm.com/download/win)

### Software opcional

- [Mysql Community](https://dev.mysql.com/downloads/)
- [DB Browser for SQLite](https://sqlitebrowser.org/dl/)
- [Workbench](https://dev.mysql.com/downloads/workbench/)
- [DBeaver](https://dbeaver.io/)
- [pgAdmin](https://www.pgadmin.org/download/)

## Instalacion 

- Instalar Visual Studio
- Clonar repositorio ejecutando comando git clone
> git clone https://github.com/.../Voxed.git
- Abrir archivo Voxed.sln con Visual Studio
- Ejectuar Voxed.WebApp desde Visual Studio

## Contribucion

Pull request son bienvenidos

## Bugs

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [x] Fix buscador
- [ ] Fix Light Mode / Dark Mode
- [x] Ocultar voxs
- [x] Seguir categorias
- [x] Bug al replicar comentarios, en el cometario original no aparece el tag de la respuesta
- [x] Al hovear un tag de los comentarios se abre un una previsualizacion y no se cierra solo
- [x] Implementar >hide

## Tech Debt

- [x] Logs
- [x] Migrar a .NET 6
- [ ] Agregar unit tests
- [ ] Reducir consultas a la DB
- [ ] Mejorar performance home
- [ ] Agregar roles mods
- [ ] Agregar documentacion
- [ ] Agregar automapper
- [ ] Limpiar estructura del proyecto

## Future features

- [ ] Implementar videos
- [ ] Separar frontend del backend
- [ ] Implementar instalable PWA using web assembly
- [ ] Envio de notificaciones push con PWA
- [ ] Eliminar categorias e implementar Tags
- [ ] Implementar Material Design 3
- [ ] Implementar moderacion basada en bots y AI services

# Patrones de dise�o

- Code First
- Generic Repository Pattern
- Unit of Work
- DI
- Options Pattern
- MVC

# Principios a seguir
- SOLID
- DRY
- Composition over inheritance

# Ejemplos voxed.net

- [Id Unico](https://web.archive.org/web/20201020000307/https://www.voxed.net/off/R6X0nNN0BA6ySYDQa8EU)

# EntityFramework Migrations

### Add Migration

1. Especificar database provider from appsettings.json
2. Desde command console seleccionar Assembly donde guardar las migraciones
2. Ejecutar comando migracion 

> add-migration NombreDeMigracion
