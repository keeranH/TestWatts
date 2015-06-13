using System.Web.Mvc;

namespace Econocom.Web4.Controllers
{
    public class NavController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Utilisateur()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}