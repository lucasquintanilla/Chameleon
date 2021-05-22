using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF
{
    public interface IVoxedContext : IDbContext
    {
        DbSet<Vox> Voxs { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Media> Media { get; set; }
        DbSet<Poll> Polls { get; set; }
        DbSet<Notification> Notifications { get; set; }
    }
}
