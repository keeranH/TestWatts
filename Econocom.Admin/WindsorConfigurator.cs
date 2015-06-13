using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Econocom.Business;
using Econocom.Data;
using Econocom.Model;
using Econocom.Model.Interfaces;
using Infrastructure;
using Infrastructure.Builder;
using Infrastructure.DTO;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using Castle.MicroKernel.Registration;

namespace Econocom.Admin
{
    public class WindsorConfigurator
    {
        public static void Configure()
        {
            //WindsorRegistrar.Register(typeof(ICrudService<Foobar>), typeof(FoobarService));
            var connectionString = ConfigurationManager.ConnectionStrings["EconocomContext"].ConnectionString;
            WindsorRegistrar.RegisterDatabaseContext(typeof(IUnitOfWork), typeof(EconocomContext), connectionString);
            WindsorRegistrar.Register(typeof(IRepository<>), typeof(Repository<>));
            WindsorRegistrar.Register(typeof(ICrudService<>), typeof(CrudService<>));
            WindsorRegistrar.Register(typeof(IBuilder<,>), typeof(Builder<,>));
                    
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Helper");
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Model");
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Resource");
            WindsorRegistrar.RegisterAllFromAssemblies("Security");
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Data");
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Business");
            WindsorRegistrar.RegisterAllFromAssemblies("Econocom.Web4");
            WindsorRegistrar.RegisterAllFromAssemblies("Infrastructure");
        }
    }
}