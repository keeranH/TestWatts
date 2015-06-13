using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Econocom.Model.CustomValidation
{
   public class EmailValidator:DataAnnotationsModelValidator<EmailAttribute>
   {
       private string errormsg = string.Empty;
       
       public EmailValidator(ModelMetadata metadata, ControllerContext context, EmailAttribute attribute) : base(metadata, context, attribute)
       {
           errormsg = attribute.ErrorMessage;
       }

       public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
       {
           ModelClientValidationRule validationRule = new ModelClientValidationRule();
           validationRule.ErrorMessage = errormsg;
           validationRule.ValidationType = "Email";
           return new[] {validationRule};
       }
   }
}
