using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class TarifApiController : Controller
    {
        //
        // GET: /TarifApi/

        public List<Tarif> GetListOfTarifs()
        {
            try
            {
                var businessService = new BusinessService();
                var listeTarifs = businessService.GetListOfTarifs();

                foreach (var tarif in listeTarifs)
                {
                    if (tarif.TarifTraductions != null && tarif.TarifTraductions.ElementAt(0) != null)
                        tarif.LibelleTarif = tarif.TarifTraductions.ElementAt(0).LibelleTarif;
                    else
                        tarif.LibelleTarif = tarif.LibelleTarif;
                }

                return listeTarifs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
