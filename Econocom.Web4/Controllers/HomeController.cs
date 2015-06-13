using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Econocom.Data;
using Econocom.Helper.Authorisation;
using Econocom.Helper.File;
using Econocom.Model;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;
using Econocom.Resource;
using Econocom.Web4.Controllers.ApiControllers;
using Econocom.Web4.Models;
using NLog;
using Rotativa;

namespace Econocom.Web4.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public HomeController()
        {
            this.InitLanguageDropDown();
            _service = new ServiceApiController();
        }
        public ActionResult TestViewWithModel(string id)
        {
            
            return new ViewAsPdf("TestViewWithModel");
        }
        public ActionResult Index()
        {
            try
            {
                ViewBag.ErrorLogin = Session["ErrorLogin"];
                Session.Remove("ErrorLogin");

              
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                base.InitierContenu("Home/Index", "Index", currentCulture);
                
                ViewBag.MsgPersoPasPret = Session["MsgPersoPasPret"];

                try
                {
                    var folderImages = ConfigurationManager.AppSettings["HomeImages"];
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
            catch (Exception e)
            {
                LogguerErreur(e);                
            }
            return View();
        }
        
       

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Tooltip()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                var contact = new Contact();
                contact.MotPasse = loginModel.Password;
                contact.Email = loginModel.UserName;
                var contactExistant = _service.GetContact(contact);

                if (contactExistant != null)
                {
                    Session.Add("idContact", contactExistant.Id);
                    FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
                    var urlReferer = Request.UrlReferrer;
                    if (urlReferer != null)
                    {
                        var queryStr = urlReferer.Query;
                        if (queryStr.Length > 0)
                        {
                            var returnUrl = queryStr.Replace("?ReturnUrl=", "");
                            if (returnUrl.Length > 0)
                            {
                                var decodedUrl = Server.UrlDecode(returnUrl);
                                return Redirect(decodedUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            Session.Remove("MsgPersoPasPret");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.loginModel = new LoginViewModel();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Edit(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginContact = new Contact {Email = model.Username, MotPasse = model.Password};
                    var contact = _service.GetContact(loginContact);
                    if (contact != null && contact.Client.Statut == (int) ClientStatut.Valide)
                    {
                        Session.Add("idContact", contact.Id);
                        FormsAuthentication.SetAuthCookie(contact.Email, model.RememberMe);
                        return Json(JsonResponseFactory.SuccessResponse(contact), JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        var result = Json(JsonResponseFactory.ErrorResponse("Username/Password error"),
                                          JsonRequestBehavior.DenyGet);
                        return result;
                    }
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse("Please review your form"),
                                JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return Json(JsonResponseFactory.ErrorResponse("Success"),JsonRequestBehavior.DenyGet);
        }

        public ActionResult GestionMotDePasse()
        {
            var gestionMotDePasseViewModel = new GestionMotDePasseViewModel();

            try
            {
                string url = "";
                if (Request.Url != null)
                {
                    url = Request.Url.AbsoluteUri;
                }

                string email = url.Split('?')[1].Split('&')[0].Split('=')[1];
                string vcode = url.Split('?')[1].Split('&')[1].Split('=')[1];               

                var questions = _service.GetListeQuestion();                
                gestionMotDePasseViewModel.Questions = questions;
                gestionMotDePasseViewModel.CodeVerification = vcode;
                gestionMotDePasseViewModel.Email = email;

                var ContactApi = new ContactApiController();
                var currentContact = ContactApi.GetContactParVerificationCode(gestionMotDePasseViewModel.CodeVerification);
                if (currentContact == null)
                {
                    ViewBag.CompteActif = true;
                    return View(gestionMotDePasseViewModel);
                }
                else
                {
                    ViewBag.CompteActif = false;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return View(gestionMotDePasseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult GestionMotDePasse(GestionMotDePasseViewModel gestionMotDePasseViewModel)
        {
            ContactApiController ContactApi= new ContactApiController();
            ClientApiController ClientApi= new ClientApiController();

            var contact = new Contact();
            try
            {
                var questions = _service.GetListeQuestion();               
                gestionMotDePasseViewModel.Questions = questions;

                var currentContact = ContactApi.GetContactParVerificationCode(gestionMotDePasseViewModel.CodeVerification);

                if (currentContact == null)
                {
                    ViewBag.CompteActif = true;
                    return View(gestionMotDePasseViewModel);
                }
                else
                {
                    ViewBag.CompteActif = false;
                }

                if (ModelState.IsValid)
                {
                    if (gestionMotDePasseViewModel.MotDePasse.Equals(gestionMotDePasseViewModel.MotDePasseConfirme))
                    {
                        var check = gestionMotDePasseViewModel.Email.Contains("%40");

                        if (check)
                        {
                            gestionMotDePasseViewModel.Email =  gestionMotDePasseViewModel.Email.Replace("%40", "@");
                        }

                        if (currentContact.Email == gestionMotDePasseViewModel.Email)
                        {
                                contact = currentContact;
                        }

                        var mdp = gestionMotDePasseViewModel.MotDePasse;
                        mdp = Security.HashHelper.HashPassWord(mdp);

                        contact.MotPasse = mdp;
                        contact.Valider = true;
                        contact.CodeVerification = null;                                            
                        var reponse = new Reponse();
                        var reponses = new List<Reponse>();
                        if (contact.Reponses != null && contact.Reponses.Count > 0)
                            reponse = contact.Reponses.ToArray()[0];
                       
                        reponse.ContactId = contact.Id;
                        reponse.QuestionId = Convert.ToInt32(gestionMotDePasseViewModel.QuestionId);
                        reponse.Valeur = gestionMotDePasseViewModel.ReponseSaisie;
                        
                        //MAJ contact
                        //set date premiere connexion
                        if (contact.DateDebut == null || contact.DateDebut == new DateTime())
                            contact.DateDebut = DateTime.Now;
                        else
                            contact.DateModification = DateTime.Now;

                        contact = _service.UpdateContact(contact);

                        //Sauvegarder reponse
                        if(reponse.Id>0)
                            reponse = _service.UpdateReponse(reponse);
                        else
                            reponse = _service.SetReponse(reponse);
                    }
                    else
                    {
                        ViewBag.MsgErreur = Resource.Traduction.MotDePassePasCorrespondre;
                        return View(gestionMotDePasseViewModel);
                    }
                }
                else
                {
                    return View(gestionMotDePasseViewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                base.Erreur();
            }

            var client=  ClientApi.GetClientById(contact.ClientId);
            var detailsClient = ClientApi.GetDetailsClientById((int) client.DetailsClientId);
            Session.Add("idContact", contact.Id);

            var authorisationHelper = new AuthorisationHelper();
            //check type contact
            if (authorisationHelper.EstAdministrateur(contact) || authorisationHelper.EstSouscripteurAdmin(contact))
            {
                Session.Add("userType", "Payant");
                if (client.SecteurActiviteId == null)
                {
                    //redirect pg souscription/simulateur
                    Session.Add("subscribeClientId", client.Id);
                    Session.Add("detailsClientId", detailsClient.Id);
                    FormsAuthentication.SetAuthCookie(contact.Email, false);
                    return RedirectToRoute(new {controller = "Souscription", action = "Simulateur"});
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(contact.Email, false);
                    Session.Add("idClient", client.Id);
                    return RedirectToRoute(new { controller = "Consommation", action = "Index", id = client.Id });
                }
            }
            else
            {
                if (client.SecteurActiviteId != null && detailsClient.Effectif != 0 && detailsClient.TauxEquipement != 0 && detailsClient.NombreSite != 0)
                {
                    //redirect pg affichage famille
                    Session.Add("userType", "Payant");
                    FormsAuthentication.SetAuthCookie(contact.Email, false);
                    Session.Add("idClient", client.Id);
                    return RedirectToRoute(new { controller = "Consommation", action = "Index", id = client.Id });
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(contact.Email, false);
                    Session.Add("MsgPersoPasPret", Traduction.MsgPersoPasPret);
                    return RedirectToAction("Index");
                }
            }
            
        }

    }
}

