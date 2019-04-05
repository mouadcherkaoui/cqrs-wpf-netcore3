using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSExample.Models
{
    public class ServiceRequest
    {
        public Guid ID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime ClosedOn { get; set; }
        public ICollection<Operation> Operations { get; set; }
    }
}
