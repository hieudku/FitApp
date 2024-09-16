using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitApp.Models
{
    public class Workouts
    {
        public int Id { get; set; }
        public string ? Name { get; set; }
        public string ? Description { get; set; }
        public TimeSpan Duration { get; set; }
        public float CaloriesBurned { get; set; }
    }
}
