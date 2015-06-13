using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class DocumentViewModel:EntityViewModel
    {
        public string Nom { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public string Repertoire { get; set; }

        public List<Document> ListeDocuments { get; set; }
    }
}
