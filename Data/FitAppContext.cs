using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitApp.Models;

namespace FitApp.Data
{
    public class FitAppContext : DbContext
    {
        public FitAppContext (DbContextOptions<FitAppContext> options)
            : base(options)
        {
        }

        public DbSet<FitApp.Models.Workouts> Workouts { get; set; } = default!;
    }
}
