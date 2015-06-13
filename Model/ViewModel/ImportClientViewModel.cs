
namespace Econocom.Model.ViewModel
{
    public partial class ImportClientViewModel
    {
        public int IdClient { get; set; }
        public int TypeDevice { get; set; }
        public string LibelleDevice { get; set; }
        public int? TypeUsageId { get; set; }
        public decimal? NbrJourTravaille { get; set; }
        public decimal? NbrHeuresIntensifJour { get; set; }
        public decimal? NbrHeuresNonIntensifJour { get; set; }
        public decimal? NbrHeuresOffJour { get; set; }
        public bool? UtiliseEnergieExterne { get; set; }
        public decimal? CoeffNonIntensif { get; set; }
        public decimal? CoeffModeOff { get; set; }
        public decimal? CoeffReductionWattExterne { get; set; }
        public decimal? PourcentageConsoExterne { get; set; }
        public int ClasseDevice { get; set; }
        public int AgeDevice { get; set; }
        public int QteDevice { get; set; }
        public decimal? ConsommationIntensif { get; set; }
        public decimal? ConsommationNonIntensif { get; set; }
        public decimal? ConsommationWatt { get; set; }
        public decimal? ConsommationCo2 { get; set; }
        public decimal? CoefficientConversion { get; set; }
        public decimal? ConsommationIntensifUnitaire { get; set; }
        public decimal? Poids { get; set; }
        public decimal? NewPV { get; set; }
        public int? Caracteristique { get; set; }
    }
}
