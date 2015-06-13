using System.Collections.Generic;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Service;

namespace Econocom.Model.ViewModel
{
    public class ImportECSchangeViewModel
    {
        public InfoClientImportViewModel InfoClientImportViewModel { get; set; }

        public bool InfoClientEnErreur { get; set; }

        public List<EquipementImportViewModel> ListeEquipementImportViewModel { get; set; }

        public bool InfoEquipementEnErreur { get; set; }

        public List<ImportClientViewModel> ListeImportClientViewModel { get; set; }

        public decimal PoidsTotal { get; set; }
        
        public DocumentUpload document { get; set; }

        public bool ImportParcValide { get; set; }

        public bool ClientExistant { get; set; }

        public Client Client { get; set; }
    }
}
