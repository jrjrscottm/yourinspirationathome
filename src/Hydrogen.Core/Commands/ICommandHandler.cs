using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Infrastructure.Commands
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<in T, out TR>
        where T : ICommand
    {
        TR Handle(T command);
    }
}
