using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel.CMS;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;

namespace Econocom.Web4.Controllers
{
    public class ContactezNousController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ServiceApiController service;
        private Langue langueChoisi;

        public ContactezNousController()
        {
            try
            {
                this.InitLanguageDropDown();
                service = new ServiceApiController();
                ViewBag.contactezNous = "contactezNous";
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
        }

        //
        // GET: /ContactezNous/

        public ActionResult Index()
        {
            var layout = "~/Views/Shared/Maquette/_Layout_Info.cshtml";

            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);

                var administrateur = service.GetAdministrateur();
                var commercial = service.GetCommercial();

                var commercialEmail = "";
                var webMasterEmail = "";

                if (administrateur != null)
                {
                    webMasterEmail = administrateur.Email;
                }

                if (commercial != null)
                {
                    commercialEmail = commercial.Email;
                }
               
                var contenuModere = "";

                if (langueChoisi != null)
                {
                    contenuModere = base.InitierContenu("ContactezNous/Index", "Index", langueChoisi.Culture);
                }
                else
                {
                    contenuModere = base.InitierContenu("ContactezNous/Index", "Index", "fr-FR");
                }

                base.RemplacerEmail(contenuModere, commercialEmail, webMasterEmail);

                if (Request.IsAuthenticated)
                {
                    layout = "~/Views/Shared/Maquette/_Layout_Client.cshtml";
                }
                else if (Session["userType"] != null)
                {
                    if ((string)Session["userType"] == "Demo")
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
