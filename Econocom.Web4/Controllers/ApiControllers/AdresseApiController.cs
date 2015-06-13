using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class AdresseApiController : ApiController
    {



        public Adresse GetAdresseById(int Id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetAdresseById(Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Adresse saveAdress(Adresse adresse)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.saveAdress(adresse);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
