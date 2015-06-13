using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Model.Interfaces;
using Web;

namespace Web.Areas.Admin.Controllers
{ 
    public class AgeController : Controller
    {
        //readonly EconocomUnitOfWork _unitOfWork = new EconocomUnitOfWork();

       // private IUnitOfWork _unitOfWork = new EconocomContext();

        //public AgeController(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;           
        //}

        private IEconocomService service;
        public AgeController(IEconocomService econocomService)
        {
            this.service = econocomService;
        }

        //
        // GET: /Admin/Default1/

        public ViewResult Index()
        {            
            //var contrat = new Contract();
            //contrat.Actif = true;
            //contrat.DateAnnulation= DateTime.Now;
            //contrat.DateCreation = DateTime.Now;
            //contrat.DateDebut = DateTime.Now;
            //contrat.DateFin = DateTime.Now;            

            //var entreprise = new Entreprise();
            //entreprise.Nom = "ccc";
            //entreprise.Contrats = new Collection<Contract>();
            //entreprise.Contrats.Add(contrat);
            //_unitOfWork.Entreprise.Create(entreprise);
            // _unitOfWork.SaveChanges();

            //var entreprise = _unitOfWork.Entreprise.Find(1);
            //if (entreprise.Contrats==null)
            //    entreprise.Contrats = new Collection<Contract>();
            //entreprise.Contrats.Add(contrat);

            //_unitOfWork.Entreprise.Update(entreprise);
           
           // IEnumerable<Age> ages = _unitOfWork.Age.All();                       


            //var contrat = new Contract();
            //contrat.Actif = true;
            //contrat.DateAnnulation= DateTime.Now;
            //contrat.DateCreation = DateTime.Now;
            //contrat.DateDebut = DateTime.Now;
            //contrat.DateFin = DateTime.Now;

            //var entreprise = new Entreprise();
            //entreprise.Nom = "c67cc";
            //entreprise.Contrats = new Collection<Contract>();
            //entreprise.Contrats.Add(contrat);

            //IRepository<Entreprise> entrepriseRepository = new Repository<Entreprise>(_unitOfWork);
            //entrepriseRepository.Create(entreprise);
            //_unitOfWork.SaveChanges();

            //var graph = new TestGraph();
            //graph.TestDynamic();

            IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
            IEnumerable<AGEDEVICE> ages = ageRepository.All();       

            return View(ages);
        }

        //
        // GET: /Admin/Default1/Details/5

        public ViewResult Details(int id)
        {
            //var age = _unitOfWork.Age.Find(id);
            IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
            var age = ageRepository.Find(id);
            return View(age);
        }

        //
        // GET: /Admin/Default1/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Default1/Create

        [HttpPost]
        public ActionResult Create(AGEDEVICE deviceAge)
        {
            if (ModelState.IsValid)
            {               
                //_unitOfWork.Age.Create(age);  
                IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
                ageRepository.Create(deviceAge);
                return RedirectToAction("Index");  
            }

            return View(deviceAge);
        }
        
        //
        // GET: /Admin/Default1/Edit/5
 
        public ActionResult Edit(int id)
        {            
            //var age = _unitOfWork.Age.Find(id);           
            IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
            var age = ageRepository.Find(id);
            return View(age);
        }

        //
        // POST: /Admin/Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(AGEDEVICE deviceAge)
        {
            if (ModelState.IsValid)
            {
               //_unitOfWork.Age.Update(age);
               // _unitOfWork.SaveChanges();
                IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
                ageRepository.Update(deviceAge);
                return RedirectToAction("Index");
            }
            return View(deviceAge);
        }

        //
        // GET: /Admin/Default1/Delete/5
 
        public ActionResult Delete(int id)
        {
            //var age = _unitOfWork.Age.Find(id);  
            //_unitOfWork.Age.Delete(age);
            //_unitOfWork.SaveChanges();
            IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
            var age = ageRepository.Find(id);
            return View(age);
        }

        //
        // POST: /Admin/Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            //Age age = _unitOfWork.Age.Find(id);
            //_unitOfWork.Age.Delete(age);
            //_unitOfWork.SaveChanges();
            IRepository<AGEDEVICE> ageRepository = new Repository<AGEDEVICE>();
            var age = ageRepository.Find(id);
            return RedirectToAction("Index");
        }
    }
}