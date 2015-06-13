using Econocom.Model.Enum;

namespace Econocom.Model.ViewModel
{
    public class MesInformationsViewModel
    {
        public SocieteSouscriptionViewModel SocieteSouscriptionViewModel { get; set; }

        public EvaluationEntrepriseViewModel DetailsEntrepriseViewModel { get; set; }

        public UtilisateurSouscriptionViewModel UtilisateurSouscriptionViewModel { get; set; }

        public int TypeClient { get; set; }
    }
}
