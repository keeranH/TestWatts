using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model;
using Model.Interfaces;
using Omu.ValueInjecter;

namespace Infrastructure.Builder
{
    public class NullDateInjection : ConventionInjection
    {

        protected override bool Match(ConventionInfo c)
        {
            var result = c.SourceProp.Name == c.TargetProp.Name &&                 
                (c.SourceProp.Type == typeof(DateTime) || c.TargetProp.Type == typeof(DateTime) || Nullable.GetUnderlyingType(c.SourceProp.Type) == typeof(DateTime?) || Nullable.GetUnderlyingType(c.TargetProp.Type) == typeof(DateTime?));
            return result;
        }


        protected override object SetValue(ConventionInfo c)
        {
            if (c.SourceProp.Value == null || c.SourceProp.Value.Equals(new DateTime()))
                return null;
            return c.SourceProp.Value;
        }
    }
}
