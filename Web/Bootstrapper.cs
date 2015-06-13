using System.Configuration;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Service;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using Unity.Mvc3;

namespace Web
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            //var context = new EconocomContext();
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            //var connectionString = ConfigurationManager.ConnectionStrings["EconocomContext"].ConnectionString;
           // container.RegisterType<IUnitOfWork, EconocomContext>(new InjectionConstructor(connectionString));

            // e.g. container.RegisterType<ITestService, TestService>(); 
            //container.RegisterType(typeof(IUnitOfWork), typeof(EconocomContext));
           // container.RegisterType(typeof (IRepository<>), typeof (Repository<>));            
            //container.RegisterInstance<EconocomContext>(context);
            
            //uncomment below when using BLL service mechanism
            container.RegisterType<Econocom.Service.IEconocomService, Econocom.Service.EconocomService>(new PerThreadLifetimeManager());
                          
           

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
               
            return container;
        }
    }
}