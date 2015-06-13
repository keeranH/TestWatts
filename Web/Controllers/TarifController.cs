using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Benchmark;

namespace Web.Controllers
{
    public class TarifController : Controller
    {
        //
        // GET: /Tarif/

        public ActionResult Index()
        {
            var tarif1 = new TARIF {LIBELLETARIF = "de 50 a 499 collaborateurs", TARIFMENSUEL = 1200};
            var tarif2 = new TARIF { LIBELLETARIF = "de 500 a 999 collaborateurs", TARIFMENSUEL = 2400 };
            var tarif3 = new TARIF { LIBELLETARIF = "de 1000 a 4 999 collaborateurs", TARIFMENSUEL = 4200 };
            var tarif4 = new TARIF { LIBELLETARIF = "de 5000 a 9 999 collaborateurs", TARIFMENSUEL = 5880 };
            var tarif5 = new TARIF { LIBELLETARIF = "de 10000 a 49 999 collaborateurs", TARIFMENSUEL = 10200 };
            var tarif6 = new TARIF { LIBELLETARIF = "Au-dela de 500 000 collaborateurs", TARIFMENSUEL = 15000 };

            var tarifListe = new List<TARIF>();
            tarifListe.Add(tarif1);
            tarifListe.Add(tarif2);
            tarifListe.Add(tarif3);
            tarifListe.Add(tarif4);
            tarifListe.Add(tarif5);
            tarifListe.Add(tarif6);

            return View(tarifListe);
        }

    }
}
