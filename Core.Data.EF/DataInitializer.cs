using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Data.EF
{
    public class DataInitializer
    {
        private readonly VoxedContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public DataInitializer(VoxedContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            await InitializeTables();

            await InitializeCategories();

            await InitiliazeRoles();

            await InitializeUsers();

            //InitializeVoxs();

            //InitializeComments();
        }

        private async Task InitializeTables()
        {
            // Using this method you will not able to use Migrations
            //await _context.Database.EnsureCreatedAsync();

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any()) 
                await _context.Database.MigrateAsync();
        }

        private async Task InitiliazeRoles()
        {
            if (await _roleManager.Roles.AnyAsync()) return;

            var roles = Enum.GetValues(typeof(RoleType)).Cast<RoleType>();

            foreach (var role in roles)
            {
                var newRole = await _roleManager.FindByNameAsync(Enum.GetName(typeof(RoleType), role));
                if (newRole == null)
                {
                    newRole = new Role { Name = Enum.GetName(typeof(RoleType), role) };
                    var result = await _roleManager.CreateAsync(newRole);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Can't create role {Enum.GetName(typeof(RoleType), role)}: {string.Join(", ", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}");
                    }
                }
            }
        }

        private async Task InitializeUsers()
        {
            if (await _userManager.Users.AnyAsync())
            {
                return;
            }

            var administrator = new User
            {
                UserName = "admin",
                EmailConfirmed = true,
                UserType = UserType.Administrator
            };

            var result = await _userManager.CreateAsync(administrator, "Admin1234");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(administrator, nameof(RoleType.Administrator));
            }

            var anonymus = new User
            {
                UserName = "anonimo",
                EmailConfirmed = true,
                UserType = UserType.Anonymous
            };

            result = await _userManager.CreateAsync(anonymus, "Anonimo1234");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(anonymus, nameof(RoleType.Anonymous));
            }
        }

        private void InitializeComments()
        {
            //if (context.Comments.Any())
            //{
            //    return;   // DB has been seeded
            //}

            //var voxs = context.Voxs.ToList();

            //foreach (var vox in voxs)
            //{
            //    vox.Comments.Add(new Comment
            //    {
            //        //Hash = 
            //        Content = "Esto es un comentario a ver que onda",
            //        Bump = DateTimeOffset.Now,
            //        Media = new Media
            //        {
            //            Url = "http://web.archive.org/web/20200816001928im_/https://upload.voxed.net/thumb_8V4XHRrdXGrWaOtVhWnY.jpg",
            //            ThumbnailUrl = "http://web.archive.org/web/20200816001928im_/https://upload.voxed.net/thumb_8V4XHRrdXGrWaOtVhWnY.jpg",
            //            MediaType = MediaType.Image
            //        },
            //        UserID = vox.UserID
            //    });
            //}

            //context.SaveChanges();
        }

        private void InitializeVoxs()
        {
            if (_context.Voxs.Any())
            {
                return;   // DB has been seeded
            }

            //var vox = new Vox
            //{
            //    Title = "¿Nuevo en Voxed?",
            //    Content = "Contenido",
            //    CategoryID = 1,
            //    State = VoxState.Normal,
            //    Media = new Media
            //    {
            //        Url = "http://web.archive.org/web/20200816001928im_/https://upload.voxed.net/thumb_8V4XHRrdXGrWaOtVhWnY.jpg",
            //        MediaType = MediaType.Image
            //    },
            //    User = new User()
            //    {
            //        Username = "Anonimo"
            //    }
            //};

            //var voxs = new List<Vox>() { vox };

            //foreach (var item in voxs)
            //{
            //    context.Voxes.Add(item);
            //}

            //context.SaveChanges();
        }

        private async Task InitializeCategories()
        {
            if (await _context.Categories.AnyAsync())
            {
                return;   // DB has been seeded
            }

            var categories = new Category[]
            {
                new Category{
                    Name = "Anime",
                    ShortName = "anm",
                    Media = new Media {
                        Url = "/img/categories/anm.jpg",
                        ThumbnailUrl = "/img/categories/anm.jpg",
                        MediaType = MediaType.Image
                    },
                },
                new Category{
                    Name = "Arte",
                    ShortName = "art",
                    Media = new Media {
                        Url = "/img/categories/art.jpg",
                        ThumbnailUrl = "/img/categories/art.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Autos y Motos",
                    ShortName = "aut",
                    Media = new Media {
                        Url = "/img/categories/aut.jpg",
                        ThumbnailUrl = "/img/categories/aut.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Ciencia",
                    ShortName = "cnc",
                    Media = new Media {
                        Url = "/img/categories/cnc.jpg",
                        ThumbnailUrl = "/img/categories/cnc.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Cine",
                    ShortName = "cin",
                    Media = new Media {
                        Url = "/img/categories/cin.jpg",
                        ThumbnailUrl = "/img/categories/cin.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Consejos",
                    ShortName = "con",
                    Media = new Media {
                        Url = "/img/categories/_con.jpg",
                        ThumbnailUrl = "/img/categories/_con.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Deportes",
                    ShortName = "dpt",
                    Media = new Media {
                        Url = "/img/categories/dpt.jpg",
                        ThumbnailUrl = "/img/categories/dpt.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Download",
                    ShortName = "dwl",
                    Media = new Media {
                        Url = "/img/categories/dwl.jpg",
                        ThumbnailUrl = "/img/categories/dwl.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Economia",
                    ShortName = "eco",
                    Media = new Media {
                        Url = "/img/categories/eco.jpg",
                        ThumbnailUrl = "/img/categories/eco.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Gastronomia",
                    ShortName = "gas",
                    Media = new Media {
                        Url = "/img/categories/gas.jpg",
                        ThumbnailUrl = "/img/categories/gas.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "General",
                    ShortName = "off",
                    Media = new Media {
                        Url = "/img/categories/off.jpg",
                        ThumbnailUrl = "/img/categories/off.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Historias",
                    ShortName = "his",
                    Media = new Media {
                        Url = "/img/categories/his.jpg",
                        ThumbnailUrl = "/img/categories/his.jpg",
                        MediaType = MediaType.Image,
                    }
                },
                new Category{
                    Name = "Humanidad",
                    ShortName = "hum",
                    Media = new Media {
                        Url = "/img/categories/hum.jpg",
                        ThumbnailUrl = "/img/categories/hum.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Humor",
                    ShortName = "hmr",
                    Media = new Media {
                        Url = "/img/categories/hmr.jpg",
                        ThumbnailUrl = "/img/categories/hmr.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Juegos",
                    ShortName = "gmr",
                    Media = new Media {
                        Url = "/img/categories/gmr.jpg",
                        ThumbnailUrl = "/img/categories/gmr.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Literatura",
                    ShortName = "lit",
                    Media = new Media {
                        Url = "/img/categories/lit.jpg",
                        ThumbnailUrl = "/img/categories/lit.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Lugares",
                    ShortName = "lgr",
                    Media = new Media {
                        Url = "/img/categories/lgr.jpg",
                        ThumbnailUrl = "/img/categories/lgr.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Musica",
                    ShortName = "mus",
                    Media = new Media {
                        Url = "/img/categories/mus.jpg",
                        ThumbnailUrl = "/img/categories/mus.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Noticias",
                    ShortName = "not",
                    Media = new Media {
                        Url = "/img/categories/not.jpg",
                        ThumbnailUrl = "/img/categories/not.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Paranormal",
                    ShortName = "par",
                    Media = new Media {
                        Url = "/img/categories/par.jpg",
                        ThumbnailUrl = "/img/categories/par.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Politica",
                    ShortName = "pol",
                    Media = new Media {
                        Url = "/img/categories/pol.jpg",
                        ThumbnailUrl = "/img/categories/pol.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Preguntas",
                    ShortName = "prg",
                    Media = new Media {
                        Url = "/img/categories/prg.jpg",
                        ThumbnailUrl = "/img/categories/prg.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Programacion",
                    ShortName = "pro",
                    Media = new Media {
                        Url = "/img/categories/pro.jpg",
                        ThumbnailUrl = "/img/categories/pro.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Salud",
                    ShortName = "sld",
                    Media = new Media {
                        Url = "/img/categories/sld.jpg",
                        ThumbnailUrl = "/img/categories/sld.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Tecnologia",
                    ShortName = "tec",
                    Media = new Media {
                        Url = "/img/categories/tec.jpg",
                        ThumbnailUrl = "/img/categories/tec.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Videos (webm)",
                    ShortName = "vid",
                    Media = new Media {
                        Url = "/img/categories/vid.jpg",
                        ThumbnailUrl = "/img/categories/vid.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Youtubers",
                    ShortName = "ytb",
                    Media = new Media {
                        Url = "/img/categories/ytb.jpg",
                        ThumbnailUrl = "/img/categories/ytb.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "GTB",
                    ShortName = "gtb",
                    Media = new Media {
                        Url = "/img/categories/gtb.jpg",
                        ThumbnailUrl = "/img/categories/gtb.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Porno",
                    ShortName = "xxx",
                    Media = new Media {
                        Url = "/img/categories/xxx.jpg",
                        ThumbnailUrl = "/img/categories/xxx.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Random",
                    ShortName = "uff",
                    Media = new Media {
                        Url = "/img/categories/uff.jpg",
                        ThumbnailUrl = "/img/categories/uff.jpg",
                        MediaType = MediaType.Image
                    }
                },
                new Category{
                    Name = "Sexy",
                    ShortName = "hot",
                    Media = new Media {
                        Url = "/img/categories/hot.jpg",
                        ThumbnailUrl = "/img/categories/hot.jpg",
                        MediaType = MediaType.Image
                    }
                },
            };


            await _context.Categories.AddRangeAsync(categories);

            await _context.SaveChangesAsync();
        }
    }
}
