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
    public class QuestionApiController : ApiController
    {

        public List<Question> GetQuestions()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetQuestions();
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
