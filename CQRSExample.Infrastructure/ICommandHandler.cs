using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSExample.Infrastructure
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}
