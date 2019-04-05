using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSExample.Infrastructure
{
    public interface ICommand<T>
    {
        T Command { get; }
    }
}
