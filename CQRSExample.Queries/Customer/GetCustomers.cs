using CQRSExample.Infrastructure;
using CQRSExample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSExample.Queries
{
    public class GetCustomers: IQuery<Customer[]>
    {

    }
}
