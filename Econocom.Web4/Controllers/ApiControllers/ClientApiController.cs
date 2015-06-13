using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class ClientApiController : ApiController
    {

        public Boolean SauveGardeClient(Client client)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauvegardeClient(client);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public Client GetClientById(int clientId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetClientById(clientId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DetailsClient GetDetailsClientById(int Id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDetailsClientById(Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DetailsClient saveDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.saveDetailsClient(detailsClient);
            }

            catch (Exception e)
            {
                throw e;
            }
        }


        public Client saveClient(Client client)
        {
            try
            {              
                var businessService = new BusinessService();
                return businessService.SaveClient(client);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CodeInterneExiste(string code)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.CodeInterneExiste(code);
            }

            catch (Exception e)
            {
                throw e;
            }
        }



    }
}
