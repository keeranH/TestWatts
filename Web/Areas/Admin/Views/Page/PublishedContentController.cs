using System;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel.CMS;
using Model.Interfaces;
using System.Collections.Generic;
using Web.Areas.Admin.Models;
using System.Linq;
using Web.Controllers;
using Web.ServiceReference1;

namespace Web.Areas.Admin.Controllers
{
    public class PublishedContentController : BaseController
    {
        
        private Web.ServiceReference1.IEconocomService service;

        public PublishedContentController()
        {
            service = new EconocomServiceClient();
        }
        
        /*
        //
        // POST: /PublishedContent/UpdateContent
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateContent(EditPublishedContent editedPublishedContent)
        {
            //var updatedPublishedContent = service.UpdatePublishedContent(editedPublishedContent);

            return RedirectToAction("Index", "Home");
        }*/


        //
        // POST: /PublishedContent/UpdateContent
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ActualiserContentu(ContenuModere publishedContent)
        {
           try
           {
               service.ActualiserContenuPublier(publishedContent);
               return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }
    }
}
