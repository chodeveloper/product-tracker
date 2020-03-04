using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Eventhint
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(20)]
        public string eventname { get; set; }
        [Required]
        public string description { get; set; }
    }
}