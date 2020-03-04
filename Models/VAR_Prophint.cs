using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Prophint
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        [MaxLength(25)]
        public string pndesc { get; set; }
    }
}