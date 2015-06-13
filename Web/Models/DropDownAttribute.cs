using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DropDownAttribute : Attribute, IMetadataAware
    {
        public string SourcePropertyName { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("SourcePropertyName", this.SourcePropertyName);
            metadata.TemplateHint = "DropDownList";
        }
    }

}