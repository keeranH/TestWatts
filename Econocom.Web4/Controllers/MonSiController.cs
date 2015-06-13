using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econocom.Web4.Controllers
{




    public class MonSiController : Controller
    {
        //
        // GET: /MonSi/

        public ActionResult Index()
        {

            return View();
        }

        //
        // GET: /MonSi/Details/5

        public ActionResult Details()
        {

            dynamic dynamicContext = new ExpandoObject();

            dynamicContext.familyLogo = "TopImage.jpg";
            dynamicContext.topImageLabel = "Collaborateur";
            dynamicContext.imageUrl = "MainImage.jpg";
            dynamicContext.kwhValue = "68781";
            dynamicContext.percentageValue = "42";

            //IDictionary<string, object> dict = (IDictionary<string, object>)dynamicContext;
            //dict.Add("Test", dynamicContext);

            ViewBag.test = dynamicContext;
            return View();
        }


    }
}
