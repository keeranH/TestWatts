using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Web4.Controllers
{
    public class TarifController : Controller
    {
        //
        // GET: /Tarif/

        public ActionResult Index()
        {
            var tarif1 = new Tarif { LibelleTarif = "de 50 a 499 collaborateurs", TarifAnnuel = 1200 };
            var tarif2 = new Tarif { LibelleTarif = "de 500 a 999 collaborateurs", TarifAnnuel = 2400 };
            var tarif3 = new Tarif { LibelleTarif = "de 1000 a 4 999 collaborateurs", TarifAnnuel = 4200 };
            var tarif4 = new Tarif { LibelleTarif = "de 5000 a 9 999 collaborateurs", TarifAnnuel = 5880 };
            var tarif5 = new Tarif { LibelleTarif = "de 10000 a 49 999 collaborateurs", TarifAnnuel = 10200 };
            var tarif6 = new Tarif { LibelleTarif = "Au-dela de 500 000 collaborateurs", TarifAnnuel = 15000 };

            var tarifListe = new List<Tarif>();
            tarifListe.Add(tarif1);
            tarifListe.Add(tarif2);
            tarifListe.Add(tarif3);
            tarifListe.Add(tarif4);
            tarifListe.Add(tarif5);
            tarifListe.Add(tarif6);

            ViewBag.IdFormulaire = 1;
            ViewBag.Nom = "TestNom";
            ViewBag.Prenom = "TestPrenom";
            ViewBag.RaisonSociale = "TestRaisonSociale";
            ViewBag.Date = "01/01/13";

            return View(tarifListe);
        }

    }
}
