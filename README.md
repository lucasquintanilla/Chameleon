# Voxed

Es un proyecto de codigo abierto escrito en ASP .NET Core 3.1

> https://voxed.club

## Requisitos previos

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [Mysql](https://dev.mysql.com/downloads/installer/)
- [Workbench](https://dev.mysql.com/downloads/workbench/) o [DBeaver](https://dbeaver.io/) (MySql)
- [Dbrowser (Sqlite)](https://sqlitebrowser.org/dl/)
- [FFmpeg](https://ffmpeg.org/download.html) agregar ffmpeg executables en wwwroot\ffmpeg (edit path in appsetting.json)
- [Git](https://git-scm.com/download/win)

## Instalacion

- Clonar repositorio y abrir .sln con Visual Studio
> git clone https://github.com/lucasquintanilla/Voxed.git

## Configuracion

- Seleccionar que tipo de base de datos utilizar desde appsettings.json (Mysql o Sqlite) 
- Ejectuar Voxed.WebApp

## Contribucion

Pull request son bienvenidos. Para cambios grandes abrir una issue primero.

## SSL Configuracion Cloudflare + Azure

openssl pkcs12 -inkey voxed.club.key -in voxed.club.pem -export -out voxed.club.pfx

tutorial https://swimburger.net/blog/azure/setting-up-cloudflare-full-universal-ssl-with-an-azure-app-services

## To fix

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [x] Fix buscador
- [ ] Logs en Azure
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

# Tecnologias

- ASP .NET Core 3.1
- MySQL
- Sqlite
- SignalR
- FFmpeg
- Elastic Search

# Patrones

- Coding First
- Generic Repository Pattern

# Ejemplos voxed.net

- [Id Unico](https://web.archive.org/web/20201020000307/https://www.voxed.net/off/R6X0nNN0BA6ySYDQa8EU)


