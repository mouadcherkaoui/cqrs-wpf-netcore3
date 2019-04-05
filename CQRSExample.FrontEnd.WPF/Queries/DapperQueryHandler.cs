using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.FrontEnd.WPF.Queries
{
    public class DapperQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery: IQuery<TResult>
    {
        private IDbConnection _connection;
        private Func<TQuery, IDbCommand, TResult> _action;
        public DapperQueryHandler(Func<TQuery, IDbCommand, TResult> action)
        {
            _action = action;
            //_connection = new SqlConnection();
        }
        public TResult Handle(TQuery command)
        {
            using (_connection = new SqlConnection())
            {
                return _action(command, _connection.CreateCommand());
            }
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            return await HandleAsync(query, new CancellationToken());
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken token)
        {
            return await Task.Run<TResult>(() => Handle(query), token);
        }
    }
}
