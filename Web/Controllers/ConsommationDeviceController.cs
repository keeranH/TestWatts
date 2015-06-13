using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Data;
using Web.ServiceReference1;
using Econocom.Model.Models.Benchmark;

namespace Web.Controllers
{
    public class ConsommationDeviceController : BaseController
    {
         private EconocomContext db = new EconocomContext();
        private Web.ServiceReference1.IEconocomService service;
        public ConsommationDeviceController()
        {
            service = new EconocomServiceClient();
            base.Service = service;
        }

        //
        // GET: /ConsommationDevice/

        public ActionResult Index()
        {
            try
            {
                var list = new List<ConsommationDeviceClient>();

                ConsommationDeviceClient cdc1 = new ConsommationDeviceClient();

                TYPEDEVICE typedev1 = new TYPEDEVICE();
                typedev1.IDTYPEDEVICE = 1;
                typedev1.LIBELLETYPEDEVICE = "Desktop";

                cdc1.TYPEDEVICE = typedev1;
                cdc1.CONSOMMATION = 97;

                ConsommationDeviceClient cdc2 = new ConsommationDeviceClient();

                TYPEDEVICE typedev2 = new TYPEDEVICE();
                typedev2.IDTYPEDEVICE = 2;
                typedev2.LIBELLETYPEDEVICE = "Station D'accueil";


                cdc2.TYPEDEVICE = typedev2;
                cdc2.CONSOMMATION = 8;


                ConsommationDeviceClient cdc3 = new ConsommationDeviceClient();

                TYPEDEVICE typedev3 = new TYPEDEVICE();
                typedev3.IDTYPEDEVICE = 3;
                typedev3.LIBELLETYPEDEVICE = "Scanner Personnel";


                cdc3.TYPEDEVICE = typedev3;

                cdc3.CONSOMMATION = 67;

                ConsommationDeviceClient cdc4 = new ConsommationDeviceClient();

                TYPEDEVICE typedev4 = new TYPEDEVICE();
                typedev4.IDTYPEDEVICE = 4;
                typedev4.LIBELLETYPEDEVICE = "Visiophone";


                cdc4.TYPEDEVICE = typedev4;


                cdc4.CONSOMMATION = 67;

                list.Add(cdc1);
                list.Add(cdc2);
                list.Add(cdc3);
                list.Add(cdc4);


                ViewBag.ConsommationDevice = list.ToList();

                return View();
            }
            catch (Exception e)
            {
                return base.Erreur();
            }



        }

    
        //
        // GET: /ConsommationDevice/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ConsommationDevice/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ConsommationDevice/Create

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
        // GET: /ConsommationDevice/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ConsommationDevice/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ConsommationDevice/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ConsommationDevice/Delete/5

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
