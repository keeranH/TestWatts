using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Econocom.Helper.Session;

namespace Web.Controllers
{
    public class LocalisationController : Controller
    {
        //
        // GET: /Localisation/

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult ChangeCulture(string language)
        {
            if (language.Equals("en"))
            {
                return Redirect("/en/Home/Welcome");
            }
            if (language.Equals("fr"))
            {
                return Redirect("/fr/Demarrer/Accueil");
            }
            if (language.Equals("nl"))
            {
                return Redirect("/nl/Thuis/Welkom");
            }
            return Redirect("/en/Home/Welcome");
          
        }


        public ActionResult ChangeCultureFromSession()
        {
            var culture = SessionHelper.Culture;
            if (culture.TwoLetterISOLanguageName.Equals("en"))
            {
                return Redirect("/en/Home/Welcome");
            }
            if (culture.TwoLetterISOLanguageName.Equals("fr"))
            {
                return Redirect("/fr/Demarrer/Accueil");
            }
            if (culture.TwoLetterISOLanguageName.Equals("nl"))
            {
                return Redirect("/nl/Thuis/Welkom");
            }
            return Redirect("/Home/Welcome");

        }
        
    }
}
