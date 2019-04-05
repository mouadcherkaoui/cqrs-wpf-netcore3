using CQRSExample.Models;
using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CQRSExample.Commands
{
    public class AddCustomer: Customer, IQuery<object>
    {
    }
}
