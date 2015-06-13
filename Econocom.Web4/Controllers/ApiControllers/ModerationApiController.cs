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
    public class ModerationApiController : ApiController
    {


        public Client GetClientParId(int id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetClientParId(id);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public bool ActualiserClient(Client client)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ActualiserClient(client);
            }
            catch (Exception e)
            {
                throw e;
            }

        }



  
    }
}
