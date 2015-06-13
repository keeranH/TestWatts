using System;

namespace Econocom.Model.ViewModel
{
    public class InfoClientImportViewModel
    {
        //Données Client
        public string IdClient { get; set; }
        public string RaisonSociale { get; set; }
        public int TypeSociete { get; set; }
        public string Groupe { get; set; }
        public string Adresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string CodePays { get; set; }
        public int? ChiffreAffaire { get; set; }
        public string CodeSecteurActivite { get; set; }
        public int? NombreSites { get; set; }
        public Decimal? PrixMoyenKWH { get; set; }
        public Decimal CoefficientCO2 { get; set; }
        public int Effectifs { get; set; }
        public Decimal? TauxEquipement { get; set; }
        public string CodeDevise { get; set; }
        public string NomAdministrateur { get; set; }
        public string PrenomAdministrateur { get; set; }
        public string AdresseMailAdministrateur { get; set; }
        public string CodeLangue { get; set; }
        public string Erreur { get; set; }
    }
}
