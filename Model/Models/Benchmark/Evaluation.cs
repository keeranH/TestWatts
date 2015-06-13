using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class CompteEvaluation : Entity
    {
        public string RaisonSociale { get; set; }
        public string Groupe { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Fonction { get; set; }
        public string Email { get; set; }
        public string NumeroPhone { get; set; }
        public int Effectif { get; set; }
        public Decimal TauxEquipement { get; set; }
        public int NombreSites { get; set; }
        public Decimal PrixMoyenKwatt { get; set; }
        public Nullable<int> PaysId { get; set; }
        public Nullable<int> SecteurActiviteId { get; set; }
        public Nullable<int> DeviseId { get; set; }

        public virtual Pays Pays { get; set; }
        public virtual SecteurActivite SecteurActivite { get; set; }
        public virtual Devise Devise { get; set; }
    }
}
