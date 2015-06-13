using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Econocom.Business.CalculatorEngine
{
    public class TypeCreator
    {
        private static AssemblyBuilder assBuilder = null;
        private static ModuleBuilder modBuilder = null;
        public static void GenerateAssemblyAndModule(string assemblyName)
        {
            if (assBuilder == null)
            {
                Debug.WriteLine("Creating AssemblyBuilder for DynamicDataRowAdapter assembly");
                AssemblyName draAssemblyName = new AssemblyName();
                draAssemblyName.Name = assemblyName;
                AppDomain thisDomain = Thread.GetDomain();
                //TODO:figure out parm list to use for isSynchronized parm = true;
                assBuilder = thisDomain.DefineDynamicAssembly(draAssemblyName, AssemblyBuilderAccess.Run);
                //assBuilder = thisDomain.DefineDynamicAssembly(draAssemblyName, AssemblyBuilderAccess.RunAndSave);

                Debug.WriteLine("Creating Module for DynamicDataRowAdapter assembly");
                modBuilder = assBuilder.DefineDynamicModule(assBuilder.GetName().Name, false);
                //modBuilder = assBuilder.	(assBuilder.GetName().Name, assBuilder.GetName().Name + ".dll", false);					
            }
        }

        public static TypeBuilder CreateType(string assemblyName, string typeName)
        {
            GenerateAssemblyAndModule(assemblyName);
            Debug.WriteLine("Creating type for table " + typeName);

            TypeBuilder typeBuilder = modBuilder.DefineType(typeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(object),null);

            return typeBuilder;
        }

    }
}
