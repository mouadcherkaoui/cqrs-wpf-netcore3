using CQRSExample.Infrastructure;
using CQRSExample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Queries
{
    public class GetCustomersHandler : IQueryHandler<GetCustomers, Customer[]>
    {
        IQueryHandler<GetCustomers, Customer[]> _innerHandler;
        public GetCustomersHandler()
        {
            _innerHandler = new DapperQueryHandler<GetCustomers, Customer[]>((data, command) =>
            {
                command.CommandText = "";
                return command.ExecuteReader().Parse<Customer>().ToArray();
            });
        }
        public Customer[] Handle(GetCustomers query)
        {
            return _innerHandler.Handle(query);
        }

        public async Task<Customer[]> HandleAsync(GetCustomers query)
        {
            return await HandleAsync(query, new CancellationToken());
        }

        public async Task<Customer[]> HandleAsync(GetCustomers query, CancellationToken token)
        {
            return await Task.Run<Customer[]>(() => Handle(query));
        }
    }
}
