using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Web.Models
{
    public static class DropDownList
    {
        public static MvcHtmlString GenericDropDownList(this HtmlHelper html, string expression)
        {
            var model = html.ViewData.Model;
            var propertyMetadata =
                html.ViewData.ModelMetadata.Properties.Single(x => x.PropertyName == expression);

            //Retrives the SourcePropertyName from DropDown Attribute
            var selectList = new SelectList("","");
            if (propertyMetadata.AdditionalValues.Count > 0)
            {
                if (propertyMetadata.AdditionalValues["SourcePropertyName"] != null)
                {
                    var dataSourcePropertyName = propertyMetadata.AdditionalValues["SourcePropertyName"].ToString();

                    //Fetch the current instance/value of the SourceProperty Name
                    System.Reflection.PropertyInfo propertyInfo = model.GetType().GetProperty(dataSourcePropertyName);

                    selectList = (SelectList) propertyInfo.GetValue(model, null);
                    return html.DropDownList(propertyMetadata.PropertyName, selectList, propertyMetadata.DisplayName);
                }
            }
            //Generate the DropDownList
            return html.DropDownList(propertyMetadata.PropertyName, selectList, propertyMetadata.DisplayName);
        }
    }

}