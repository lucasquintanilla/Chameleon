using Core.Entities;
//using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Core.Data.EF
{
    public class VoxedInitializer
    {
        //protected override void Seed(VoxedContext context)
        //{
        //    //InitializeTables().GetAwaiter().GetResult();

        //    InitializeCategories(context).GetAwaiter().GetResult();
        //}

        //private async Task InitializeTables()
        //{
        //    // Using this method you will not able to use Migrations
        //    //await _context.Database.EnsureCreatedAsync();

        //    var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
        //    if (pendingMigrations.Any()) 
        //        await _context.Database.MigrateAsync();
        //}



        //private async Task InitializeCategories(VoxedContext _context)
        //{
        //    if (await _context.Categories.AnyAsync())
        //    {
        //        return;   // DB has been seeded
        //    }

        //    var categories = new Category[]
        //    {
        //        new Category{
        //            Name = "Anime",
        //            ShortName = "anm",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/anm.jpg",
        //                ThumbnailUrl = "/img/categories/anm.jpg",
        //                Type = AttachmentType.Image
        //            },
        //        },
        //        new Category{
        //            Name = "Arte",
        //            ShortName = "art",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/art.jpg",
        //                ThumbnailUrl = "/img/categories/art.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Autos y Motos",
        //            ShortName = "aut",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/aut.jpg",
        //                ThumbnailUrl = "/img/categories/aut.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Ciencia",
        //            ShortName = "cnc",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/cnc.jpg",
        //                ThumbnailUrl = "/img/categories/cnc.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Cine",
        //            ShortName = "cin",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/cin.jpg",
        //                ThumbnailUrl = "/img/categories/cin.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Consejos",
        //            ShortName = "con",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/_con.jpg",
        //                ThumbnailUrl = "/img/categories/_con.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Deportes",
        //            ShortName = "dpt",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/dpt.jpg",
        //                ThumbnailUrl = "/img/categories/dpt.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Download",
        //            ShortName = "dwl",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/dwl.jpg",
        //                ThumbnailUrl = "/img/categories/dwl.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Economia",
        //            ShortName = "eco",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/eco.jpg",
        //                ThumbnailUrl = "/img/categories/eco.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Gastronomia",
        //            ShortName = "gas",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/gas.jpg",
        //                ThumbnailUrl = "/img/categories/gas.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "General",
        //            ShortName = "off",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/off.jpg",
        //                ThumbnailUrl = "/img/categories/off.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Historias",
        //            ShortName = "his",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/his.jpg",
        //                ThumbnailUrl = "/img/categories/his.jpg",
        //                Type = AttachmentType.Image,
        //            }
        //        },
        //        new Category{
        //            Name = "Humanidad",
        //            ShortName = "hum",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/hum.jpg",
        //                ThumbnailUrl = "/img/categories/hum.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Humor",
        //            ShortName = "hmr",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/hmr.jpg",
        //                ThumbnailUrl = "/img/categories/hmr.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Juegos",
        //            ShortName = "gmr",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/gmr.jpg",
        //                ThumbnailUrl = "/img/categories/gmr.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Literatura",
        //            ShortName = "lit",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/lit.jpg",
        //                ThumbnailUrl = "/img/categories/lit.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Lugares",
        //            ShortName = "lgr",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/lgr.jpg",
        //                ThumbnailUrl = "/img/categories/lgr.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Musica",
        //            ShortName = "mus",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/mus.jpg",
        //                ThumbnailUrl = "/img/categories/mus.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Noticias",
        //            ShortName = "not",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/not.jpg",
        //                ThumbnailUrl = "/img/categories/not.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Paranormal",
        //            ShortName = "par",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/par.jpg",
        //                ThumbnailUrl = "/img/categories/par.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Politica",
        //            ShortName = "pol",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/pol.jpg",
        //                ThumbnailUrl = "/img/categories/pol.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Preguntas",
        //            ShortName = "prg",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/prg.jpg",
        //                ThumbnailUrl = "/img/categories/prg.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Programacion",
        //            ShortName = "pro",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/pro.jpg",
        //                ThumbnailUrl = "/img/categories/pro.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Salud",
        //            ShortName = "sld",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/sld.jpg",
        //                ThumbnailUrl = "/img/categories/sld.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Tecnologia",
        //            ShortName = "tec",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/tec.jpg",
        //                ThumbnailUrl = "/img/categories/tec.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Videos (webm)",
        //            ShortName = "vid",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/vid.jpg",
        //                ThumbnailUrl = "/img/categories/vid.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Youtubers",
        //            ShortName = "ytb",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/ytb.jpg",
        //                ThumbnailUrl = "/img/categories/ytb.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "GTB",
        //            ShortName = "gtb",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/gtb.jpg",
        //                ThumbnailUrl = "/img/categories/gtb.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Porno",
        //            ShortName = "xxx",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/xxx.jpg",
        //                ThumbnailUrl = "/img/categories/xxx.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Random",
        //            ShortName = "uff",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/uff.jpg",
        //                ThumbnailUrl = "/img/categories/uff.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //        new Category{
        //            Name = "Sexy",
        //            ShortName = "hot",
        //            Attachment = new Attachment {
        //                Url = "/img/categories/hot.jpg",
        //                ThumbnailUrl = "/img/categories/hot.jpg",
        //                Type = AttachmentType.Image
        //            }
        //        },
        //    };


        //    await _context.Categories.AddRangeAsync(categories);

        //    await _context.SaveChangesAsync();
        //}
    }
}
