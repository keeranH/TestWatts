using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Resource;
using NLog;
using LoginModel = Econocom.Admin.Models.LoginModel;


namespace Econocom.Admin.Controllers
{
    public class AdminController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       private readonly ServiceApiController _service;

        public AdminController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /Admin/Admin/

        public ActionResult Index()
        {
            var loginModel = new LoginModel();           
            ViewBag.Erreur = TempData["Erreur"];
            
            /*
            try
            {
                var result = _service.GetSimulation(1);
            }
            catch (Exception e)
            {
            }*/
            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new Utilisateur();
                    user.MotDePasse = Security.HashHelper.HashPassWord(loginModel.Password);
                    user.Email = loginModel.UserName;
                    var utilisateurExistant = _service.GetUtilisateur(user);
                    if (utilisateurExistant != null)
                    {
                        FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
                        this.HttpContext.User = new GenericPrincipal(new GenericIdentity(loginModel.UserName), null);
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
                        else
                            return RedirectToAction("Souscription", "Moderation");
                    }
                    else
                    {
                        TempData.Add("Erreur", Traduction.ErreurUtilisateur);
                    }
                }
                else
                {
                    TempData.Add("Erreur", Traduction.ErreurLogin);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
