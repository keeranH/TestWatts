using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Business.Service;
using Econocom.Model.Models.CMS;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Controllers.Contenu
{
    public class FaqController : BaseController
    {
        private ServiceApiController service;
        private Langue langueChoisi;

        public FaqController()
        {
            try
            {
                this.InitLanguageDropDown();
                service = new ServiceApiController();                
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
        }

        //
        // GET: /Faq/

        public ActionResult Index()
        {
            var layout = "~/Views/Shared/Maquette/_Layout_Info.cshtml";
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);

                if (langueChoisi != null)
                {
                    base.InitierContenu("Faq/Index", "Index", langueChoisi.Culture);
                }
                else
                {
                    base.InitierContenu("Faq/Index", "Index", "fr-FR");
                }

                if (Request.IsAuthenticated)
                {
                    layout = "~/Views/Shared/Maquette/_Layout_Client.cshtml";
                }
                else if (Session["userType"] != null)
                {
                    if ((string) Session["userType"] == "Demo")
                    {
                        layout = "~/Views/Shared/Maquette/_Layout_Evaluation.cshtml";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View("index", layout);
        }

    }
}
