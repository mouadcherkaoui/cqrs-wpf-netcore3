using CQRSExample.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CQRSExample.DataAccessLayer
{
    public class DapperMediator
    {
        IServiceProvider ServiceProvider;
        public DapperMediator(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }

        public TResult Send<TQuery, TResult>(TQuery query)
            where TQuery: IQuery<TResult>
        {
            var handler = ResolveHandler<TQuery, TResult>();
            return handler.Handle(query);
        }

        public async Task<TResult> SendAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = new CancellationToken())
            where TQuery : IQuery<TResult>
        {
            var handler = ResolveHandler<TQuery, TResult>();
            return await handler.HandleAsync(query, cancellationToken);
        }

        private IQueryHandler<TQuery, TResult> ResolveHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            var queryType = typeof(TQuery);
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            var handler = ServiceProvider.GetRequiredService(handlerType);
            return (IQueryHandler<TQuery, TResult>) handler;
        }
    }
}
