using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Core.Commands
{
    public class ResultBase
    {
        public ResultBase(string erroMessage = null)
        {
            if (erroMessage != null)
            {
                Errors = new List<string> { erroMessage };
            }
        }

        public void AddError(string errorMessage)
        {
            if (Errors != null)
            {
                Errors = new List<string> { errorMessage };
            }
            else
            {
                Errors.Add(errorMessage);
            }
        }

        public List<string> Errors { get; set; }

        public bool Success { get { return Errors == null || !Errors.Any(); } }

    }
}
