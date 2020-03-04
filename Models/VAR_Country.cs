using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Country
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(2)]
        public string code { get; set; }
        [Required]
        [MaxLength(60)]
        public string country { get; set; }
    }
}