using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model
{    
    [Serializable]
    public class EconocomException : Exception
    {
        public EconocomException(string message)
            : base(message)
        {
        }
    }
}
