# Voxed

Es un proyecto de codigo abierto escrito en ASP .NET Core 3.1

> https://voxed.club

## Requisitos previos

### Nivel Basico

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [Dbrowser (Sqlite)](https://sqlitebrowser.org/dl/) (opcional)
- [FFmpeg](https://ffmpeg.org/download.html) Descargar archivos ejectuables (.exe) (Ya no es mas requerido)
- [Git](https://git-scm.com/download/win)

### Nivel Avanzado

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [Mysql](https://dev.mysql.com/downloads/installer/)
- [Workbench](https://dev.mysql.com/downloads/workbench/) o [DBeaver](https://dbeaver.io/) (MySql) (opcional)
- [FFmpeg](https://ffmpeg.org/download.html) agregar ffmpeg executables en wwwroot\ffmpeg (edit path in appsetting.json)
- [Git](https://git-scm.com/download/win)

## Instalacion 

### Nivel Basico

- Instalar software requerido
- Clonar repositorio ejecutando comando git clone
> git clone https://github.com/lucasquintanilla/Voxed.git
- Agregar archivos ejecutable FFmpeg en wwwroot\ffmpeg  (Ya no es mas requerido)
- Abrir archivo Voxed.sln con Visual Studio
- Configurar Sqlite como proveedor de base de datos en archivo appsettings.json
> "Provider": "Sqlite" 
- Ejectuar Voxed.WebApp desde Visual Studio

### Nivel Avanzado

- Instalar software requerido, crear instancia de MySql y base de datos
- Clonar repositorio ejecutando comando git clone
> git clone https://github.com/lucasquintanilla/Voxed.git
- Agregar archivos ejecutable FFmpeg en wwwroot\ffmpeg  (Ya no es mas requerido)
- Abrir archivo Voxed.sln con Visual Studio
- Configurar MySql como proveedor de base de datos en archivo appsettings.json
> "Provider": "MySql"
- Configurar ConnectionString en appsettings.json.
> "MySql": "Server=localhost;Database=voxed;Uid=root;Pwd=Password;Allow User Variables=true;"
- Ejectuar Voxed.WebApp desde Visual Studio

## Contribucion

Pull request son bienvenidos. Para cambios grandes abrir una issue primero.

## SSL Configuracion Cloudflare + Azure

openssl pkcs12 -inkey voxed.club.key -in voxed.club.pem -export -out voxed.club.pfx

tutorial https://swimburger.net/blog/azure/setting-up-cloudflare-full-universal-ssl-with-an-azure-app-services

## To Do

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [x] Fix buscador
- [x] Logs en AWS
- [ ] Mejorar performance home
- [ ] Separar front del back
- [ ] Implementar PWA
- [ ] Envio de notificaciones push con PWA
- [ ] Reducir consultas a la db
- [ ] Agregar roles mods
- [ ] Ocultar categorias
- [x] Bug al raplicar comentarios, en el cometario original no aparece el tag de la respuesta
- [x] Al hovear un tag de los comentarios se abre un una previsualizacion y no se cierra solo
- [ ] Implementar moderacion basada en bots y AI
- [ ] Implementar hosting de imagenes en servicios externos
- [ ] Fix Light Mode
- [ ] Implementar >hide
- [x] Migrar a .NET 6
- [ ] Agregar tests

# Tecnologias

- .NET 6
- Entity Framework Core
- MySQL
- Sqlite
- SignalR
- FFmpeg
- Elastic Search
- Telegram

# Patrones

- Coding First
- Generic Repository Pattern
- DI

# Ejemplos voxed.net

- [Id Unico](https://web.archive.org/web/20201020000307/https://www.voxed.net/off/R6X0nNN0BA6ySYDQa8EU)

# EntityFramework Migrations

### Add Migration

1. Select data base provider from appsettings.json
2. Desde command console seleccionar Assembly donde guardar las migraciones
2. Ejecutar comando migracion 

> add-migration NombreDeMigracion
