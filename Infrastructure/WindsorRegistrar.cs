using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace Infrastructure
{
    public class WindsorRegistrar
    {
        public static void Register(Type interfaceType, Type implementationType)
        {
            IoC.Container.Register(Component.For(interfaceType).ImplementedBy(implementationType).LifeStyle.PerWebRequest);
        }

        public static void RegisterAllFromAssemblies(string a)
        {
            IoC.Container.Register(AllTypes.FromAssemblyNamed(a).Pick()
                                 .WithService.DefaultInterface()
                                .Configure(c => c.LifeStyle.PerWebRequest));
        }

        public static void RegisterDatabaseContext(Type interfaceType, Type implementationType, string connectionString)
        {
            IoC.Container.Register(
                Component.For(interfaceType)
                         .ImplementedBy(implementationType)
                         .LifeStyle.PerWebRequest.DependsOn(Parameter.ForKey("connectionString").Eq(connectionString)));

        }
    }
}
