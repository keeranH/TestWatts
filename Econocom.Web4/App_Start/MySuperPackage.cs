using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(Econocom.Web4.App_Start.MySuperPackage), "PreStart")]

namespace Econocom.Web4.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            //MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}