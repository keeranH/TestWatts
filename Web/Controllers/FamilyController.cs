using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Interfaces;
using Econocom.Model.ViewModel;


namespace Web.Controllers
{
    public class FamilyController : Controller
    {
        private IEconocomService service;
        public FamilyController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        private IEnumerable<DeviceFamilyViewModel> GetFamilies()
        {
            var usage = new UsageViewModel();
            usage.NumberOfDaysInYear = 365;
            usage.NumberOfWorkingDays = 240;
            usage.NumberOfIntensiveWorkingHours = 3.5f;
            usage.NumberOfNonIntensiveWorkingHours = 4;

            var deviceAge = new DeviceAgeViewModel();
            deviceAge.Id = 100;
            deviceAge.Label = "<1";
            deviceAge.Ratio = 0.33f;
            deviceAge.Value = 33;

            var deviceAge2 = new DeviceAgeViewModel();
            deviceAge2.Id = 100;
            deviceAge2.Label = "1-3";
            deviceAge2.Ratio = 0.33f;
            deviceAge2.Value = 33;

            var deviceAges = new List<DeviceAgeViewModel>();
            deviceAges.Add(deviceAge);
            deviceAges.Add(deviceAge2);


            var usage1 = new UsageViewModel();
            usage1.NumberOfDaysInYear = 365;
            usage1.NumberOfWorkingDays = 240;
            usage1.NumberOfIntensiveWorkingHours = 3.5f;
            usage1.NumberOfNonIntensiveWorkingHours = 4;

            var deviceClass = new DeviceClassViewModel();
            deviceClass.Id = 2;
            deviceClass.Label = "class 1";
            deviceClass.Ratio = 0.56f;
            deviceClass.Value = 12;
            deviceClass.ViewModelDeviceAges = deviceAges;
            deviceClass.ViewModelUsage = usage1;

            var deviceClass2 = new DeviceClassViewModel();
            deviceClass2.Id = 2;
            deviceClass2.Label = "class 2";
            deviceClass2.Ratio = 0.56f;
            deviceClass2.Value = 12;
            deviceClass2.ViewModelDeviceAges = deviceAges;
            deviceClass2.ViewModelUsage = usage1;

            var deviceClasses = new List<DeviceClassViewModel>();
            deviceClasses.Add(deviceClass);
            deviceClasses.Add(deviceClass2);

            var usage2 = new UsageViewModel();
            usage2.NumberOfDaysInYear = 365;
            usage2.NumberOfWorkingDays = 240;
            usage2.NumberOfIntensiveWorkingHours = 3.5f;
            usage2.NumberOfNonIntensiveWorkingHours = 4;

            var deviceType = new DeviceTypeViewModel();
            deviceType.Id = 2;
            deviceType.Label = "type 1";
            deviceType.Ratio = 0.56f;
            deviceType.Value = 12;
            deviceType.ViewModelDeviceClasses = deviceClasses;
            deviceType.ViewModelUsage = usage2;

            var deviceType2 = new DeviceTypeViewModel();
            deviceType2.Id = 2;
            deviceType2.Label = "type 2";
            deviceType2.Ratio = 0.56f;
            deviceType2.Value = 12;
            deviceType2.ViewModelDeviceClasses = deviceClasses;
            deviceType2.ViewModelUsage = usage2;

            var deviceTypes = new List<DeviceTypeViewModel>();
            deviceTypes.Add(deviceType);
            deviceTypes.Add(deviceType2);

            var deviceCategory = new DeviceCategoryViewModel();
            deviceCategory.Id = 2;
            deviceCategory.Label = "category 1";
            deviceCategory.ViewModelDeviceTypes = deviceTypes;

            var deviceCategory2 = new DeviceCategoryViewModel();
            deviceCategory2.Id = 2;
            deviceCategory2.Label = "category 2";
            deviceCategory2.ViewModelDeviceTypes = deviceTypes;

            var deviceCategories = new List<DeviceCategoryViewModel>();
            deviceCategories.Add(deviceCategory);
            deviceCategories.Add(deviceCategory2);

            var family = new DeviceFamilyViewModel();
            family.Id = 1;
            family.Label = "family 1";
            family.ViewModelDeviceCategories = deviceCategories;

            var family2 = new DeviceFamilyViewModel();
            family2.Id = 1;
            family2.Label = "family 2";
            family2.ViewModelDeviceCategories = deviceCategories;

            var families = new List<DeviceFamilyViewModel>();
            families.Add(family);
            families.Add(family2);

            return families;
        }

        /*
        [HttpPost]
        public ActionResult UpdateContent(List<ViewModelDeviceFamily> deviceFamilies)
        {
            return RedirectToAction("About", "Home");
        }
        */

        [HttpPost]
        public ActionResult UpdateContent(ClientDeviceTypeViewModel clientBenchmarkViewModel)
        {
            return RedirectToAction("About", "Home");
        }

        //
        // GET: /Family/

        public ActionResult Index()
        {
            var families = GetFamilies();

            var clientBenchmarkViewModel = new ClientDeviceTypeViewModel();
            clientBenchmarkViewModel.ViewModelDeviceFamilies = families;

            return View(clientBenchmarkViewModel);
        }

        //
        // GET: /Family/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Family/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Family/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Family/Edit/5
 
        public ActionResult Edit(int id)
        {
            var clientBenchmarkViewModel = service.GetClientBenchmark(id);
            return View(clientBenchmarkViewModel);
        }

        //
        // POST: /Family/Edit/5

        [HttpPost]
        public ActionResult Edit(ClientDeviceTypeViewModel clientBenchmarkViewModel)
        {
            try
            {
                // TODO: Add update logic here
                var clientBenchmark = service.SaveClientBenchmarkDetails(clientBenchmarkViewModel);
                return View("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Family/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Family/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
