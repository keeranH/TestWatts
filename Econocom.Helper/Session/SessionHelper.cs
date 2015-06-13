using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Econocom.Model.Models.CMS;

namespace Econocom.Helper.Session
{
    public class SessionHelper
    {
        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        /// <summary>
        /// Gets the session culture.
        /// This allow us to choose the right language for the pages
        /// </summary>
        public static CultureInfo Culture
        {
            get { return (CultureInfo) Session["Culture"]; }
            set { Session["Culture"] = value; }
        }

    }
}
