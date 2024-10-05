using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FitApp.Models;

namespace FitApp.Data
{
    public class FitAppContext : IdentityDbContext
    {
        public FitAppContext (DbContextOptions<FitAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FitApp.Models.Workouts> Workouts { get; set; } = default!;
        public virtual DbSet<FitApp.Models.UserWorkout> UserWorkouts { get; set; } = default!;
        public virtual DbSet<FitApp.Models.UserSpecificWorkout> UserSpecificWorkouts { get; set; } = default!;
    }
}
