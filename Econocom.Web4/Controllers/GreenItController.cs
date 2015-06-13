using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Helper.File;
using Econocom.Model.Models.CMS;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Controllers
{
    public class GreenItController : BaseController
    {
        private ServiceApiController service;
        private Langue langueChoisi;

        public GreenItController()
         {
            try
            {
                this.InitLanguageDropDown();                
                service = new ServiceApiController();
                ViewBag.greenIt = "greenIt";
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
         }

        //
        // GET: /GreenIT/

        public ActionResult Index()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);

                if (langueChoisi != null)
                {
                    base.InitierContenu("GreenIt/PartialViewOne", "PartialViewOne", langueChoisi.Culture);
                }
                else
                {
                    base.InitierContenu("GreenIt/PartialViewOne", "PartialViewOne", "fr-FR");
                }


                var folderImages = ConfigurationManager.AppSettings["GreenItImages"];
                var imageExtension = ConfigurationManager.AppSettings["ImageExtension"];
                var filesCount = FileHelper.GetNumberOfFiles(Server.MapPath(Url.Content(folderImages)), imageExtension);
                ViewBag.NumImages = filesCount;
                ViewBag.Folder = folderImages;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }

            return View();
        }


        [ValidateInput(false)]
        public PartialViewResult PartialViewOne()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);
                if (langueChoisi != null)
                {
                    base.InitierContenu("GreenIt/PartialViewOne", "PartialViewOne", langueChoisi.Culture);
                }
                else
                {
                    base.InitierContenu("GreenIt/PartialViewOne", "PartialViewOne", "fr-FR");
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return PartialView("PartialView");

        }

        [ValidateInput(false)]
        public PartialViewResult PartialViewTwo()
        {
            try
            {
                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);
                if (langueChoisi != null)
                {
                    base.InitierContenu("GreenIt/PartialViewTwo", "PartialViewTwo", langueChoisi.Culture);
                }
                else
                {
                    base.InitierContenu("GreenIt/PartialViewTwo", "PartialViewTwo", "fr-FR");
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return PartialView("PartialViewTwo");

        }

    }
}
