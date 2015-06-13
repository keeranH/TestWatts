using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Error
{
    public class Resultat
    {
        public bool Succes { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
