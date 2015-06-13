using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Common;
using Econocom.Web4.Controllers;
using Infrastructure;
using Infrastructure.Builder;
using Infrastructure.DTO;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using Omu.Awesome.Mvc;
using Unity.Mvc4;

namespace Econocom.Web4
{
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {            
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IoC.Container));
            WindsorConfigurator.Configure();
            ModelMetadataProviders.Current = new AwesomeModelMetadataProvider();

            Settings.Confirm.NoText = "No";
            Settings.PopupForm.OkText = "Submit";
            Settings.PopupForm.ClientSideValidation = true;
        }
    }
}