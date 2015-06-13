using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class JournalSouscriptionViewModel
    {
        public int IdClient { get; set; }
        public string Statut { get; set; }
        public string Pays { get; set; }
        public string Groupe { get; set; }
        public string IdentificationNational { get; set; }
        public string RaisonSociale { get; set; }
        public string NoTva { get; set; }
        public bool ClientEconocom { get; set; }
        public string MailCommercial { get; set; }
        public string Adresse { get; set; }
        public string CodePostale { get; set; }
        public string Ville { get; set; }
        public int? ChiffreAffaire { get; set; }
        public string NomSouscripteur { get; set; }
        public string PrenomSouscripteur { get; set; }
        public string FonctionSouscripteur { get; set; }
        public string MailSouscripteur { get; set; }
        public string NumeroTelephoneSouscripteur { get; set; }
        public string NomAdministrateur { get; set; }
        public string PrenomAdministrateur { get; set; }
        public string FonctionAdministrateur { get; set; }
        public string MailAdministrateur { get; set; }
        public string NumeroTelephoneAdministrateur { get; set; }
        public string MailUser1 { get; set; }
        public string MailUser2 { get; set; }
        public string MailUser3 { get; set; }
        public string MailUser4 { get; set; }
        public string MailUser5 { get; set; }
        public string Tarification { get; set; }
        public DateTime? DateSouscription { get; set; }
        public DateTime? DateValidation { get; set; }
    }
}
