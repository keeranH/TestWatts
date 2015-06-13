using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Interfaces;
using Omu.ValueInjecter;

namespace Infrastructure.Builder
{
    public class NullableInjection : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            var result =  c.SourceProp.Name == c.TargetProp.Name &&
              ( c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type)
              || (Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type && c.SourceProp.Value != null)
              );
            var sourceVal = c.SourceProp.Value;
            var targetVal = c.TargetProp.Value;
            return result;
        }

        protected override object SetValue(ConventionInfo c)
        {
            return c.SourceProp.Value;
        }
    }
}
