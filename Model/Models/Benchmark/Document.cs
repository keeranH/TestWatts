using System;

namespace Econocom.Model.Models.Benchmark
{
    public class Document : Entity
    {
        public string Nom { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public string Repertoire { get; set; }
    }
}
