using System;
using System.ComponentModel.DataAnnotations;

namespace KNC.API.Models
{
    public class EventRegistration
    {
        [Key]
        public int RegistrationId { get; set; } 

        [Required]
        public string EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
    public class RegistrationDTO
    {
        public int RegistrationId { get; set; }
        public string EventId { get; set; }
        public string EventTitle { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}