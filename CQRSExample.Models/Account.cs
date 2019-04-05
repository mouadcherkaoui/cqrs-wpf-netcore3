using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSExample.Models
{
    public class Account
    {
        public Guid ID { get; set; }
        public string Alias { get; set; }
        public Customer Customer { get; set; }
        public ICollection<ServiceRequest> Requests { get; set; }
    }
}
