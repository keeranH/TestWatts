using System.Linq;
using System.Web.Mvc;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.CMS;
using Model.Interfaces;

namespace Web.Areas.Admin.Controllers
{ 
    public class PageController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public PageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //
        // GET: /Page/

        public ViewResult Index()
        {
            var pageRepository = new Repository<Page>();
            var pages = pageRepository.All();
            return View(pages.ToList());
        }

        //
        // GET: /Page/Details/5

        public ViewResult Details(int id)
        {
            var pageRepository = new Repository<Page>();
            var page = pageRepository.Find(id);
            return View(page);
        }

        //
        // GET: /Page/Create

        public ActionResult Create()
        {
            var sectionRepository = new Repository<Section>();
            var templateRepository = new Repository<Modele>();

            ViewBag.section_id = new SelectList(sectionRepository.All(), "id", "name");
            ViewBag.template_id = new SelectList(templateRepository.All(), "id", "name");
            return View();
        } 

        //
        // POST: /Page/Create

        [HttpPost]
        public ActionResult Create(Page page)
        {
            if (ModelState.IsValid)
            {
                var pageRepository = new Repository<Page>(_unitOfWork);
                pageRepository.Create(page);
                _unitOfWork.SaveChanges();
                return RedirectToAction("Index");  
            }

            var sectionRepository = new Repository<Section>();
            var templateRepository = new Repository<Modele>();

            ViewBag.section_id = new SelectList(sectionRepository.All(), "id", "name", page.Section);
            ViewBag.template_id = new SelectList(templateRepository.All(), "id", "name", page.ModeleModere);
            return View(page);
        }
        
        //
        // GET: /Page/Edit/5
 
        public ActionResult Edit(int id)
        {
            var pageRepository = new Repository<Page>(_unitOfWork);
            var page = pageRepository.Find(id);
            var sectionRepository = new Repository<Section>();
            var templateRepository = new Repository<Modele>();
            ViewBag.section_id = new SelectList(sectionRepository.All(), "id", "name", page.Section);
            ViewBag.template_id = new SelectList(templateRepository.All(), "id", "name", page.ModeleModere);
            return View(page);
        }

        //
        // POST: /Page/Edit/5

        [HttpPost]
        public ActionResult Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                var pageRepository = new Repository<Page>(_unitOfWork);
                pageRepository.Update(page);
                _unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }
            var sectionRepository = new Repository<Section>();
            var templateRepository = new Repository<Modele>();
            ViewBag.section_id = new SelectList(sectionRepository.All(), "id", "name", page.Section);
            ViewBag.template_id = new SelectList(templateRepository.All(), "id", "name", page.ModeleModere);
            return View(page);
        }

        //
        // GET: /Page/Delete/5
 
        public ActionResult Delete(int id)
        {
            var pageRepository = new Repository<Page>();
            Page page = pageRepository.Find(id);
            return View(page);
        }

        //
        // POST: /Page/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var pageRepository = new Repository<Page>(_unitOfWork);
            var page = pageRepository.Find(id);
            pageRepository.Delete(page);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
         
    }
}