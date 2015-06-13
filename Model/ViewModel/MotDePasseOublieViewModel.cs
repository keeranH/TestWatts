using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class MotDePasseOublieViewModel
    {
        public virtual Contact Contact { get; set; }

        public virtual Question Question { get; set; }

        public virtual Reponse Reponse { get; set; }

        public virtual List<Question> Questions { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string ReponseSaisie { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        public string EmailSaisi { get; set; }

    }
}

