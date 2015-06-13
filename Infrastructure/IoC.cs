using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;


namespace Infrastructure
{
    public sealed class IoC
    {
        private static readonly object LockObj = new object();

        //private static IUnityContainer container;
        private static IWindsorContainer container;

        private static IoC instance = new IoC();

        private IoC()
        {
            //container = new UnityContainer();
            container = new WindsorContainer();
        }

        //public static IUnityContainer Container
        //{
        //    get { return container; }

        //    set
        //    {
        //        lock (LockObj)
        //        {
        //            container = value;
        //        }
        //    }
        //}

        public static IWindsorContainer Container
        {
            get { return container; }

            set
            {
                lock (LockObj)
                {
                    container = value;
                }
            }

        }

        public static IoC Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (LockObj)
                    {
                        if (instance == null)
                        {
                            instance = new IoC();
                        }
                    }
                }

                return instance;
            }
        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return container.Resolve(type);
        }
    }
}
