using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitApp.Models
{
    public class Workouts
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Workout Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(typeof(TimeSpan), "00:01:00", "05:00:00", ErrorMessage = "Duration must be between 1 minute and 5 hours.")]
        public TimeSpan Duration { get; set; }

        [Required(ErrorMessage = "Calories burned is required.")]
        [Range(1, 10000, ErrorMessage = "Calories burned must be between 1 and 10,000.")]
        public float CaloriesBurned { get; set; }
    }
}
