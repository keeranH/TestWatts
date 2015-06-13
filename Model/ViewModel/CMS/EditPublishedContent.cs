using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;

namespace Econocom.Model.ViewModel.CMS
{
    public class EditPublishedContent
    {
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public List<ContenuModere> ContenuModeres { get; set; }
    }
}