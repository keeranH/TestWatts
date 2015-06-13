using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Econocom.Data;

namespace Econocom.Model.CustomValidation
{
    public class EmailAttribute : ValidationAttribute
    {

        private readonly EconocomDataManager _econocomDataManager;

        public EmailAttribute()
        {
             var unitOfWork = new EconocomContext();
             _econocomDataManager = new EconocomDataManager(unitOfWork);
        }
        public EmailAttribute(EconocomDataManager econocomDataManager)
        {
            _econocomDataManager = econocomDataManager;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           if (value != null)

           {
               string email = (String)value;
               var emailExist = _econocomDataManager.GetContactByEmail(email);
              
               if (emailExist != null)
               {
                   return new ValidationResult("email already exist");
               }

           }
            return ValidationResult.Success;
        }

    }
}
