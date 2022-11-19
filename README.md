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
> git clone https://github.com/lucasquintanilla/Voxed.git
- Abrir archivo Voxed.sln con Visual Studio
- Ejectuar Voxed.WebApp desde Visual Studio

## Contribucion

Pull request son bienvenidos

## To Do List

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [x] Fix buscador
- [x] Logs
- [ ] Mejorar performance home
- [ ] Separar front del back
- [ ] Implementar PWA
- [ ] Envio de notificaciones push con PWA
- [ ] Reducir consultas a la db
- [ ] Agregar roles mods
- [x] Ocultar categorias
- [x] Bug al raplicar comentarios, en el cometario original no aparece el tag de la respuesta
- [x] Al hovear un tag de los comentarios se abre un una previsualizacion y no se cierra solo
- [ ] Implementar moderacion basada en bots y AI
- [x] Implementar hosting de imagenes en servicios externos
- [ ] Fix Light Mode
- [x] Implementar >hide
- [x] Migrar a .NET 6
- [ ] Agregar unit tests
- [ ] Agregar tags
- [ ] Material Design 3

# Patrones de diseño

- Coding First
- Generic Repository Pattern
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
