# Voxed

Es un proyecto de codigo abierto escrito en ASP .NET Core 3.1 MVC

> https://voxed.club

## Requisitos previos

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [Mysql](https://dev.mysql.com/downloads/installer/)
- [Workbench](https://dev.mysql.com/downloads/workbench/) or [DBeaver](https://dbeaver.io/) (MySql)
- [Dbrowser (Sqlite)](https://sqlitebrowser.org/dl/)
- [FFmpeg](https://ffmpeg.org/download.html) agregar ffmpeg executables en wwwroot\ffmpeg (edit path in appsetting.json)
- [Git](https://git-scm.com/download/win)

## Instalacion

- Clonar repositorio y abrir .sln con Visual Studio
> git clone https://github.com/lucasquintanilla/Voxed.git

## Configuracion

- Seleccionar que tipo de base de datos utilizar desde appsettings.json (Mysql or Sqlite) 
- Ejectuar Voxed.WebApp

## Contribucion

Pull request son bienvenidos. Para cambios grandes abrir una issue primero.

## SSL Configuracion Cloudflare + Azure

openssl pkcs12 -inkey voxed.club.key -in voxed.club.pem -export -out voxed.club.pfx

tutorial https://swimburger.net/blog/azure/setting-up-cloudflare-full-universal-ssl-with-an-azure-app-services

## To fix

- [ ] Doble envio de noticacion al OP cuando responden etiquetandolo en un comentario del propio post
- [ ] areglar buscador
- [ ] Logs en Azure
- [ ] mejorar performance home
- [ ] separar front del back
- [ ] PWA
- [ ] envio de notificaciones con pwa
- [ ] Reducir consultas a la db
- [ ] Agregar roles mods
- [ ] Ocultar categorias
- [ ] Bug al raplicar comentarios, en el cometario original no aparece el tag de la respuesta
- [ ] Al hovear un tag de los comentarios se abre un una previsualizacion y no se cierra solo

# Tecnologias

- ASP .NET Core 3.1 MVC
- MySQL
- Sqlite
- SignalR
- FFmpeg
- Elastic Search

# Patrones

- Generic Repository Pattern

