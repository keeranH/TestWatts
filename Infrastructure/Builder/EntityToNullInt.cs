using System;
using Econocom.Model;
using Omu.ValueInjecter;

namespace Infrastructure.Builder
{
    public class EntityToNullInt : LoopValueInjection
    {
        protected override bool TypesMatch(Type sourceType, Type targetType)
        {
            return sourceType.IsSubclassOf(typeof (Entity)) && targetType == typeof (int?);
        }

        protected override object SetValue(object o)
        {
            if (o == null) return null;
            return (o as Entity).Id;
        }
    }
}