using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSExample.Infrastructure
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery: IQuery<TResult>
    {
        TResult Handle(TQuery query);
        Task<TResult> HandleAsync(TQuery query);
        Task<TResult> HandleAsync(TQuery query, CancellationToken token);
    }
}
