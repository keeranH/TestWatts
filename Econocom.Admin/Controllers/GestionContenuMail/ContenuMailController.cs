using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;
using Omu.ValueInjecter;
//using RTE;

namespace Econocom.Admin.Controllers.GestionContenuMail
{
    public class ContenuMailController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public ContenuMailController()
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
        // GET: /ContenuMail/
        public ActionResult Index()
        {
            try
            {
               var listeContenuMail = _service.GetListeTypeMail();
               return View(listeContenuMail);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);    
                throw;
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ActualiserContenu(ContenuMailViewModel contenuMailViewModel)
        {
            try
            {
                var contenuMail = new ContenuMail();
                contenuMail.InjectFrom(contenuMailViewModel);
                contenuMail.Id = contenuMailViewModel.Id.Value;
                _service.SetContenuMail(contenuMail);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }                     
        }

        public ActionResult ModifierContenu(int idTypeMail, int idLangue)
        {
            var contenuMailViewModel = new ContenuMailViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var contenuMail = _service.GetContenuMail(idTypeMail,idLangue);
                if (contenuMail == null)
                {
                    contenuMail = new ContenuMail();
                    contenuMail.TypeMailId = idTypeMail;
                }
                contenuMailViewModel.InjectFrom(contenuMail);
                contenuMailViewModel.Id = contenuMail.Id;
                contenuMailViewModel.LangueId = idLangue;
                contenuMailViewModel.ListeLangues = langues;

                //PrepairEditor("Contenu", delegate(Editor editor)
                //{
                //    editor.LoadFormData(contenuMailViewModel.Contenu);
                //});
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);    
                throw;
            }
            return PartialView("ContenuPartial", contenuMailViewModel);
        }
    }
}

