using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ReferenceUnitaireViewModel
    {
        public string nomObjet { get; set; }
        public int Id { get; set; }
        public List<TypeObjet> TypeObjets { get; set; }
    }
}
