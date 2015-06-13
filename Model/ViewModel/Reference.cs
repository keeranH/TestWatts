using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
   public class Reference
    {
        public List<object> Entitees { get; set; }
        public List<object> EntiteeCreer { get; set; }
        public List<object> EntiteeSupprimer { get; set; }
        public Type ListType { get; set; }
        public int TemplateIndex { get; set; }
        public int Id { get; set; }
    }
}
