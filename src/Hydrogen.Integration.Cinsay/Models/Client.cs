using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Integration.Cinsay.Models
{
    public class CinsayResponse<T>
    {
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public T ResponseObject { get; set; }
    }
}
