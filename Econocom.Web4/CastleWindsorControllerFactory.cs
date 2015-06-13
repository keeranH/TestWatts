﻿using System;
using System.Linq;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.Reflection;
using System.Web.Routing;

namespace Econocom.Web4
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            this.container = container;
            var controllerTypes =
                from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof (IController).IsAssignableFrom(t)
                select t;
            foreach (var t in controllerTypes)
                container.Register(Component.For(t).LifeStyle.Transient);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
                return (IController)container.Resolve(controllerType);
            else
                return null;
        }
    }
}