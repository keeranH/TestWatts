using System.Web.Mvc;

namespace Econocom.Admin.Controllers
{
    [Authorize]
    public class NavController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }       

        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Langue()
        {
            return View();
        }

        public ActionResult Traduction()
        {
            return View();
        }
    }
}