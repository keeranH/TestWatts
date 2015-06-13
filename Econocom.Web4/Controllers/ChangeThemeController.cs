using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econocom.Web4.Controllers
{
    public class ChangeThemeController : Controller
    {
        public ActionResult Index()
        {
            var theme = "ui-lightness";
            if (Request.Cookies["jqtheme"] != null)
                theme = Request.Cookies["jqtheme"].Value;

            var themes = new[] { "black-tie", "blitzer", "cupertino", "dark-hive", "dot-luv", "eggplant", "excite-bike", "flick", "hot-sneaks", "humanity", "le-frog", "mint-choc", "overcast", "pepper-grinder", "redmond", "smoothness", "south-street", "start", "sunny", "swanky-purse", "trontastic", "ui-darkness", "ui-lightness", "vader" };

            var items = themes.Select(o => new SelectListItem { Text = o, Value = o, Selected = o == theme });

            return View(items);
        }

        [HttpPost]
        public ActionResult Change(string[] themes)
        {
            var theme = themes[0];
            Response.Cookies.Add(new HttpCookie("jqtheme", theme) { Expires = DateTime.Now.AddYears(1) });
            return new EmptyResult();
        }

        public ActionResult CurrentTheme()
        {
            var theme = "ui-lightness";
            if (Request.Cookies["jqtheme"] != null)
                theme = Request.Cookies["jqtheme"].Value;

            return Content(theme);
        }
    }
}
