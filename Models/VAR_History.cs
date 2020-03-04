using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class History
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string entrytime { get; set; }
        [Required]
        public string worder { get; set; }
        public string eventdate { get; set; }
        [Required]
        [MaxLength(20)]
        public string eventdesc { get; set; }
        [Required]
        [MaxLength(11)]
        public int instid { get; set; }
        public string contact { get; set; }
        public string detail { get; set; }
        public string who { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public string po_rma { get; set; }
        public string complaint_id { get; set; }
    }
}