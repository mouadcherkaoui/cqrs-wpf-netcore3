using CQRSExample.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CQRSExample.FrontEnd.WPF.DataAccess
{
    public class DapperMediator
    {
        IServiceProvider ServiceProvider;
        public DapperMediator(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }
        //public void Send<TCommand>(TCommand command)
        //{
        //    var commandType = typeof(TCommand);
        //    var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        //    var handler = ServiceProvider.GetRequiredService(handlerType);
        //    ((ICommandHandler<TCommand>)handler)?.Handle(command);
        //}

        public TResult Send<TQuery, TResult>(TQuery query)
            where TQuery: IQuery<TResult>
        {
            var handler = ResolveHandler<TQuery, TResult>();
            return ((IQueryHandler<TQuery, TResult>)handler).Handle(query);
        }

        public async Task<TResult> SendAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
        {
            var handler = ResolveHandler<TQuery, TResult>();
            return await ((IQueryHandler<TQuery, TResult>)handler).HandleAsync(query);
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
