using CQRSExample.Infrastructure;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Commands
{
    public class AddCustomerHandler : IQueryHandler<AddCustomer, object>
    {
        string ConnectionString = "";
        IQueryHandler<AddCustomer, object> _innerHandler;
        public AddCustomerHandler()
        {
            _innerHandler = new DapperCommandHandler<AddCustomer, object>((command, connection) =>
            {
                connection.ConnectionString = ConnectionString;
                return connection.Execute("INSERT INTO CUSTOMERS (ID, Firstname, Lastname) Values(@ID,@Firstname,@Lastname)", command);
            });
        }

        public object Handle(AddCustomer command)
        {
            return (new ValidateCommandDecorator<AddCustomer, object>(_innerHandler)).Handle(command);
        }

        public Task<object> HandleAsync(AddCustomer query)
        {
            return (new ValidateCommandDecorator<AddCustomer, object>(_innerHandler)).HandleAsync(query);
        }

        public Task<object> HandleAsync(AddCustomer query, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
