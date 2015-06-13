using System.Configuration;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using Unity.Wcf;

namespace Econocom.Service
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            // register all your components with the container here
            // container
            //    .RegisterType<IService1, Service1>()
            //    .RegisterType<DataContext>(new HierarchicalLifetimeManager());
           container = container.RegisterType<Business.Service.BusinessService>();
           var connectionString = ConfigurationManager.ConnectionStrings["EconocomContext"].ConnectionString;
           container.RegisterType<IUnitOfWork, EconocomContext>(new InjectionConstructor(connectionString));
           //var CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSContext"].ConnectionString;
           //container.RegisterType<IUnitOfWork, CMSContext>(new InjectionConstructor(CMSConnectionString));

           // e.g. container.RegisterType<ITestService, TestService>(); 
           //container.RegisterType(typeof(IUnitOfWork), typeof(EconocomContext));
           container.RegisterType(typeof(IRepository<>), typeof(Repository<>));
          // container.RegisterType(typeof(IRepository<>), typeof(CMSRepository<>));    
            
        }
    }    
}