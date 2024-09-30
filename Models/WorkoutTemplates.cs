using Microsoft.EntityFrameworkCore;
using FitApp.Data;
using System;
using System.Linq;


namespace FitApp.Models
{
    public static class WorkoutTemplates
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FitAppContext(
                serviceProvider.GetRequiredService<
                DbContextOptions<FitAppContext>>()))
            {
                if (context.Workouts.Any())
                {
                    return;
                }
                context.Workouts.AddRange(
        new Workouts
        {
            Name = "Bench Press",
            Description = "Chest workout focusing on strength.",
            Duration = new TimeSpan(0, 45, 0),
            CaloriesBurned = 200
        },
        new Workouts
        {
            Name = "Deadlift",
            Description = "Full-body strength exercise focusing on lower back and hamstrings.",
            Duration = new TimeSpan(0, 40, 0),
            CaloriesBurned = 250
        },
        new Workouts
        {
            Name = "Squat",
            Description = "Leg workout focusing on quads, glutes, and hamstrings.",
            Duration = new TimeSpan(0, 50, 0),
            CaloriesBurned = 300
        },
        new Workouts
        {
            Name = "Overhead Press",
            Description = "Shoulder workout focusing on deltoids.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 150
        },
        new Workouts
        {
            Name = "Barbell Row",
            Description = "Back workout focusing on lats and traps.",
            Duration = new TimeSpan(0, 40, 0),
            CaloriesBurned = 200
        },
        new Workouts
        {
            Name = "Bicep Curl",
            Description = "Arm workout focusing on biceps.",
            Duration = new TimeSpan(0, 25, 0),
            CaloriesBurned = 100
        },
        new Workouts
        {
            Name = "Tricep Extension",
            Description = "Arm workout focusing on triceps.",
            Duration = new TimeSpan(0, 20, 0),
            CaloriesBurned = 90
        },
        new Workouts
        {
            Name = "Leg Press",
            Description = "Leg workout focusing on quads.",
            Duration = new TimeSpan(0, 35, 0),
            CaloriesBurned = 180
        },
        new Workouts
        {
            Name = "Lat Pulldown",
            Description = "Back workout focusing on lats.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 160
        },
        new Workouts
        {
            Name = "Chest Fly",
            Description = "Chest workout focusing on pectorals.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 140
        },
        new Workouts
        {
            Name = "Cable Cross",
            Description = "Chest workout focusing on upper and lower pectorals.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 130
        },
        new Workouts
        {
            Name = "Shoulder Press Machine",
            Description = "Shoulder workout on a machine.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 120
        },
        new Workouts
        {
            Name = "Treadmill Run",
            Description = "Cardio workout on the treadmill.",
            Duration = new TimeSpan(0, 60, 0),
            CaloriesBurned = 400
        },
        new Workouts
        {
            Name = "Elliptical Trainer",
            Description = "Low-impact cardio workout.",
            Duration = new TimeSpan(0, 45, 0),
            CaloriesBurned = 300
        },
        new Workouts
        {
            Name = "Rowing Machine",
            Description = "Full-body cardio workout.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 250
        },
        new Workouts
        {
            Name = "Leg Curl",
            Description = "Leg workout focusing on hamstrings.",
            Duration = new TimeSpan(0, 25, 0),
            CaloriesBurned = 110
        },
        new Workouts
        {
            Name = "Leg Extension",
            Description = "Leg workout focusing on quadriceps.",
            Duration = new TimeSpan(0, 25, 0),
            CaloriesBurned = 100
        },
        new Workouts
        {
            Name = "Plank",
            Description = "Core workout focusing on abs and lower back.",
            Duration = new TimeSpan(0, 10, 0),
            CaloriesBurned = 50
        },
        new Workouts
        {
            Name = "Dumbbell Lunges",
            Description = "Leg workout focusing on quads and glutes.",
            Duration = new TimeSpan(0, 30, 0),
            CaloriesBurned = 150
        },
        new Workouts
        {
            Name = "Pull-Ups",
            Description = "Upper body workout focusing on lats and biceps.",
            Duration = new TimeSpan(0, 20, 0),
            CaloriesBurned = 120
        }
    );

                context.SaveChanges();
            }
        }
    }
}
