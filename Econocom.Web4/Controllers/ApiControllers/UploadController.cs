using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Service;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class UploadController : ApiController
    {
        public Task<HttpResponseMessage> PostMultipartStream()
        {
            // Check we're uploading a file
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            try
            {

                var streamProvider = new MultipartMemoryStreamProvider();
                // Read the MIME multipart content using the stream provider we just created.
                //var bodyparts =  Request.Content.ReadAsMultipartAsync(streamProvider);

                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<HttpResponseMessage>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                        }

                        // This illustrates how to get the file names.
                        foreach (var file in streamProvider.Contents)
                        {
                            var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                            var headers = file.Headers;
                            var stream = file.ReadAsStreamAsync().Result;
                            //var contents = GetFileContent(stream);
                            var b = new BusinessService();
                            //var bytes = file.ReadAsByteArrayAsync();                     
                            b.SauvegardeDonneeDuFichier(stream, fileName);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);
                    });
                return task;
            }
            catch (System.Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }           
        }

        public List<object> PostFile(DocumentUpload documentUpload)
        {
            try
            {
                var b = new BusinessService();
                var statut = b.SauvegardeDonneeDuFichier(documentUpload);
                return statut;
            }
            catch (System.Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return null;
        }

        public List<object> SauvegardeObjet(Type type, List<object> listeObjets)
        {
            try
            {
                var businessService = new BusinessService();
                var objetsEnErreur = businessService.SauvegardeListe(type, listeObjets);                
                return objetsEnErreur;
            }
            catch (System.Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return null;
        }    
        // GET api/upload/5
        public string Get(int id)
        {
            return "value";
        }
       
    }
}
