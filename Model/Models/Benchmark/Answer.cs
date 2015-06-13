using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Econocom.Model.Models.Benchmark
{
    public class Answer : Entity
    {
        //[ForeignKey("Contact")]
        public int Id { get; set; }
        public string Value { get; set; }
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
       
    }
}
