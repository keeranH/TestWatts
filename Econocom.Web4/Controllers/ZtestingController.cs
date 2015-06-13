using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.ViewModel; 

namespace Econocom.Web4.Controllers
{
    public class ZtestingController : Controller
    {
        //
        // GET: /Ztesting/

        public ActionResult Index()
        {
            return View();
        }



        [ValidateInput(false)]
        public PartialViewResult PartialViewOne()
        {
            ContactViewModel cvm= new ContactViewModel();
            

            return PartialView("PartialViewOne", cvm);

        }

        [ValidateInput(false)]
        public PartialViewResult PartialViewTwo()
        {

            ContactViewModel cvm = new ContactViewModel();

            return PartialView("PartialViewTwo", cvm);

        }

    }
}
