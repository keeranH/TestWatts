using System;

namespace Econocom.Model.ViewModel
{
    public class EquipementImportViewModel
    {
        //Données Equipements Flow / ECSchange
        public string CodeCategorie { get; set; } //imp 1
        public string LibelleCategorie { get; set; }
        public string CodeFamille { get; set; } //imp 3
        public string LibelleFamille { get; set; }
        public string CodeSousFamille { get; set; } //imp 5
        public string LibelleSousFamille { get; set; }
        public string RefECS { get; set; }
        public string LibelleRefECS { get; set; } //imp 8
        public string LibelleMarque { get; set; }
        public string LibelleGamme { get; set; }
        public string LibelleModele { get; set; }
        public string NbProcesseurs { get; set; }
        public string CodeProcesseur { get; set; } //imp 13
        public string LibelleCodeProcesseur { get; set; }
        public string LibelleTypeProcesseur { get; set; }
        public string Frequence { get; set; }
        public string UniteFrequence { get; set; }
        public string LibelleUniteFrequence { get; set; }
        public string Memoire { get; set; }
        public string UniteMemoire { get; set; }
        public string LibelleUniteMemoire { get; set; }
        public Decimal Taille { get; set; } //imp 22
        public string UniteTaille { get; set; } //imp 23
        public Decimal Vitesse { get; set; } //imp 24
        public string UniteVitesse { get; set; } //imp 25
        public string LibelleVitesse { get; set; }
        public string Couleur { get; set; }
        public string RectoVerso { get; set; }
        public string NormeGraphique { get; set; }
        public string CodeResolution { get; set; }
        public string LibelleResolution { get; set; }
        public Decimal Poids { get; set; } //imp 32
        public string UnitePoids { get; set; } //imp 33
        public string LibelleUnitePoids { get; set; }
        public string CodeTechnologie { get; set; }
        public string NombreVoies { get; set; }
        public Decimal Puissance { get; set; } //imp 37
        public string UnitePuissance { get; set; } //imp 38
        public DateTime DateFacture { get; set; } //imp 39
        public int Quantite { get; set; } //imp 40
        public string Erreur { get; set; }
        public int? Caracteristique { get; set; }

        //Données Watts Green
        public int TypeDeviceId { get; set; }
        public int ClasseDeviceId { get; set; }
        public int AgeDeviceId { get; set; }
    }
}
