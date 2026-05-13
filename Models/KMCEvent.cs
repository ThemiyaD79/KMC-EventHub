using System;
using System.ComponentModel.DataAnnotations;

namespace KNC.API.Models
{
    public class KMCEvent
    {
        [Key]
        public string EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string Category { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public string Location { get; set; }
        public string ImageUrl { get; set; }
    }
}