using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ReponseViewModel
    {
        public string Valeur { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}

