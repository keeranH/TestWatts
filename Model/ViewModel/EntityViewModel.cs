using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Econocom.Model.ViewModel
{
    public class EntityViewModel
    {
       // [CsvField(Index = 0)]
        public int? Id { get; set; }

        //[CsvField(Ignore = true)]
        public bool Supprimer { get; set; }

        public bool IsModified { get; set; }
    }
}
