//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class Reponse : Entity
    {
        //public int ReponseId { get; set; }
        public string Valeur { get; set; }
        public int QuestionId { get; set; }
        public int ContactId { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual Contact Contact { get; set; }
    }
}