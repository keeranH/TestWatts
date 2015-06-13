namespace Econocom.Web4.Controllers
{
    public static class DemoExtensions
    {
        public static bool Containz(this string o, string oo)
        {
            return o.ToLower().Contains(oo.ToLower());
        }

    }
}