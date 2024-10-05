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
                    Description = "1. Lie on the bench.\n2. Lower the bar to your chest.\n3. Push the bar back up.\n4. Focus on chest strength.",
                    Duration = new TimeSpan(0, 45, 0),
                    CaloriesBurned = 200
                },
                new Workouts
                {
                    Name = "Deadlift",
                    Description = "1. Stand with feet hip-width apart.\n2. Lift the barbell from the ground to hip height.\n3. Lower the bar back down.\n4. Focus on lower back and hamstrings.",
                    Duration = new TimeSpan(0, 40, 0),
                    CaloriesBurned = 250
                },
                new Workouts
                {
                    Name = "Squat",
                    Description = "1. Stand with feet shoulder-width apart.\n2. Lower hips until thighs are parallel to the ground.\n3. Stand back up.\n4. Focus on quads, glutes, and hamstrings.",
                    Duration = new TimeSpan(0, 50, 0),
                    CaloriesBurned = 300
                },
                new Workouts
                {
                    Name = "Overhead Press",
                    Description = "1. Stand with barbell at chest height.\n2. Press the barbell overhead.\n3. Lower the barbell back to chest.\n4. Focus on deltoids.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 150
                },
                new Workouts
                {
                    Name = "Barbell Row",
                    Description = "1. Bend at the waist with knees slightly bent.\n2. Pull the barbell to your chest.\n3. Lower the barbell back down.\n4. Focus on lats and traps.",
                    Duration = new TimeSpan(0, 40, 0),
                    CaloriesBurned = 200
                },
                new Workouts
                {
                    Name = "Bicep Curl",
                    Description = "1. Hold dumbbells at your thighs.\n2. Curl dumbbells to your shoulders.\n3. Lower them back down.\n4. Focus on biceps.",
                    Duration = new TimeSpan(0, 25, 0),
                    CaloriesBurned = 100
                },
                new Workouts
                {
                    Name = "Tricep Extension",
                    Description = "1. Hold a dumbbell overhead.\n2. Lower the dumbbell behind your head.\n3. Raise it back overhead.\n4. Focus on triceps.",
                    Duration = new TimeSpan(0, 20, 0),
                    CaloriesBurned = 90
                },
                new Workouts
                {
                    Name = "Leg Press",
                    Description = "1. Sit on the leg press machine.\n2. Push the platform away with your legs.\n3. Lower the platform back down.\n4. Focus on quads.",
                    Duration = new TimeSpan(0, 35, 0),
                    CaloriesBurned = 180
                },
                new Workouts
                {
                    Name = "Lat Pulldown",
                    Description = "1. Sit at the lat pulldown machine.\n2. Pull the bar down towards your chest.\n3. Release the bar slowly.\n4. Focus on lats.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 160
                },
                new Workouts
                {
                    Name = "Chest Fly",
                    Description = "1. Lie on a bench with dumbbells.\n2. Open your arms wide.\n3. Bring dumbbells together above your chest.\n4. Focus on pectorals.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 140
                },
                new Workouts
                {
                    Name = "Cable Cross",
                    Description = "1. Stand between the cable stations.\n2. Pull the cables from low to high or high to low.\n3. Return cables to starting position.\n4. Focus on upper and lower pectorals.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 130
                },
                new Workouts
                {
                    Name = "Shoulder Press Machine",
                    Description = "1. Sit on the shoulder press machine.\n2. Push the handles upward until arms are extended.\n3. Lower the handles back down.\n4. Focus on shoulders.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 120
                },
                new Workouts
                {
                    Name = "Treadmill Run",
                    Description = "1. Set the treadmill speed to a comfortable run.\n2. Run at a steady pace.\n3. Maintain posture.\n4. Focus on cardio.",
                    Duration = new TimeSpan(0, 60, 0),
                    CaloriesBurned = 400
                },
                new Workouts
                {
                    Name = "Elliptical Trainer",
                    Description = "1. Step onto the elliptical trainer.\n2. Pedal in a smooth, elliptical motion.\n3. Maintain a steady pace.\n4. Focus on low-impact cardio.",
                    Duration = new TimeSpan(0, 45, 0),
                    CaloriesBurned = 300
                },
                new Workouts
                {
                    Name = "Rowing Machine",
                    Description = "1. Sit on the rowing machine.\n2. Pull the handle towards you while pushing legs back.\n3. Return handle forward as legs bend.\n4. Full-body workout.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 250
                },
                new Workouts
                {
                    Name = "Leg Curl",
                    Description = "1. Sit on the leg curl machine.\n2. Curl legs up by pulling them towards your body.\n3. Lower legs back down.\n4. Focus on hamstrings.",
                    Duration = new TimeSpan(0, 25, 0),
                    CaloriesBurned = 110
                },
                new Workouts
                {
                    Name = "Leg Extension",
                    Description = "1. Sit on the leg extension machine.\n2. Extend your legs outward by pushing up.\n3. Lower legs back down.\n4. Focus on quadriceps.",
                    Duration = new TimeSpan(0, 25, 0),
                    CaloriesBurned = 100
                },
                new Workouts
                {
                    Name = "Plank",
                    Description = "1. Get into a plank position on your elbows and toes.\n2. Hold your body straight.\n3. Engage your core and maintain balance.\n4. Focus on abs and lower back.",
                    Duration = new TimeSpan(0, 10, 0),
                    CaloriesBurned = 50
                },
                new Workouts
                {
                    Name = "Dumbbell Lunges",
                    Description = "1. Hold dumbbells by your sides.\n2. Step forward with one leg, lowering body until knees are bent.\n3. Return to standing position.\n4. Focus on quads and glutes.",
                    Duration = new TimeSpan(0, 30, 0),
                    CaloriesBurned = 150
                },
                new Workouts
                {
                    Name = "Pull-Ups",
                    Description = "1. Grab the pull-up bar with palms facing forward.\n2. Pull your body up until chin clears the bar.\n3. Lower yourself back down.\n4. Focus on lats and biceps.",
                    Duration = new TimeSpan(0, 20, 0),
                    CaloriesBurned = 120
                }

    );

                context.SaveChanges();
            }
        }
    }
}
