using CQRSExample.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace CQRSExample.Commands
{
    public class TransactionCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decorated;

        public TransactionCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated)
        {
            this.decorated = decorated;
        }

        public void Handle(TCommand command)
        {
            using (var scope = new TransactionScope())
            {
                this.decorated.Handle(command);

                scope.Complete();
            }
        }
    }
}
