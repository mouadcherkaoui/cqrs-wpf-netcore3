using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Commands
{
    public class DapperCommandHandler<TCommand, TResult> : IQueryHandler<TCommand, TResult>
        where TCommand: IQuery<TResult>
    {
        private IDbConnection _connection;
        private Func<TCommand, IDbConnection, TResult> _action;
        public DapperCommandHandler(Func<TCommand, IDbConnection, TResult> action)
        {
            _action = action;
            //_connection = new SqlConnection();
        }

        public TResult Handle(TCommand command)
        {
            using (_connection = new SqlConnection())
            {
                return _action(command, _connection);
            }
        }

        public async Task<TResult> HandleAsync(TCommand query)
        {
            return await Task.FromResult(_action(query, _connection));
        }

        public async Task<TResult> HandleAsync(TCommand query, CancellationToken token)
        {
            return await Task.Run<TResult>(() => _action(query, _connection));
        }
    }
}
