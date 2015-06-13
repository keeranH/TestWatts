using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class ViewModel
    {
        [Required(ErrorMessage = "field required")]
        public string Name{ get; set; }

        [DropDown(SourcePropertyName = "StateCollection")]
        public string State { get; set; }

        [TemplatesVisibility(ShowForEdit = false, ShowForDisplay = false)]
        public SelectList StateCollection { get; set; }

        [DropDown(SourcePropertyName = "CountryCollection")]
        public string Country { get; set; }

        [TemplatesVisibility(ShowForEdit = false)]
        public SelectList CountryCollection { get; set; }

    }
}