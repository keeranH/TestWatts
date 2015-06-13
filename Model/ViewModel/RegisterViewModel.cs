using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class RegisterViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int QuestionId { get; set; }       

        [Required]
        public string Answer { get; set; }

        public int ClientId { get; set; }
    }
}
