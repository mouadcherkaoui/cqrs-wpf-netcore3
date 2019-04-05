using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CQRSExample.Models
{
    public class Customer
    {
        [Required]
        public Guid ID { get; set; }
        [MinLength(5)]
        public string Firstname { get; set; }
        [MinLength(5), MaxLength(7)]
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
    }
}
