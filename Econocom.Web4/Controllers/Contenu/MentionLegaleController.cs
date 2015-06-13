using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Controllers.Contenu
{
    public class MentionLegaleController : BaseController
    {
        private ServiceApiController service;
        private Langue langueChoisi;

        public MentionLegaleController()
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
        // GET: /MentionLegale/

        public ActionResult Index()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);

                if (langueChoisi != null)
                {
                    base.InitierContenu("MentionLegale/Index", "Index", langueChoisi.Culture);
                }
                else
                {
                    base.InitierContenu("MentionLegale/Index", "Index",  "fr-FR");
                }
            }
            catch (Exception)
            {
                throw;
            }


            return View();
        }

    }
}
