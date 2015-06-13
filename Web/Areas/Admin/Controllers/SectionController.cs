using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Model;
using Model.Interfaces;
using System.Collections;
using RTE;
using Web.Areas.Admin.Models;
using Web.Controllers;
using Web.Models;
using Web.ServiceReference1;
using IEconocomService = Econocom.Model.Interfaces.IEconocomService;

namespace Web.Areas.Admin.Controllers
{    public class SectionController : BaseController
    {
        private Web.ServiceReference1.IEconocomService service;

        public SectionController()
        {
            service = new EconocomServiceClient();
        }

        protected Editor PrepairEditor(string propertyName, Action<Editor> oninit)
        {
            Editor editor = new Editor(System.Web.HttpContext.Current, propertyName);

            editor.ClientFolder = "/richtexteditor/";
            //editor.ClientFolder = "/Content/richtexteditor/";
            //editor.ClientFolder = "/Scripts/richtexteditor/";

            //editor.Text = "Type here";

            editor.AjaxPostbackUrl = Url.Action("EditorAjaxHandler");

            if (oninit != null) oninit(editor);

            //try to handle the upload/ajax requests
            bool isajax = editor.MvcInit();

            if (isajax)
                return editor;

            //load the form data if any
            if (this.Request.HttpMethod == "POST")
            {
                string formdata = this.Request.Form[editor.Name];
                if (formdata != null)
                    editor.LoadFormData(formdata);
            }

            //render the editor to ViewBag.Editor
            ViewBag.Editor = editor.MvcGetString();

            return editor;
        }

        // GET: /Section/Pages
        public ActionResult Pages()
        {
            try
            {
                var liste = service.ListeSection();
                return View(liste);
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
           
        }


        //this action is specified by editor.AjaxPostbackUrl = Url.Action("EditorAjaxHandler");
        //it will handle the editor dialogs Upload/Ajax requests
        [ValidateInput(false)]
        public ActionResult EditorAjaxHandler()
        {
            PrepairEditor("Contenu", delegate(Editor editor)
            {

            });
            return new EmptyResult();
        }

        //
        // GET: /Section/ModifierPage
        [ValidateInput(false)]
        public PartialViewResult ModifierPage(string nomDePage)
        {
            try
            {
                var publishedContent = service.GetContenuPublier(nomDePage) ?? new ContenuModere {Contenu = ""};

                PrepairEditor("Contenu", delegate(Editor editor)
                {
                    editor.LoadFormData(publishedContent.Contenu);
                });
                
                return PartialView("RichTextPartial", publishedContent);
            }
            catch (Exception e)
            {
                //log e
                return base.ErreurPartielle();
            }
        }
    
        //
        // POST: /Section/ActualiserContentu
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ActualiserContentu(ContenuModere publishedContent)
        {
           try
           {
               service.ActualiserContenuPublier(publishedContent);
               return RedirectToAction("Pages", "Section");
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }

        public ActionResult Edit(int id)
        {
            var model = new SectionViewModel {IdParent = id};
            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(SectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = service.SauvegardeSection(model);
                return Json(JsonResponseFactory.SuccessResponse(model), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Error"),JsonRequestBehavior.DenyGet);
            }
        }

        
        public ActionResult AddPage(int id)
        {
            var model = new PageViewModel { IdSection = id };
            var listeModele = service.ListeModele();
            ViewBag.IdModele = listeModele;
            return View(model);
        }

        [HttpPost]
        public JsonResult AddPage(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = service.SauvegardePage(model);
                return Json(JsonResponseFactory.SuccessResponse(model), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet);
            }
        }
    }
}
