using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel.CMS;
using Econocom.Web4.Controllers;
using Econocom.Web4.Controllers.ApiControllers;
using RTE;
using Web.Models;

namespace Econocom.Web4.Areas.Admin.Controllers
{
    public class SectionController : BaseController
    {
        private ServiceApiController service;

        public SectionController()
        {
            service = new ServiceApiController();
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
                var liste = service.GetListeSection();
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
        public PartialViewResult ModifierPage(string nomDePage, string lien, string idLangue, int idPage)
        {
            try
            {
                var contenuModererViewModel = new ContenuModereViewModel();

                var langue = service.GetLangue( Convert.ToInt32(idLangue));

                contenuModererViewModel = service.GetContenuPublier(lien, nomDePage, langue.Culture) ?? new ContenuModereViewModel { Contenu = "" };
                //contenuModererViewModel.PageId = idPage;
                var page = service.GetPage(Convert.ToInt32(idPage));
                contenuModererViewModel.Page = page;

                var langues = service.GetListeLangue();
                //ViewBag.Langues = new SelectList(langues, "Id", "Nom");

                contenuModererViewModel.ListeLangues = langues;

                /*PrepairEditor("Contenu", delegate(Editor editor)
                {
                    editor.LoadFormData(contenuModererViewModel.Contenu);
                });*/

                return PartialView("RichTextPartial", contenuModererViewModel);
            }
            catch (Exception e)
            {
                //log e
                return base.ErreurPartielle();
            }
        }

        [ValidateInput(false)]
        public string Modifier(string nomDePage, string lien, string idLangue, int idPage)
        {
            try
            {
                var contenuModererViewModel = new ContenuModereViewModel();

                var langue = service.GetLangue(Convert.ToInt32(idLangue));

                contenuModererViewModel = service.GetContenuPublier(lien, nomDePage, langue.Culture) ?? new ContenuModereViewModel { Contenu = "" };

                var page = service.GetPage(Convert.ToInt32(idPage));
                contenuModererViewModel.Page = page;

                var langues = service.GetListeLangue();
                //ViewBag.Langues = new SelectList(langues, "Id", "Nom");

                contenuModererViewModel.ListeLangues = langues;

                /* PrepairEditor("Contenu", delegate(Editor editor)
                 {
                     editor.LoadFormData(contenuModererViewModel.Contenu);
                 });*/

                return contenuModererViewModel.Contenu;
            }
            catch (Exception e)
            {
                //log e
                throw e;
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
                service.MAJContenuModere(publishedContent);
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
            var model = new SectionViewModel { IdParent = id };
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
                return Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet);
            }
        }


        public ActionResult AddPage(int id)
        {
            var model = new PageViewModel { IdSection = id };
            var listeModele = service.GetListeModele();
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
