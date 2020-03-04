using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Property
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int object_type { get; set; }
        [Required]
        public int object_id { get; set; }
        [Display(Name="property name")]
        [Required(ErrorMessage="Property name is required")]
        public string name { get; set; }
        [Display(Name="property value")]
        [Required(ErrorMessage="Property value is required")]
        public string value { get; set; }
    }
}