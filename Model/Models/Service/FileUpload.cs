using System.IO;
using System.ServiceModel;

namespace Econocom.Model.Models.Service
{
    [MessageContract]
    public class FileUpload
    {
        [MessageHeader]
        public string FileName;

        [MessageBodyMember]
        public Stream Data;

        public FileUpload()
        {
        }
    }
}