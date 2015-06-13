using System.Collections.Generic;
using System.ServiceModel;

namespace Econocom.Model.Models.Service
{
    [MessageContract]
    public class UploadResult
    {
        [MessageHeader]
        public bool Status;

        [MessageHeader]
        public string Error;

        [MessageHeader]
        public List<string> ErrorLines;

        public UploadResult()
        {
        }
    }
}