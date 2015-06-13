using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Rotativa;

namespace Econocom.Admin.Controllers
{
    public class HelpController : BaseController
    {

        private ServiceApiController service;

        public HelpController()
        {
            service = new ServiceApiController();
        }

        //
        // GET: /Help/

        public ActionResult Index()
        {
            try
            {
                //Récupérer le contenu modéré pour cette page
                base.InitierContentu("Help/HelpPartial", "HelpPartial", "fr-FR");
            }
            catch (Exception)
            {
                return base.Erreur();
            }
            return View();
        }

        public ActionResult HelpPartial()
        {
            try
            {
                base.InitierContentu("Help/HelpPartial", "HelpPartial", "fr-FR");
                return new PartialViewAsPdf();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult Pagination()
        {
            ViewBag.People = GetPeople();
            return View();
        }

        public List<Tarif> GetPeople()
        {
            List<Tarif> listPeople = new List<Tarif>();

            var tarif1 = new Tarif();
            tarif1.LibelleTarif = "Tarif1";
            tarif1.NbreMinDevice = 1;
            tarif1.NbreMaxDevice = 10;
            tarif1.DateDebut = DateTime.Now;
            tarif1.DateModification = DateTime.Now;
            tarif1.DateFin = DateTime.Now;
            tarif1.TarifAnnuel = 10;

            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);
            listPeople.Add(tarif1);

            /*
            listPeople.Add(new Tarif("Tarif", 1, 10, DateTime.Now, DateTime.Now, DateTime.Now, 10));
            listPeople.Add(new Tarif("Straight", "Dean", "email@address.com", 123456789, DateTime.Now.AddDays(-5), 35));
            listPeople.Add(new Tarif("Karsen", "Livia", "karsen@livia.com", 46874651, DateTime.Now.AddDays(-2), 31));
            listPeople.Add(new Tarif("Ringer", "Anne", "anne@ringer.org", null, DateTime.Now, null));
            listPeople.Add(new Tarif("O'Leary", "Michael", "23sssa@asssa.org", 32424344, DateTime.Now, 44));
            listPeople.Add(new Tarif("Gringlesby", "Anne", "email@yahoo.org", null, DateTime.Now.AddDays(-9), 18));
            listPeople.Add(new Tarif("Locksley", "Stearns", "my@email.org", 2135345, DateTime.Now, null));
            listPeople.Add(new Tarif("DeFrance", "Michel", "email@address.com", 235325352, DateTime.Now.AddDays(-18), null));
            listPeople.Add(new Tarif("White", "Johnson", null, null, DateTime.Now.AddDays(-22), 55));
            listPeople.Add(new Tarif("Panteley", "Sylvia", null, 23233223, DateTime.Now.AddDays(-1), 32));
            listPeople.Add(new Tarif("Blotchet-Halls", "Reginald", null, 323243423, DateTime.Now, 26));
            listPeople.Add(new Tarif("Merr", "South", "merr@hotmail.com", 3232442, DateTime.Now.AddDays(-5), 85));
            listPeople.Add(new Tarif("MacFeather", "Stearns", "mcstearns@live.com", null, DateTime.Now, null));*/

            return listPeople;
        }

    }
}
