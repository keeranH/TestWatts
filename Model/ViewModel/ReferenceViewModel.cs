using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ReferenceViewModel
    {
        public List<EntityViewModel> Entitees { get; set; }
        public Type ListType { get; set; }
        public int TemplateIndex { get; set; }
        public Type DisplayModel { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
        public int PageDimension { get; set; }
        public int Id { get; set; }
    }
}
