using System;
using Model.Interfaces;
using Omu.ValueInjecter;
using Econocom.Model;

namespace Infrastructure.Builder
{
    public class NullIntToEntity : LoopValueInjection
    {
        protected override bool TypesMatch(Type sourceType, Type targetType)
        {
            return sourceType == typeof(int?) && targetType.IsSubclassOf(typeof(Entity));
        }

        protected override object SetValue(object sourcePropertyValue)
        {
            if (sourcePropertyValue == null) return null;
            var id = ((int?) sourcePropertyValue).Value;

            dynamic repo = IoC.Resolve(typeof(IRepository<>).MakeGenericType(TargetPropType));

            return repo.Find(id);
        }
    }
}