using System;

namespace CQRSExample.Models
{
    public class Operation
    {
        public Guid ID { get; set; }
        public DateTime OperationDate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public ServiceRequest Request { get; set; }
    }
}