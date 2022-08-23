using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurfProjekt.Models;

namespace SurfProjekt.Data
{
    public class SurfProjektContext : DbContext
    {
        public SurfProjektContext (DbContextOptions<SurfProjektContext> options)
            : base(options)
        {
        }

        public DbSet<SurfProjekt.Models.Boards> Boards { get; set; } = default!;
    }
}
