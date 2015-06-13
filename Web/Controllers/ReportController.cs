using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel.Report;
using Econocom.Model.Interfaces;

namespace Web.Controllers
{
    public class ReportController : Controller
    {
       
        private IEconocomService service;
        public ReportController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        [Authorize]
        public ActionResult ReportDisplay(int id)
        {
            CLIENT client = service.GetClientByLoginName(User.Identity.Name);
            if (client != null)
            {
                dynamic reportData = service.GetReportClientData(client.IDCLIENT, "test");
                var reportClientData = new ViewModelReportClient();
                reportClientData.Total = (int)reportData.Total;
                reportClientData.ViewModelDeviceFamilies = reportData.Families;
                return View("ReportDisplay",reportClientData);
            }

            return null;

        }
        
        //
        // GET: /Report/

        public ActionResult Index()
        {
            string userName = User.Identity.Name;
            dynamic reportData = service.GetReportClientData(1, "test");
            var reportClientData = new ViewModelReportClient();
            reportClientData.Total =(int) reportData.Total;
            reportClientData.ViewModelDeviceFamilies = reportData.Families;
            return View(reportClientData);

        }




        [HttpPost]
        public ActionResult Edit(String btnstatus, dynamic model)
        {
            var families = service.GetReportClientData(1, "test");
            return PartialView("_Report", model);
        }

    }
}
