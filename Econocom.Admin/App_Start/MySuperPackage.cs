using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(Econocom.Admin.App_Start.MySuperPackage), "PreStart")]

namespace Econocom.Admin.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}