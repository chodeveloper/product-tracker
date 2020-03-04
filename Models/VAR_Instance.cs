using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace productTracker.Models
{
    public class Instance
    {
        [Key]
        public int id { get; set; }
        public int pid { get; set; }
        [Display(Name="Model number")]
        [Required]
        public string pn { get; set; }
        [Display(Name="Serial number")]
        [Required]
        public string sn { get; set; }

        [NotMapped]
        public string model { get; set; }
        [NotMapped]
        public string ppn { get; set; }
        [NotMapped]
        public string psn { get; set; }   

        [NotMapped]
        public List<Instance> children { get; set; } = new List<Instance>();
        [NotMapped]
        public List<Property> properties { get; set; } = new List<Property>();  
        // [NotMapped]
        // public string selectedParent { get; set; }
        // [NotMapped]
        // public string selectedModel { get; set; }
        [NotMapped]
        public Description newDesc { get; set; }
    }
}