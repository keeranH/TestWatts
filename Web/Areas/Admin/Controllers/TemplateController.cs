using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.CMS;
using Model;
using System.Data;
using Model.Interfaces;

namespace Web.Controllers
{
    public class TemplateController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public TemplateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //
        // GET: /Template/

        public ViewResult Index()
        {
            var templateRepository = new Repository<Modele>();

            return View(templateRepository.All().AsEnumerable());
        }

        //
        // GET: /Template/Details/5

        public ViewResult Details(int id)
        {
            var templateRepository = new Repository<Modele>();

            var template = templateRepository.Find(id);
            return View(template);
        }

        //
        // GET: /Template/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Template/Create

        [HttpPost]
        public ActionResult Create(Modele template)
        {
            if (ModelState.IsValid)
            {
                var templateRepository = new Repository<Modele>(_unitOfWork);
                templateRepository.Create(template);
                _unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(template);
        }

        //
        // GET: /Template/Edit/5

        public ActionResult Edit(int id)
        {
            var templateRepository = new Repository<Modele>();
            var template = templateRepository.Find(id);            
            return View(template);
        }

        //
        // POST: /Template/Edit/5

        [HttpPost]
        public ActionResult Edit(Modele template)
        {
            if (ModelState.IsValid)
            {
                var templateRepository = new Repository<Modele>(_unitOfWork);
                templateRepository.Update(template);              
                _unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        //
        // GET: /Template/Delete/5

        public ActionResult Delete(int id)
        {
            var templateRepository = new Repository<Modele>(_unitOfWork);
            var template = templateRepository.Find(id);
            return View(template);
        }

        //
        // POST: /Template/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var templateRepository = new Repository<Modele>(_unitOfWork);
            var template = templateRepository.Find(id);
            templateRepository.Delete(template);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
