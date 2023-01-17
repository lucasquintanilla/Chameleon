# Voxed

Proyecto free, cross-platform y open-source

> https://voxed.club

# Tech Stack :hammer_and_wrench:

- .NET 6
- PostgreSQL
- MySQL
- Sqlite
- Entity Framework Core
- SignalR
- Telegram

## Requisitos previos :clipboard:

### Software requerido

- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/)
- [Git](https://git-scm.com/download/win)

### Software opcional

- [pgAdmin](https://www.pgadmin.org/download/)
- [Mysql Community](https://dev.mysql.com/downloads/)
- [DB Browser for SQLite](https://sqlitebrowser.org/dl/)
- [Workbench](https://dev.mysql.com/downloads/workbench/)
- [DBeaver](https://dbeaver.io/)

## Instalacion :rocket:

- Instalar Visual Studio
- Clonar repositorio ejecutando comando git clone
> git clone https://github.com/.../Voxed.git
- Abrir archivo Voxed.sln con Visual Studio
- Ejectuar Voxed.WebApp desde Visual Studio

## Contribucion :coffee:

Pull request son bienvenidos

## Bugs :bug:

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [x] Fix buscador
- [ ] Fix Light Mode / Dark Mode
- [x] Ocultar voxs
- [x] Seguir categorias
- [x] Bug al replicar comentarios, en el cometario original no aparece el tag de la respuesta
- [x] Al hovear un tag de los comentarios se abre un una previsualizacion y no se cierra solo
- [x] Implementar >hide
- [ ] Cargas mas post en vista categoria

## Tech Debt :pencil2:

- [x] Logs
- [x] Migrar a .NET 6
- [x] Migrar a postgresSQL
- [x] Multiple database providers
- [ ] Agregar unit tests
- [ ] Implementar clean code y clean architecture practices
- [ ] Reducir consultas a la DB
- [ ] Mejorar performance home
- [ ] Agregar roles mods
- [ ] Agregar documentacion
- [ ] Agregar automapper
- [ ] Limpiar estructura del proyecto

## Future features :bulb:

- [ ] Implementar videos as reels
- [ ] Implementar markdown processor. Frontmatter?
- [ ] Separar frontend del backend
- [ ] Implementar instalable PWA using web assembly / Flutter
- [ ] Implementar notificaciones push
- [ ] Eliminar categorias e implementar Tags
- [ ] Implementar Material Design 3
- [ ] Implementar moderacion basada en bots y AI cognitive services
- [ ] Post scores
- [x] External post data sources
- [ ] Board mixer based on post scores and external post sources

# Patrones de diseño

- Code First
- Repository Pattern
- Dependency Injection
- Options Pattern
- MVC

# Principios a seguir

- KISS
- SOLID
- DRY
- Composition over inheritance

# EntityFramework Migrations

### Add Migration

1. Especificar database provider from appsettings.json
2. Desde command console seleccionar Assembly donde guardar las migraciones
2. Ejecutar comando migracion 

> add-migration NombreDeMigracion
