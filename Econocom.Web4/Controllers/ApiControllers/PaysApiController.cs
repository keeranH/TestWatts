using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;


namespace Econocom.Web4.Controllers.ApiControllers
{
    public class PaysApiController : Controller
    {
        //
        // GET: /Pays/

        public Pays getPaysById(int paysId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetPaysById(paysId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
