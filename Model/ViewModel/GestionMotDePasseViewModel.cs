using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Resource;

namespace Econocom.Model.ViewModel
{
    public class GestionMotDePasseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Econocom.Resource.Traduction), ErrorMessageResourceName = "Error_Msg_Mdp")]
        public string MotDePasse { get; set; }

        [Required(ErrorMessageResourceType = typeof(Econocom.Resource.Traduction), ErrorMessageResourceName = "Error_Msg_MdpConfirmer")]
        public string MotDePasseConfirme { get; set; }

        [Required(ErrorMessageResourceType = typeof(Econocom.Resource.Traduction), ErrorMessageResourceName = "Error_Msg_Reponse")]
        public string ReponseSaisie { get; set; }

        public Nullable<int> QuestionId { get; set; }
        
        public virtual Question Question { get; set; }
       
        public virtual List<Question> Questions { get; set; }

        public string Email { get; set; }

        public string CodeVerification { get; set; }
    }
}
