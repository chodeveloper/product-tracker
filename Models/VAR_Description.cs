using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace productTracker.Models
{
    public class Description
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(25)]
        [Display(Name="model number")]
        public string pn { get; set; }
        [Required]
        [MaxLength(25)]
        [Display(Name="product name")]
        public string model { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name="description")]
        public string description { get; set; }
        [MaxLength(5)]
        public string active { get; set; }

        [NotMapped]
        public bool check { get; set; }
        
    }
}