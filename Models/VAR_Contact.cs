using System;
using System.ComponentModel.DataAnnotations;

namespace productTracker.Models
{
    public class Contact
    {
        [Key]
        public int id { get; set; }
        public int cust_no { get; set; }
        [Required]
        [MaxLength(40)]
        public string name { get; set; }
        [MaxLength(40)]
        public string email { get; set; }
        public string phone { get; set; }
        public string company { get; set; }
        public string addr_first { get; set; }
        public string addr_second { get; set; }
        public string state { get; set; }
        public string province { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string active { get; set; }
        public string city { get; set; }
        public int distributor { get; set; }        
    }
}