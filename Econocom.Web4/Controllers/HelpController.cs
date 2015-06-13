using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;
using Econocom.Web4.Controllers;
using Econocom.Web4.Controllers.ApiControllers;
using Rotativa;

namespace Econocom.Admin.Controllers
{
    public class HelpController : BaseController
    {
        private ServiceApiController service;
        private Langue langueChoisi;

        public HelpController()
        {
            this.InitLanguageDropDown();
            service = new ServiceApiController();
        }

        //
        // GET: /Help/

        public ActionResult Index()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);
                base.InitierContenu("Help/HelpPartial", "HelpPartial", langueChoisi != null ? langueChoisi.Culture : "fr-FR");
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return View();
        }

        public ActionResult HelpPartial()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);
                base.InitierContenu("Help/HelpPartial", "HelpPartial", langueChoisi != null ? langueChoisi.Culture : "fr-FR");
                return new PartialViewAsPdf();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return View();
        }

    }
}
