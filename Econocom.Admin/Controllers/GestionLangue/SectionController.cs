using System;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel.CMS;
using Econocom.Resource;
using NLog;
//using RTE;

namespace Econocom.Admin.Controllers.GestionLangue
{
    [Authorize]
    public class SectionController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public SectionController()
        {
            _service = new ServiceApiController();
        }

        //protected Editor PrepairEditor(string propertyName, Action<Editor> oninit)
        //{
        //    Editor editor = new Editor(System.Web.HttpContext.Current, propertyName);

        //    editor.ClientFolder = "/richtexteditor/";
        //    //editor.ClientFolder = "/Content/richtexteditor/";
        //    //editor.ClientFolder = "/Scripts/richtexteditor/";

        //    //editor.Text = "Type here";

        //    editor.AjaxPostbackUrl = Url.Action("EditorAjaxHandler");

        //    if (oninit != null) oninit(editor);

        //    //try to handle the upload/ajax requests
        //    bool isajax = editor.MvcInit();

        //    if (isajax)
        //        return editor;

        //    //load the form data if any
        //    if (this.Request.HttpMethod == "POST")
        //    {
        //        string formdata = this.Request.Form[editor.Name];
        //        if (formdata != null)
        //            editor.LoadFormData(formdata);
        //    }

        //    //render the editor to ViewBag.Editor
        //    ViewBag.Editor = editor.MvcGetString();

        //    return editor;
        //}

        // GET: /Section/Pages
        public ActionResult Pages()
        {
            try
            {
                var liste = _service.GetListeSection();
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
        //[ValidateInput(false)]
        //public ActionResult EditorAjaxHandler()
        //{
        //    PrepairEditor("Contenu", delegate(Editor editor)
        //    {
        //        string m = Request.QueryString["mode"];
        //        if (string.IsNullOrEmpty(m))
        //            m = "auto";
        //        switch (m)
        //        {
        //            case "auto":
        //                editor.ResizeMode = RTEResizeMode.AutoAdjustHeight;
        //                break;
        //            case "width":
        //                editor.ResizeMode = RTEResizeMode.ResizeWidth;
        //                break;
        //            case "height":
        //                editor.ResizeMode = RTEResizeMode.ResizeHeight;
        //                break;
        //            case "both":
        //                editor.ResizeMode = RTEResizeMode.ResizeBoth;
        //                break;
        //            case "no":
        //                editor.ResizeMode = RTEResizeMode.Disabled;
        //                break;
        //        }

        //        ViewBag.mode = m;
        //    });
        //    return new EmptyResult();
        //}

        //
        // GET: /Section/ModifierPage
        [ValidateInput(false)]
        public PartialViewResult ModifierPage(string nomDePage, string lien, string idLangue, int idPage)
        {
            try
            {
                var contenuModererViewModel = new ContenuModereViewModel();

                var langue = _service.GetLangue( Convert.ToInt32(idLangue));

                contenuModererViewModel = _service.GetContenuPublier(lien, nomDePage, langue.Culture) ?? new ContenuModereViewModel { Contenu = "" };

                var page = _service.GetPage(Convert.ToInt32(idPage));
                contenuModererViewModel.PageId = idPage;
                contenuModererViewModel.Page = page;

                var langues = _service.GetListeLangue();
                //ViewBag.Langues = new SelectList(langues, "Id", "Nom");

                contenuModererViewModel.ListeLangues = langues;
                contenuModererViewModel.LangueId = Convert.ToInt32(idLangue);

                //PrepairEditor("Contenu", delegate(Editor editor)
                //{
                //    editor.LoadFormData(contenuModererViewModel.Contenu);
                //});

                return PartialView("RichTextPartial", contenuModererViewModel);
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
                _service.MAJContenuModere(publishedContent);
                var msg = String.Format("<span class=\"succes\">{0}</span>", Traduction.MsgModification);
                return Json(msg, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Pages", "Section");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                var msg = String.Format("<span class=\"erreur\">{0}</span>", Traduction.MsgModificatioEchoue);
                return Json(msg, JsonRequestBehavior.AllowGet);
                //return base.Erreur();
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
                var success = _service.SauvegardeSection(model);
                return Json(JsonResponseFactory.SuccessResponse(model), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Error"), JsonRequestBehavior.DenyGet);
            }
        }


        public ActionResult AddPage(int id)
        {
            var model = new PageViewModel { SectionId = id };
            var listeModele = _service.GetListeModele();
            ViewBag.IdModele = listeModele;
            return View(model);
        }

        [HttpPost]
        public JsonResult AddPage(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var success = _service.SauvegardePage(model);
                return Json(JsonResponseFactory.SuccessResponse("Page ajoutee avec succes"), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Ajout de la page echoue"), JsonRequestBehavior.DenyGet);
            }
        }
        
        public ActionResult SupprimerSection(int id)
        {
            try
            {
                //get section by id
                var section = _service.GetSection(id);

                SupprimeSection(section);

                return RedirectToAction("Pages", "Section");
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }

        public void SupprimeSection(Section section)
        {
            try
            {
                //get children sections
                var children = _service.GetChildren(section.Id);

                var supprimeSection = false;
                if (children == null || children.Count == 0)
                {
                    //get liste pages
                    var listePages = section.Pages;

                    var supprimePage = false;
                    if (listePages != null && listePages.Count > 0)
                    {
                        foreach (var page in listePages)
                        {
                            //get liste contenu modere pour la page
                            var listeContenuModere = _service.GetListeContenuModere(page.Id);

                            var supprimeContenu = false;

                            //supprimer liste contenu modere
                            if (listeContenuModere != null && listeContenuModere.Count > 0)
                            {
                                foreach (var contenuModere in listeContenuModere)
                                {
                                    supprimeContenu = _service.SupprimerContenuModere(contenuModere);
                                }
                            }

                            //supprimer page
                            //if (supprimeContenu)
                            //{
                            supprimePage = _service.SupprimerPage(page);
                            //}
                        }
                    }

                    //delete section
                    //if (supprimePage)
                    //{
                    supprimeSection = _service.SupprimerSection(section);
                    //}
                }
                else
                {
                    foreach (var child in children)
                    {
                        //call function recursively
                        SupprimeSection(child);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult SupprimerPage(int pageId)
        {
            try
            {
                //get contenu moderer par pg id
                var listeContenuModere = _service.GetListeContenuModere(pageId);

                var supprime = false;

                //si liste n'est pas nulle, supprimer toutes les contenu moderes
                if (listeContenuModere != null)
                {
                    foreach (var contenuModere in listeContenuModere)
                    {
                       supprime =  _service.SupprimerContenuModere(contenuModere);
                    }
                }

                //supprimer page
                //if (supprime)
                //{
                    var page = _service.GetPage(pageId);
                    _service.SupprimerPage(page);
                //}

                return RedirectToAction("Pages", "Section");
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }

    }
}
