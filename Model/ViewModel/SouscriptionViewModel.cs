using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class SouscriptionViewModel
    {
        //public SouscriptionViewModel SouscriptionViewModelMain { get; set; }

        //public string EstSouscripteurAdmin { get; set; }
        public bool EstSouscripteurAdmin { get; set; }

        [UIHint("Souscription/SocieteAffichageTemplate")]
        public SocieteSouscriptionViewModel SocieteSouscriptionViewModel { get; set; }

        [UIHint("Souscription/SouscripteurAffichageTemplate")]
        public ContactSouscriptionViewModel SouscripteurSouscriptionViewModel { get; set; }

        [UIHint("Souscription/AdministrateurAffichageTemplate")]
        public ContactSouscriptionViewModel AdministrationSouscriptionViewModel { get; set; }

        [UIHint("Souscription/UtilisateurAffichageTemplate")]
        public UtilisateurSouscriptionViewModel UtilisateurSouscriptionViewModel { get; set; }



        [UIHint("Souscription/SocieteTemplate")]
        public SocieteSouscriptionViewModel SocieteInputSouscriptionViewModel { get; set; }

        [UIHint("Souscription/SouscripteurTemplate")]
        public ContactSouscriptionViewModel SouscripteurInputSouscriptionViewModel { get; set; }

        [UIHint("Souscription/AdministrateurTemplate")]
        public ContactSouscriptionViewModel AdministrationInputSouscriptionViewModel { get; set; }

        [UIHint("Souscription/UtilisateurTemplate")]
        public UtilisateurSouscriptionViewModel UtilisateurInputSouscriptionViewModel { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public bool Accepter { get; set; }

        //used only for gestion client - admin
        public Nullable<int> TarifId { get; set; }
        public virtual Tarif Tarif { get; set; }
        public virtual List<Tarif> ListTarif { get; set; }
    }
}
