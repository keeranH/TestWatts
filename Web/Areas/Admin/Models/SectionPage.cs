using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models
{
    public class SectionPage
    {
        public int sectionId { get; set; }
        public string name { get; set; }
        public Nullable<int> parentId { get; set; }
        public int pageId { get; set; }
        public int templateId { get; set; }
        public string relativePath { get; set; }
    }
}