using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Province
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(2)]
        public string code { get; set; }
        [Required]
        public string province { get; set; }
    }
}