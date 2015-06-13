using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Econocom.Helper.Authorisation;
using Econocom.Model;
using Econocom.Model.Enum;
using Econocom.Model.Interfaces;
using Econocom.Model.ViewModel;
using Econocom.Resource;
using Econocom.Web4.Controllers.ApiControllers;
using Infrastructure.Builder;
using Infrastructure.DTO;
using Omu.Awesome.Mvc;
using Rotativa;
using NLog;

namespace Econocom.Web4.Controllers
{
   
    public class LoginController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoginController()
        {
            this.InitLanguageDropDown();
        }
        
        
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [OutputCache(Duration = 0)] 
        public ActionResult Login(FormCollection formCollection)
        {
            try
            {
                Session.Remove("MsgPersoPasPret");

                var email = formCollection["username"];
                var password = formCollection["password"];

                var apiLogin = new LoginApiController();
                var loginUser = apiLogin.GetContact(email, password);

                if (!ModelState.IsValid)
                    return Json(new {error = "User does not exist"}, JsonRequestBehavior.AllowGet);

                if (loginUser != null)
                {                   
                    var ret = loginUser.Email;
                    FormsAuthentication.SetAuthCookie(ret,false);
                    Session.Add("idContact", loginUser.Id);

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

                    var autorisationHelper = new AuthorisationHelper();
                    //get client par contact.clientId
                    var client = apiLogin.GetClientParId(loginUser.ClientId);
                    var detailsClient = client.DetailsClient;
                    //check type contact
                    if (autorisationHelper.EstAdministrateur(loginUser) || autorisationHelper.EstSouscripteurAdmin(loginUser))
                    {
                        if ((client.SecteurActiviteId != null && detailsClient.Effectif != 0 &&
                            detailsClient.TauxEquipement != 0 && detailsClient.NombreSite != 0) || 
                            (client.SecteurActiviteId != null && client.TypeClient == (int)TypeClient.ECSchange))
                        {
                            //redirect pg affichage famille
                            Session.Add("userType", "Payant");
                            Session.Add("idClient", client.Id);
                            return RedirectToRoute(new { controller = "Consommation", action = "Index", id = client.Id });
                        }
                        else
                        {
                            //redirect pg souscription/simulateur
                            Session.Add("subscribeClientId", client.Id);
                            Session.Add("detailsClientId", detailsClient.Id);
                            return RedirectToRoute(new { controller = "Souscription", action = "Simulateur" });
                        }
                    }
                    else
                    {
                        if (client.SecteurActiviteId != null && detailsClient.Effectif != 0 &&
                            detailsClient.TauxEquipement != 0 && detailsClient.NombreSite != 0)
                        {
                            //redirect pg affichage famille
                            Session.Add("userType", "Payant");
                            Session.Add("idClient", client.Id);
                            return RedirectToRoute(new {controller = "Consommation", action = "Index", id = client.Id});
                        }
                        else
                        {
                            Session.Add("MsgPersoPasPret", Traduction.MsgPersoPasPret);
                            return RedirectToRoute(new { controller = "Home", action = "Index"});
                        }
                    }
                }
                else
                {
                    Session.Add("ErrorLogin", Resource.Traduction.MotDePasseIncorrect);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return RedirectToAction("Index", "Home");
            }
        }


       
        [OutputCache(Duration = 0)] 
        public ActionResult Logoff()
        {            
            //Session.Remove("MsgPersoPasPret");
            //FormsAuthentication.SignOut();
            //Session.Abandon();

            FormsAuthentication.SignOut();
            Session.RemoveAll();
            Session.Clear();  // This may not be needed -- but can't hurt
            Session.Abandon();
            
            // Clear authentication cookie
            HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            rFormsCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rFormsCookie);

            // Clear session cookie 
            HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            rSessionCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rSessionCookie);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Registration()
        {
            try
            {

                var apiLogin = new LoginApiController();
                var pays = apiLogin.GetPays();
                var model = new RegistrationClientModel { Type = "Full", Contacts = new List<ContactViewModel>() };
                model.Adresse.ListPays = pays;
                var contact = new ContactViewModel();
                model.Contacts.Add(contact);
                return View(model);
            }
            catch (Exception)
            {
                //return RedirectToAction("NotFound", "Error"); //change to error page
                return RedirectToAction("Index", "Home");

            }
        }
              

        public ActionResult ContactRow()
        {
            return PartialView("ContactEditor");
        }

        public ActionResult ApresRegistration()
        {
            return View();
        }

        public ActionResult AccountVerification(Guid id)
        {   
            var apiContact = new ContactApiController();
            var apiQuestion = new QuestionApiController();
            Guid newId = Guid.NewGuid();
            if (id != null)
                newId = (Guid)id;
            string verificationCode = newId.ToString();
            var contact = apiContact.GetContactParVerificationCode(verificationCode);
            if (contact != null)
            {
                var newContact = new RegisterViewModel { Id = newId, ClientId = contact.Client.Id };

                var questions = apiQuestion.GetQuestions();
                foreach (var question in questions)
                {
                    question.LibelleQuestion = Resource.Traduction.ResourceManager.GetString("Question" + question.Id) ?? question.LibelleQuestion;
                }
                ViewBag.Questions = questions.AsQueryable();
                ViewBag.QuestionId = new SelectList(questions.AsEnumerable(), "Id", "LibelleQuestion");

                return View(newContact);
            }
            return base.Erreur(); //change to error page
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AccountVerification(RegisterViewModel enregistrementModel)
        {
            var apiQuestion = new QuestionApiController();
            var apiContact = new ContactApiController();
            var questions = apiQuestion.GetQuestions();           
            ViewBag.QuestionId = new SelectList(questions.AsEnumerable(), "Id", "LibelleQuestion");
            if (ModelState.IsValid)
            {
                try
                {
                    var contact = apiContact.VerificationCompte(enregistrementModel);
                    if (contact != null)
                    {
                        Session.Add("idContact", contact.Id);
                        FormsAuthentication.SetAuthCookie(contact.Email, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View(enregistrementModel);
                    }
                }
                catch (Exception e)
                {
                    return View(enregistrementModel);
                }
            }
            return View(enregistrementModel);
        }

        public ActionResult ModifierMotDePasse()
        {
            var model = new LoginViewModel();
            return View("ModifierMotDePasse", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ModifierMotDePasse(LoginViewModel model)
        {
            if (model.Username == "neo")
            {
                ModelState.AddModelError("", "Le nom d'utilisateur ou mot de passe fourni est incorrect.");

                return View("ModifierMotDePasse", model);
            }
            else
            {               
                UrlHelper u = new UrlHelper(this.ControllerContext.RequestContext);
                string url = u.Action("Index", "Developpement", null);
                var json = Json(JsonResponseFactory.SuccessResponse(url), JsonRequestBehavior.DenyGet);
                return json;
            }
        }   
    }
}
