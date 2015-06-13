using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Common;
//using Infrastructure.DTO.Utilisateur;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class LoginApiController : ApiController
    {
        private BusinessService _businessService;
        public LoginApiController()
        {
            _businessService = new BusinessService();
        }

        public LoginApiController(BusinessService businessService)
        {
            _businessService = businessService;
        }

        // GET api/loginapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/loginapi/5
     

        // POST api/loginapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/loginapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/loginapi/5
        public void Delete(int id)
        {
        }


        public Contact GetContact(string email, string password)
        {
          try
          {   var businessService = new BusinessService();
              return businessService.GetContact(email, password);
          }
          catch (Exception e)
          {
              throw e;
          }
        }

        public List<Pays> GetPays()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetPays();
            }
            catch (Exception e)
            {
                throw e;
            }
          
        }

        //public UtilisateurInput GetUtilisateur(string email, string password)
        //{
        //    var utilisateur = new UtilisateurInput();
        //    return utilisateur;
        //}


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
    }
}
